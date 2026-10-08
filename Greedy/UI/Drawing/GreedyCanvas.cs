using Avalonia;
using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.TextFormatting;

namespace Greedy.UI.Drawing;

public class GreedyCanvas : Control
{
	public IScenePainter painter;

	private bool dragInProgress;
	private Point dragStart;
	private Point mousePos;

	private double zoomScale = 1;
	public Point PivotPoint { get; set; }

	public void ResetScale()
	{
		if (painter == null) return;

		var scene = painter.RealSize;
		var w = Width;
		var h = Height;

		var scale = Math.Min(w / scene.Width, h / scene.Height);

		var offsetXpx = (w - scene.Width * scale) / 2.0;
		var offsetYpx = (h - scene.Height * scale) / 2.0;

		zoomScale = scale;
		PivotPoint = new Point(offsetXpx, offsetYpx);
		InvalidateVisual();
	}

	protected override void OnPointerPressed(PointerPressedEventArgs e)
	{
		var props = e.GetCurrentPoint(this).Properties;
		var position = e.GetPosition(this);

		switch (props.PointerUpdateKind)
		{
			case PointerUpdateKind.LeftButtonPressed:
				ShowCoords(position);
				break;
			case PointerUpdateKind.MiddleButtonPressed:
				ResetScale();
				break;
			case PointerUpdateKind.RightButtonPressed:
				dragInProgress = true;
				dragStart = position;
				break;
		}
	}

	protected override void OnPointerReleased(PointerReleasedEventArgs e)
	{
		var props = e.GetCurrentPoint(this).Properties;

		dragInProgress = props.PointerUpdateKind switch
		{
			PointerUpdateKind.RightButtonReleased => false,
			_ => dragInProgress
		};
	}

	private Task showCoordsTask;

	private async Task ShowCoords(Point position, int displayTimeout = 3000)
	{
		mousePos = position;
		InvalidateVisual();
		var delay = Task.Delay(displayTimeout);
		showCoordsTask = delay;
		await showCoordsTask;
		if (delay != showCoordsTask) return;
		mousePos = new Point(0, 0);
		InvalidateVisual();
	}

	protected override void OnPointerMoved(PointerEventArgs e)
	{
		var position = e.GetPosition(this);
		if (!dragInProgress) return;
		var dx = position.X - dragStart.X;
		var dy = position.Y - dragStart.Y;
		PivotPoint += new Point(dx, dy);
		dragStart = position;
		InvalidateVisual();
	}

	protected override void OnPointerWheelChanged(PointerWheelEventArgs e)
	{
		const double step = 1.1;
		var factor = e.Delta.Y > 0 ? step : 1.0 / step;
		ZoomAt(factor, e.GetPosition(this));
		e.Handled = true;
	}

	private void ZoomAt(double factor, Point focal)
	{
		var old = zoomScale;
		var next = Math.Clamp(old * factor, 0.05, 4.0);
		if (Math.Abs(next - old) < 1e-6) return;

		var k = next / old;
		PivotPoint = new Point(
			(1 - k) * focal.X + k * PivotPoint.X,
			(1 - k) * focal.Y + k * PivotPoint.Y);

		zoomScale = next;
		InvalidateVisual();
	}

	public override void Render(DrawingContext drawingContext)
	{
		if (painter == null) return;

		using (drawingContext.PushTransform(
			       Matrix.CreateScale(zoomScale, zoomScale) * Matrix.CreateTranslation(PivotPoint.X, PivotPoint.Y)))
		{
			painter.Paint(drawingContext, zoomScale);

			if (mousePos.X == 0 || mousePos.Y == 0) return;

			DrawText(drawingContext);
		}
	}

	void DrawText(DrawingContext ctx)
	{
		var world = (mousePos - PivotPoint) / zoomScale;

		var tf = new Typeface("Segoe UI Light");
		var layout = new TextLayout(
			$"(X: {Math.Floor(world.X / painter.CellSize.Width)}; " +
			$"Y: {Math.Floor(world.Y / painter.CellSize.Height)})",
			typeface: tf,
			fontSize: 16 / zoomScale,
			foreground: Brushes.Red,
			textAlignment: TextAlignment.Left,
			textWrapping: TextWrapping.NoWrap,
			maxWidth: double.PositiveInfinity);

		layout.Draw(ctx, world);
	}
}