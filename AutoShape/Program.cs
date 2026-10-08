using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class PointVisualizer
{
    static (List<Point> points, List<string> possibleAnswers) ReadPointsFromFile(string filename)
    {
        var points = new List<Point>();
        var possibleAnswers = new List<string>();

        if (!File.Exists(filename))
        {
            Console.WriteLine($"Файл {filename} не найден!");
            return (points, possibleAnswers);
        }

        string content = File.ReadAllText(filename);
        Console.WriteLine($"Содержимое файла: {content}");

        var match = Regex.Match(content, @"Possible\s*answers?\s*=\s*([^|]+)");

        if (match.Success)
        {
            string answersStr = match.Groups[1].Value.Trim().ToLower();

            // Обработка слитного написания
            answersStr = answersStr.Replace("equilateraltriangle", "equilateral,triangle")
                                   .Replace("righttriangle", "triangle");

            possibleAnswers = answersStr.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                                       .Select(a => a.Trim())
                                       .Distinct()
                                       .ToList();
            Console.WriteLine($"Возможные ответы: {string.Join(", ", possibleAnswers)}");
        }

        // Чтение точек
        var matches = Regex.Matches(content, @"\((\d+),(\d+)\)");
        foreach (Match m in matches)
        {
            if (int.TryParse(m.Groups[1].Value, out int x) &&
                int.TryParse(m.Groups[2].Value, out int y))
            {
                points.Add(new Point(x, y));
            }
        }

        Console.WriteLine($"Прочитано точек: {points.Count}");
        return (points, possibleAnswers);
    }

    static (RectangleF rect, double angle) FindMinBoundingBox(List<Point> points)
    {
        if (points.Count < 2) return (new RectangleF(), 0);

        var hull = GetConvexHull(points);
        if (hull.Count < 3) hull = points;

        double minArea = double.MaxValue;
        RectangleF bestRect = new RectangleF();
        double bestAngle = 0;

        for (double angle = 0; angle < 90; angle += 1)
        {
            var rotated = RotatePoints(hull, -angle);
            double minX = rotated.Min(p => p.X);
            double maxX = rotated.Max(p => p.X);
            double minY = rotated.Min(p => p.Y);
            double maxY = rotated.Max(p => p.Y);

            double width = maxX - minX;
            double height = maxY - minY;
            double area = width * height;

            if (area < minArea)
            {
                minArea = area;
                bestRect = new RectangleF((float)minX, (float)minY, (float)width, (float)height);
                bestAngle = angle;
            }
        }

        return (bestRect, bestAngle);
    }

    static double CalculateArea(List<Point> points)
    {
        if (points.Count < 3) return 0;

        var hull = GetConvexHull(points);
        if (hull.Count < 3) hull = points;

        double area = 0;
        for (int i = 0; i < hull.Count; i++)
        {
            var p1 = hull[i];
            var p2 = hull[(i + 1) % hull.Count];
            area += p1.X * p2.Y - p2.X * p1.Y;
        }

        return Math.Abs(area) / 2.0;
    }

    static string DetermineFigureType(List<Point> points, List<string> possibleAnswers)
    {
        if (points.Count < 3 || possibleAnswers.Count == 0) return "unknown";

        var (rect, angle) = FindMinBoundingBox(points);
        double figureArea = CalculateArea(points);
        double rectArea = rect.Width * rect.Height;
        double ratio = rectArea > 0 ? figureArea / rectArea : 0;
        double aspectRatio = rect.Width > 0 ? Math.Min(rect.Width, rect.Height) / Math.Max(rect.Width, rect.Height) : 0;

        Console.WriteLine($"\n=== Результаты анализа ===");
        Console.WriteLine($"Площадь фигуры: {figureArea:F2}");
        Console.WriteLine($"Площадь прямоугольника: {rectArea:F2}");
        Console.WriteLine($"Отношение: {ratio:F3}");
        Console.WriteLine($"Соотношение сторон: {aspectRatio:F3}");
        Console.WriteLine($"Угол: {angle:F1}°");

        // Логика определения
        if (possibleAnswers.Contains("circle") && aspectRatio > 0.9 && ratio > 0.75)
            return "circle";
        if (possibleAnswers.Contains("ellipse") && ratio > 0.65)
            return "ellipse";
        if (possibleAnswers.Contains("square") && aspectRatio > 0.9 && ratio > 0.8)
            return "square";
        if (possibleAnswers.Contains("rectangle") && ratio > 0.8)
            return "rectangle";
        if (possibleAnswers.Contains("triangle") && ratio < 0.65)
            return possibleAnswers.Contains("equilateral") && IsEquilateral(points) ? "equilateral" : "triangle";

        return possibleAnswers.FirstOrDefault() ?? "unknown";
    }

    static bool IsEquilateral(List<Point> points)
    {
        var hull = GetConvexHull(points);
        if (hull.Count != 3) return false;

        double a = Distance(hull[0], hull[1]);
        double b = Distance(hull[1], hull[2]);
        double c = Distance(hull[2], hull[0]);
        double avg = (a + b + c) / 3;

        return Math.Abs(a - avg) / avg < 0.15 &&
               Math.Abs(b - avg) / avg < 0.15 &&
               Math.Abs(c - avg) / avg < 0.15;
    }

    static List<Point> GetConvexHull(List<Point> points)
    {
        if (points.Count < 4) return points.Distinct().ToList();

        var sorted = points.Distinct()
            .OrderBy(p => p.X)
            .ThenBy(p => p.Y)
            .ToList();

        var lower = new List<Point>();
        foreach (var p in sorted)
        {
            while (lower.Count >= 2 && Cross(lower[lower.Count - 2], lower[lower.Count - 1], p) <= 0)
                lower.RemoveAt(lower.Count - 1);
            lower.Add(p);
        }

        var upper = new List<Point>();
        for (int i = sorted.Count - 1; i >= 0; i--)
        {
            var p = sorted[i];
            while (upper.Count >= 2 && Cross(upper[upper.Count - 2], upper[upper.Count - 1], p) <= 0)
                upper.RemoveAt(upper.Count - 1);
            upper.Add(p);
        }

        lower.RemoveAt(lower.Count - 1);
        upper.RemoveAt(upper.Count - 1);
        lower.AddRange(upper);
        return lower;
    }

    static double Cross(Point a, Point b, Point c) =>
        (b.X - a.X) * (c.Y - a.Y) - (b.Y - a.Y) * (c.X - a.X);

    static List<PointF> RotatePoints(List<Point> points, double angleDeg)
    {
        double angleRad = angleDeg * Math.PI / 180;
        double cos = Math.Cos(angleRad);
        double sin = Math.Sin(angleRad);
        double cx = points.Average(p => p.X);
        double cy = points.Average(p => p.Y);

        return points.Select(p =>
        {
            double x = p.X - cx;
            double y = p.Y - cy;
            return new PointF(
                (float)(x * cos - y * sin + cx),
                (float)(x * sin + y * cos + cy)
            );
        }).ToList();
    }

    static double Distance(Point a, Point b) =>
        Math.Sqrt(Math.Pow(b.X - a.X, 2) + Math.Pow(b.Y - a.Y, 2));

    static void CreateVisualization(List<Point> points, string figureType, List<string> possibleAnswers, string outputPath)
    {
        if (points.Count == 0) return;

        int minX = points.Min(p => p.X);
        int maxX = points.Max(p => p.X);
        int minY = points.Min(p => p.Y);
        int maxY = points.Max(p => p.Y);

        int padding = 50;
        int width = maxX - minX + 2 * padding;
        int height = maxY - minY + 2 * padding;

        using (var bitmap = new Bitmap(width, height))
        using (var g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.White);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            PointF Transform(Point p) =>
                new PointF(p.X - minX + padding, height - (p.Y - minY + padding));

            // Точки
            foreach (var p in points)
            {
                var t = Transform(p);
                g.FillEllipse(Brushes.Blue, t.X - 2, t.Y - 2, 4, 4);
            }

            // Минимальный прямоугольник
            var (rect, angle) = FindMinBoundingBox(points);
            var mbbPen = new Pen(Color.Red, 2);
            var mbbBrush = new SolidBrush(Color.FromArgb(30, Color.Red));

            float cx = rect.X + rect.Width / 2;
            float cy = rect.Y + rect.Height / 2;

            PointF[] rectPts = {
                new PointF(rect.X, rect.Y),
                new PointF(rect.X + rect.Width, rect.Y),
                new PointF(rect.X + rect.Width, rect.Y + rect.Height),
                new PointF(rect.X, rect.Y + rect.Height)
            };

            double rad = -angle * Math.PI / 180;
            double cos = Math.Cos(rad);
            double sin = Math.Sin(rad);

            for (int i = 0; i < 4; i++)
            {
                double x = rectPts[i].X - cx;
                double y = rectPts[i].Y - cy;
                double rx = x * cos - y * sin + cx;
                double ry = x * sin + y * cos + cy;
                rectPts[i] = Transform(new Point((int)rx, (int)ry));
            }

            g.FillPolygon(mbbBrush, rectPts);
            g.DrawPolygon(mbbPen, rectPts);

            // Текст
            var font = new Font("Arial", 10);
            g.DrawString($"Figure: {figureType}", font, Brushes.Red, 10, 10);
            g.DrawString($"Possible: {string.Join(", ", possibleAnswers)}", font, Brushes.Blue, 10, 30);

            double figureArea = CalculateArea(points);
            double rectArea = rect.Width * rect.Height;
            double ratio = rectArea > 0 ? figureArea / rectArea : 0;
            g.DrawString($"Area ratio: {ratio:F3}", font, Brushes.Black, 10, 50);

            bitmap.Save(outputPath, ImageFormat.Png);
            Console.WriteLine($"Изображение сохранено: {outputPath}");
        }
    }

    static void ShowImage(string imagePath)
    {
        if (!File.Exists(imagePath))
        {
            Console.WriteLine($"Файл изображения не найден: {imagePath}");
            return;
        }

        if (OperatingSystem.IsWindows())
        {
            try
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = imagePath,
                    UseShellExecute = true
                });
                Console.WriteLine("Изображение открыто");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при открытии изображения: {ex.Message}");
            }
        }
        else
        {
            Console.WriteLine($"Изображение сохранено по пути: {imagePath}");
            Console.WriteLine("Для просмотра откройте файл вручную");
        }
    }

    static void Main()
    {
        Console.WriteLine("=== Point Figure Analyzer ===\n");

        string inputFile = @"points1.txt";
        var (points, possibleAnswers) = ReadPointsFromFile(inputFile);

        if (points.Count > 0)
        {
            if (possibleAnswers.Count == 0)
            {
                Console.WriteLine("Возможные ответы не указаны, использую стандартные");
                possibleAnswers = new List<string> { "circle", "square", "triangle", "ellipse", "rectangle", "equilateral" };
            }

            string figureType = DetermineFigureType(points, possibleAnswers);
            Console.WriteLine($"\nОпределенная фигура: {figureType}");

            string outputFile = @"figure_output.png";
            CreateVisualization(points, figureType, possibleAnswers, outputFile);

            // Показ изображения
            ShowImage(outputFile);
        }
        else
            Console.WriteLine("Нет точек для анализа");

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.Read();
    }
}