using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace func_rocket.UI;

public class RocketCanvas : UserControl
{
	public RocketCanvas()
	{
		var prefix = "ui/images";
		rocket = new Bitmap(prefix + "/rocket.png");
		target = new Bitmap(prefix + "/target.png");
		ClipToBounds = true;
	}

	private readonly Bitmap rocket;
	private readonly Bitmap target;
	public RocketModel Model;

	public override void Render(DrawingContext context)
	{
		base.Render(context);

		DrawTo(context);
	}

	public void DrawTo(DrawingContext context)
	{
		context.FillRectangle(Brushes.Beige, new Rect(DesiredSize));

		DrawGravity(context);

		var targetRect =
			new Rect(
				new Point(
					Model.CurrentLevel.Target.X - target.Size.Width / 2,
					Model.CurrentLevel.Target.Y - target.Size.Height / 2),
				target.Size);
		context.DrawImage(target, targetRect);

		if (!Model.CurrentLevel.IsCompleted)
		{
			var r = Model.CurrentLevel.Rocket;
			var center = new Point(r.Location.X, r.Location.Y);
			var angleRad = (90.0 * Math.PI / 180.0) + r.Direction;

			var rect = new Rect(
				center.X - rocket.Size.Width  / 2,
				center.Y - rocket.Size.Height / 2,
				rocket.Size.Width,
				rocket.Size.Height);

			var m =
				Matrix.CreateTranslation(-center.X, -center.Y) *
				Matrix.CreateRotation(angleRad) *
				Matrix.CreateTranslation( center.X,  center.Y);

			using (context.PushTransform(m))
			{
				context.DrawImage(rocket, rect);
			}
		}
	}

	private void DrawGravity(DrawingContext context)
	{
		var pen = new Pen(Brushes.DeepSkyBlue);
		Action<Vector, Vector> draw = (a, b) => context.DrawLine(pen, new Point(a.X, a.Y), new Point(b.X, b.Y));
		for (var x = 0; x < Model.SpaceSize.X; x += 50)
		for (var y = 0; y < Model.SpaceSize.Y; y += 50)
		{
			var p1 = new Vector(x, y);
			var v = Model.CurrentLevel.Gravity(Model.SpaceSize, p1);
			if (double.IsInfinity(v.X) || double.IsInfinity(v.Y))
				continue;
			var p2 = p1 + 20 * v;
			var end1 = p2 - 8 * v.Rotate(0.5);
			var end2 = p2 - 8 * v.Rotate(-0.5);

			draw(p1, p2);
			draw(p2, end1);
			draw(p2, end2);
		}
	}
}