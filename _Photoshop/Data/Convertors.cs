namespace MyPhotoshop;

public static class Convertors
{
	public static Photo Bitmap2Photo(Bitmap bmp)
	{
		var photo = new Photo
		{
			width = bmp.Width,
			height = bmp.Height,
			data = new double[bmp.Width, bmp.Height, 3],
		};

		for (var x = 0; x < bmp.Width; x++)
		for (var y = 0; y < bmp.Height; y++)
		{
			var pixel = bmp.GetPixel(x, y);
			photo.data[x, y, 0] = (double)pixel.R / 255;
			photo.data[x, y, 1] = (double)pixel.G / 255;
			photo.data[x, y, 2] = (double)pixel.B / 255;
		}
		return photo;
	}

	static int ToChannel(double val)
	{
		if (val is < 0 or > 1)
			throw new Exception($"Wrong channel value {val} (the value must be between 0 and 1");
		return (int)(val * 255);
	}

	public static Bitmap Photo2Bitmap(Photo photo)
	{
		var bmp = new Bitmap(photo.width, photo.height);
		for (var x = 0; x < bmp.Width; x++)
		for (var y = 0; y < bmp.Height; y++)
			bmp.SetPixel(x, y, Color.FromArgb(
				ToChannel(photo.data[x, y, 0]),
				ToChannel(photo.data[x, y, 1]),
				ToChannel(photo.data[x, y, 2])));

		return bmp;
	}
}