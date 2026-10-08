//using System.Reflection;

//namespace Reflection.Randomness;

//[AttributeUsage(AttributeTargets.Property)]
//public class FromDistributionAttribute : Attribute
//{
//    public Type DistributionType { get; }
//    public object[] Arguments { get; }

//    public FromDistributionAttribute(Type distributionType, params object[] arguments)
//    {
//        DistributionType = distributionType;
//        Arguments = arguments;
//    }
//}

//public class Generator<T> where T : new()
//{
//    private readonly Dictionary<PropertyInfo, IContinuousDistribution> _distributions = new();

//    public Generator()
//    {
//        var properties = typeof(T)
//            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
//            .Select(p => new
//            {
//                Property = p,
//                Attribute = p.GetCustomAttribute<FromDistributionAttribute>()
//            })
//            .Where(x => x.Attribute != null);

//        foreach (var item in properties)
//        {
//            var attribute = item.Attribute;
//            var distributionType = attribute.DistributionType;

//            if (!typeof(IContinuousDistribution).IsAssignableFrom(distributionType))
//                throw new ArgumentException(
//                    $"Type {distributionType.Name} does not implement IContinuousDistribution");

//            var constructor = distributionType.GetConstructor(
//                attribute.Arguments.Select(a => a.GetType()).ToArray());

//            if (constructor == null)
//                throw new ArgumentException(
//                    $"Type {distributionType.Name} does not have a constructor with the specified arguments");

//            var distribution = (IContinuousDistribution)constructor.Invoke(attribute.Arguments);
//            _distributions[item.Property] = distribution;
//        }
//    }

//    public T Generate(Random rnd)
//    {
//        var instance = new T();

//        foreach (var pair in _distributions)
//        {
//            pair.Key.SetValue(instance, pair.Value.Generate(rnd));
//        }

//        return instance;
//    }
//}