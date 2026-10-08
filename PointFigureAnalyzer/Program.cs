using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

class PointVisualizer
{
    public static List<Point> ReadPointsFromFile(string filename)
    {
        var points = new List<Point>();

        try
        {
            if (!File.Exists(filename))
            {
                Console.WriteLine($"Файл {filename} не найден!");
                return points;
            }

            // Читаем все содержимое файла
            string content = File.ReadAllText(filename);
            Console.WriteLine($"Содержимое файла: {content}");

            // Используем регулярное выражение для поиска всех пар координат
            var matches = Regex.Matches(content, @"\((\d+),(\d+)\)");

            foreach (Match match in matches)
            {
                if (match.Groups.Count == 3 &&
                    int.TryParse(match.Groups[1].Value, out int x) &&
                    int.TryParse(match.Groups[2].Value, out int y))
                {
                    points.Add(new Point(x, y));
                    Console.WriteLine($"Добавлена точка: ({x}, {y})");
                }
            }

            Console.WriteLine($"Успешно прочитано {points.Count} точек");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка чтения чтения файла: {ex.Message}");
        }

        return points;
    }

    public static void CreateVisualization(List<Point> points, string outputPath)
    {
        if (points == null || points.Count == 0)
        {
            Console.WriteLine("Нет точек для визуализации");
            return;
        }

        // Определяем границы
        int minX = points.Min(p => p.X);
        int maxX = points.Max(p => p.X);
        int minY = points.Min(p => p.Y);
        int maxY = points.Max(p => p.Y);

        Console.WriteLine($"Границы: X({minX}-{maxX}), Y({minY}-{maxY})");

        // Создаем изображение с отступами
        int padding = 50;
        int width = maxX - minX + 2 * padding;
        int height = maxY - minY + 2 * padding;

        using (Bitmap bitmap = new Bitmap(width, height))
        using (Graphics graphics = Graphics.FromImage(bitmap))
        {
            // Белый фон
            graphics.Clear(Color.White);

            // Настраиваем сглаживание
            graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // Преобразование координат
            Func<Point, PointF> transform = p =>
                new PointF(p.X - minX + padding, height - (p.Y - minY + padding));

            // Рисуем точки
            var brush = new SolidBrush(Color.Blue);
            foreach (var point in points)
            {
                var transformed = transform(point);
                graphics.FillEllipse(brush, transformed.X - 1, transformed.Y - 1, 3, 3);
            }

            // Рисуем прямоугольник
            var rect = new RectangleF(padding, padding, maxX - minX, maxY - minY);
            var pen = new Pen(Color.Red, 2) { DashStyle = System.Drawing.Drawing2D.DashStyle.Dash };
            graphics.DrawRectangle(pen, rect.X, rect.Y, rect.Width, rect.Height);

            // Подписи
            var font = new Font("Arial", 10);
            graphics.DrawString($"Points: {points.Count}", font, Brushes.Black, 10, 10);
            graphics.DrawString($"Bounds: X({minX}-{maxX}) Y({minY}-{maxY})",
                              font, Brushes.Black, 10, 30);
            graphics.DrawString("Figure: Rectangle", font, Brushes.Red, 10, 50);

            // Сохраняем
            bitmap.Save(outputPath, ImageFormat.Png);
            Console.WriteLine($"Визуализация сохранена: {outputPath}");
        }
    }

    static void Main()
    {
        Console.WriteLine("=== Point Figure Analyzer ===");

        // Указываем полный путь к файлу
        string inputFile = @"C:\Users\Артем\source\repos\PointFigureAnalyzer\points.txt";

        // Чтение точек из файла
        var points = ReadPointsFromFile(inputFile);

        if (points.Count > 0)
        {
            // Создание визуализации
            string outputFile = @"C:\Users\Артем\source\repos\PointFigureAnalyzer\figure_output.png";
            CreateVisualization(points, outputFile);

            // Анализ в консоли
            var minX = points.Min(p => p.X);
            var maxX = points.Max(p => p.X);
            var minY = points.Min(p => p.Y);
            var maxY = points.Max(p => p.Y);

            Console.WriteLine($"\n=== Результаты анализа ===");
            Console.WriteLine($"Границы: X({minX}, {maxX}), Y({minY}, {maxY})");
            Console.WriteLine("Фигура: rectangle");

            // Открываем изображение (только на Windows)
            if (OperatingSystem.IsWindows())
            {
                System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                {
                    FileName = outputFile,
                    UseShellExecute = true
                });
            }
        }
        else
        {
            Console.WriteLine("Не удалось прочитать точки из файла.");
        }

        Console.WriteLine("\nНажмите любую клавишу для выхода...");
        Console.WriteLine("файл создан");

    }
}