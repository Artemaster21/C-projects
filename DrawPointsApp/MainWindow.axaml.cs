using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Input;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DrawPointsApp
{
    public partial class MainWindow : Window
    {
        private List<(double X, double Y)> points = new();

        public MainWindow()
        {
            InitializeComponent();
            this.KeyDown += OnKeyDown;
            DrawCanvas.AttachedToVisualTree += (_, _) => AskForPoints();
        }

        private async void AskForPoints()
        {
            var dialog = new Window
            {
                Width = 500,
                Height = 150,
                Title = "Введите точки",
                Content = new TextBox { Name = "InputBox", Watermark = "(220,266) (240,327) (263,297)", AcceptsReturn = false }
            };
            var inputBox = (TextBox)dialog.Content!;
            dialog.ShowDialog(this);
            inputBox.KeyDown += (_, e) =>
            {
                if (e.Key == Key.Enter)
                {
                    string input = inputBox.Text ?? "";
                    ParsePoints(input);
                    dialog.Close();
                    DrawShape();
                }
            };
        }

        private void ParsePoints(string input)
        {
            points.Clear();
            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            foreach (var part in parts)
            {
                var trimmed = part.Trim('(', ')');
                var nums = trimmed.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (nums.Length == 2 &&
                    double.TryParse(nums[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double x) &&
                    double.TryParse(nums[1], NumberStyles.Any, CultureInfo.InvariantCulture, out double y))
                {
                    points.Add((x, y));
                }
            }
        }

        private void DrawShape()
        {
            if (points.Count < 2)
                return;

            var canvas = DrawCanvas;
            var dc = canvas.CreateDrawingContext(null);
            var brush = Brushes.Blue;
            var pen = new Pen(brush, 2);

            // Найдём смещение для центрирования
            double minX = points.Min(p => p.X);
            double maxX = points.Max(p => p.X);
            double minY = points.Min(p => p.Y);
            double maxY = points.Max(p => p.Y);
            double offsetX = (canvas.Bounds.Width - (maxX - minX)) / 2 - minX;
            double offsetY = (canvas.Bounds.Height - (maxY - minY)) / 2 - minY;

            // Нарисуем линии
            var geo = new StreamGeometry();
            using (var ctx = geo.Open())
            {
                ctx.BeginFigure(new Avalonia.Point(points[0].X + offsetX, points[0].Y + offsetY), false);
                for (int i = 1; i < points.Count; i++)
                    ctx.LineTo(new Avalonia.Point(points[i].X + offsetX, points[i].Y + offsetY));
                ctx.EndFigure(true);
            }

            dc.DrawGeometry(null, pen, geo);

            // Точки
            foreach (var p in points)
            {
                dc.DrawEllipse(Brushes.Red, null, new Avalonia.Point(p.X + offsetX, p.Y + offsetY), 3, 3);
            }

            dc.Dispose();
        }

        private void OnKeyDown(object? sender, KeyEventArgs e)
        {
            if (e.Key == Key.R)
                AskForPoints();
        }
    }
}
