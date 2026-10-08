using System.Linq.Expressions;
using System.Reflection;

namespace Reflection.Randomness;

[AttributeUsage(AttributeTargets.Property)]
public class FromDistributionAttribute : Attribute
{
    public Type DistributionType { get; }
    public object[] Arguments { get; }

    public FromDistributionAttribute(Type distributionType, params object[] arguments)
    {
        DistributionType = distributionType;
        Arguments = arguments;
    }
}

public class Generator<T> where T : new()
{
    private static readonly Func<Random, T> _generate;

    static Generator()
    {
        var rndParam = Expression.Parameter(typeof(Random), "rnd");
        var properties = GetPropertiesWithAttributes();
        var bindings = CreateBindings(properties, rndParam);
        _generate = CompileGenerator(bindings, rndParam);
    }

    public T Generate(Random rnd)
    {
        return _generate(rnd);
    }

    private static List<(PropertyInfo Property, FromDistributionAttribute Attribute)> GetPropertiesWithAttributes()
    {
        return typeof(T)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p => new
            {
                Property = p,
                Attribute = p.GetCustomAttribute<FromDistributionAttribute>()
            })
            .Where(x => x.Attribute != null)
            .Select(x => (x.Property, x.Attribute))
            .ToList();
    }

    private static List<MemberBinding> CreateBindings(
        List<(PropertyInfo Property, FromDistributionAttribute Attribute)> properties,
        ParameterExpression rndParam)
    {
        var bindings = new List<MemberBinding>();

        foreach (var (property, attribute) in properties)
        {
            var distribution = CreateDistribution(attribute);
            var generateCall = CreateGenerateCall(distribution, rndParam);
            var binding = Expression.Bind(property, generateCall);
            bindings.Add(binding);
        }

        return bindings;
    }

    private static IContinuousDistribution CreateDistribution(FromDistributionAttribute attribute)
    {
        var distributionType = attribute.DistributionType;

        if (!typeof(IContinuousDistribution).IsAssignableFrom(distributionType))
            throw new ArgumentException(
                $"Type {distributionType.Name} does not implement IContinuousDistribution");

        var constructor = distributionType.GetConstructor(
            attribute.Arguments.Select(a => a.GetType()).ToArray());

        if (constructor == null)
            throw new ArgumentException(
                $"Type {distributionType.Name} does not have a constructor with the specified arguments");

        return (IContinuousDistribution)constructor.Invoke(attribute.Arguments);
    }

    private static MethodCallExpression CreateGenerateCall(
        IContinuousDistribution distribution,
        ParameterExpression rndParam)
    {
        var distributionConstant = Expression.Constant(
            distribution,
            typeof(IContinuousDistribution));

        return Expression.Call(
            distributionConstant,
            typeof(IContinuousDistribution).GetMethod(nameof(IContinuousDistribution.Generate)),
            rndParam);
    }

    private static Func<Random, T> CompileGenerator(
        List<MemberBinding> bindings,
        ParameterExpression rndParam)
    {
        var newExpression = Expression.New(typeof(T));
        var memberInit = Expression.MemberInit(newExpression, bindings);
        var lambda = Expression.Lambda<Func<Random, T>>(memberInit, rndParam);

        return lambda.Compile();
    }
}