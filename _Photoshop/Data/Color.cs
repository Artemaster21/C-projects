namespace MyPhotoshop;

public record Color(int A, int R, int G, int B)
{
	public static Color FromArgb(int a, int r, int g, int b)
	{
		return new Color(a, r, g, b);
	}

	public static Color FromArgb(int r, int g, int b)
	{
		return FromArgb(255, r, g, b);
	}
}