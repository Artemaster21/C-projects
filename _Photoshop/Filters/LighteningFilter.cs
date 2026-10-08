namespace MyPhotoshop;

public class LighteningFilter : IFilter
{
	public ParameterInfo[] GetParameters()
	{
		return new[]
		{
			new ParameterInfo
			{
				Name = "Коэффициент",
				MaxValue = 10,
				MinValue = 0,
				Increment = 0.1,
				DefaultValue = 1
			}
		};
	}

	public override string ToString()
	{
		return "Осветление/затемнение";
	}

	public Photo Process(Photo original, double[] parameters)
	{
		var result = new Photo();
		result.width = original.width;
		result.height = original.height;
		result.data = new double[result.width, result.height, 3];

		for (var x = 0; x < result.width; x++)
		for (var y = 0; y < result.height; y++)
		for (var z = 0; z < 3; z++)
			result.data[x, y, z] = original.data[x, y, z] * parameters[0];
		return result;
	}
}