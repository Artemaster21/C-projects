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
//    private readonly List<(PropertyInfo Property, IContinuousDistribution Distribution)> _properties = new();

//    public Generator()
//    {
//        var type = typeof(T);
//        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

//        foreach (var property in properties)
//        {
//            var attribute = property.GetCustomAttribute<FromDistributionAttribute>();
//            if (attribute == null) continue;

//            if (!typeof(IContinuousDistribution).IsAssignableFrom(attribute.DistributionType))
//                throw new ArgumentException($"Type {attribute.DistributionType.Name} does not implement IContinuousDistribution");

//            IContinuousDistribution distribution;
//            try
//            {
//                distribution = (IContinuousDistribution)Activator.CreateInstance(
//                    attribute.DistributionType,
//                    attribute.Arguments);
//            }
//            catch (Exception ex)
//            {
//                throw new ArgumentException(
//                    $"Cannot create distribution {attribute.DistributionType.Name}: {ex.Message}");
//            }

//            _properties.Add((property, distribution));
//        }
//    }

//    public T Generate(Random rnd)
//    {
//        var instance = new T();
//        foreach (var (property, distribution) in _properties)
//        {
//            property.SetValue(instance, distribution.Generate(rnd));
//        }
//        return instance;
//    }
//}