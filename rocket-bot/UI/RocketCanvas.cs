using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace rocket_bot.UI;

public class RocketCanvas : Control
{
    public RocketModel Model { get; set; } = default!;

    private const string prefix = "UI/images";
    private readonly Bitmap rocketBitmap = new($"{prefix}/rocket.png");
    private readonly Bitmap flagBitmap = new($"{prefix}/flag.png");

    public override void Render(DrawingContext context)
    {
        base.Render(context);

        context.FillRectangle(Brushes.Beige, Bounds);

        var (level, rocket, channel) = Model.Data;

        var pen = new Pen();

        for (var i = 0; i < level.Checkpoints.Length; ++i)
        {
            var flagSize = flagBitmap.Size;
            var flagCenter = new Point(level.Checkpoints[i].X, level.Checkpoints[i].Y);
            var flagPosition = new Point(
                flagCenter.X - flagSize.Width / 2,
                flagCenter.Y - flagSize.Height / 2
            );
            var flagRect = new Rect(flagPosition, flagSize);
            context.DrawImage(flagBitmap, flagRect);

            if (rocket.TakenCheckpointsCount % level.Checkpoints.Length == i)
                context.DrawEllipse(Brushes.Gold, pen, flagCenter, 10, 10);
        }

        var center = new Point(rocket.Location.X, rocket.Location.Y);
        var angleRad = (90.0 * Math.PI / 180.0) + rocket.Direction;

        var rect = new Rect(
	        center.X - rocketBitmap.Size.Width  / 2,
	        center.Y - rocketBitmap.Size.Height / 2,
	        rocketBitmap.Size.Width,
	        rocketBitmap.Size.Height);

        var m =
	        Matrix.CreateTranslation(-center.X, -center.Y) *
	        Matrix.CreateRotation(angleRad) *
	        Matrix.CreateTranslation( center.X,  center.Y);

        using (context.PushTransform(m))
        {
	        context.DrawImage(rocketBitmap, rect);
        }
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        rocketBitmap.Dispose();
        flagBitmap.Dispose();
    }
}
