namespace MyPhotoshop;

public interface IFilter
{
	/// <summary>
	/// Этот класс служит графическим элементом управления, который связан с NumericUpDown-элементом
	/// ввода для изменения значения счётчика.
	/// </summary>
	/// <returns></returns>
	ParameterInfo[] GetParameters();
	/// <summary>
	/// Этот класс предназначен для хранения, после десериализации, в коллекции всех параметров,
	/// чтобы вызвать метод с набором параметров, возвращаемых методом GetParameters.
	/// </summary>
	/// <param name="original"></param>
	/// <param name="parameters"></param>
	/// <returns></returns>
	Photo Process(Photo original, double[] parameters);
}