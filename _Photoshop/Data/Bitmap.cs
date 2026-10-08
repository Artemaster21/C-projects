using Avalonia;
using Avalonia.Media.Imaging;
using Avalonia.Platform;

namespace MyPhotoshop;

public class Bitmap : IDisposable
{
	public int Width => _bitmap.PixelSize.Width;
	public int Height => _bitmap.PixelSize.Height;

	private readonly WriteableBitmap _bitmap;

	public Bitmap(string path)
	{
		using var source = new Avalonia.Media.Imaging.Bitmap(path);
		var size = source.PixelSize;

		_bitmap = new WriteableBitmap(
			size,
			new Vector(96, 96),
			PixelFormat.Bgra8888,
			AlphaFormat.Premul);

		using var dst = _bitmap.Lock();
		source.CopyPixels(dst, AlphaFormat.Opaque);
	}

	public Bitmap(int width, int height)
	{
		_bitmap = new WriteableBitmap(new PixelSize(width, height), new Vector(96, 96), PixelFormat.Bgra8888);
	}

	public Color GetPixel(int x, int y)
	{
		using var locked = _bitmap.Lock();
		unsafe
		{
			var ptr = (uint*)locked.Address;
			var stride = locked.RowBytes / 4;
			var pixel = ptr[y * stride + x];
			var a = (byte)(pixel >> 24 & 0xFF);
			var r = (byte)(pixel >> 16 & 0xFF);
			var g = (byte)(pixel >> 8 & 0xFF);
			var b = (byte)(pixel & 0xFF);
			return Color.FromArgb(a, r, g, b);
		}
	}

	public void SetPixel(int x, int y, Color color)
	{
		using var locked = _bitmap.Lock();
		unsafe
		{
			var ptr = (uint*)locked.Address;
			var stride = locked.RowBytes / 4;

			var a = (uint)(byte)color.A;
			var r = (uint)(byte)color.R;
			var g = (uint)(byte)color.G;
			var b = (uint)(byte)color.B;

			ptr[y * stride + x] = a << 24 | r << 16 | g << 8 | b;
		}
	}

	public void Save(string path)
	{
		using var fs = File.Create(path);
		_bitmap.Save(fs);
	}

	public Avalonia.Media.Imaging.Bitmap AsAvaloniaBitmap() => _bitmap;

	public void Dispose() => _bitmap.Dispose();
}