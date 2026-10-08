using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

class Program
{
    struct Point
    {
        public double X, Y;
        public Point(double x, double y) { X = x; Y = y; }
    }

    static void Main()
    {
        string filePath = @"C:\\Users\\Артем\\source\\repos\\shape\\points.txt";

        if (!File.Exists(filePath))
        {
            Console.WriteLine("Файл не найден.");
            return;
        }

        string fileContent = File.ReadAllText(filePath);

        var points = new List<Point>();

        foreach (var item in fileContent.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries))
        {
            string s = item.Trim('(', ')');
            var parts = s.Split(',');
            if (parts.Length == 2 &&
                double.TryParse(parts[0], NumberStyles.Float, CultureInfo.InvariantCulture, out double x) &&
                double.TryParse(parts[1], NumberStyles.Float, CultureInfo.InvariantCulture, out double y))
            {
                points.Add(new Point(x, y));
            }
        }

        if (points.Count < 3)
        {
            Console.WriteLine("Недостаточно точек для анализа.");
            return;
        }

        var hull = ConvexHull(points.ToArray());
        string figure = DetermineFigure(hull);
        Console.WriteLine($"Фигура: {figure}");
    }

    static double Cross(Point O, Point A, Point B)
    {
        return (A.X - O.X) * (B.Y - O.Y) - (A.Y - O.Y) * (B.X - O.X);
    }

    static Point[] ConvexHull(Point[] points)
    {
        if (points.Length <= 3) return points;

        var sorted = points.OrderBy(p => p.X).ThenBy(p => p.Y).ToArray();
        List<Point> lower = new List<Point>();
        List<Point> upper = new List<Point>();

        foreach (var p in sorted)
        {
            while (lower.Count >= 2 && Cross(lower[^2], lower[^1], p) <= 0)
                lower.RemoveAt(lower.Count - 1);
            lower.Add(p);
        }

        for (int i = sorted.Length - 1; i >= 0; i--)
        {
            var p = sorted[i];
            while (upper.Count >= 2 && Cross(upper[^2], upper[^1], p) <= 0)
                upper.RemoveAt(upper.Count - 1);
            upper.Add(p);
        }

        lower.RemoveAt(lower.Count - 1);
        upper.RemoveAt(upper.Count - 1);
        lower.AddRange(upper);

        return lower.ToArray();
    }

    static string DetermineFigure(Point[] hull)
    {
        if (hull.Length < 3) return "Неизвестная";

        if (hull.Length == 4)
        {
            double[] dists = new double[4];
            for (int i = 0; i < 4; i++)
                dists[i] = Distance(hull[i], hull[(i + 1) % 4]);

            bool sidesEqual = Math.Abs(dists[0] - dists[1]) < 1e-5 &&
                              Math.Abs(dists[1] - dists[2]) < 1e-5 &&
                              Math.Abs(dists[2] - dists[3]) < 1e-5;

            if (sidesEqual) return "square";
            else return "equilateraltriangle";
        }

        Point center = new Point(hull.Average(p => p.X), hull.Average(p => p.Y));
        double avgDist = hull.Average(p => Distance(p, center));
        double maxDiff = hull.Max(p => Math.Abs(Distance(p, center) - avgDist));

        if (maxDiff / avgDist < 0.05) return "circle";

        return "Неизвестная( equilateraltriangle )";
    }

    static double Distance(Point a, Point b)
    {
        double dx = a.X - b.X;
        double dy = a.Y - b.Y;
        return Math.Sqrt(dx * dx + dy * dy);
    }
}