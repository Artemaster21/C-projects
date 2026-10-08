using System;
using System.Linq;
using Avalonia;
using Avalonia.ReactiveUI;
using Greedy.Architecture;

namespace Greedy.UI;

internal static class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        // Запускаем консольный тест
        RunConsoleTest();

        // Для UI раскомментируйте:
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
    }

    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI();

    private static void RunConsoleTest()
    {
        Console.WriteLine("=== Тест DijkstraPathFinder ===\n");

        Console.WriteLine("--- Тест Go Left ---");
        TestGoLeft();
    }

    private static void TestGoLeft()
    {
        // Используем Greedy.Architecture.Point
        var cellCost = new int[2, 1];
        cellCost[0, 0] = 1;
        cellCost[1, 0] = 1;

        var start = new Greedy.Architecture.Point(0, 0);
        var chests = new[] { new Greedy.Architecture.Point(1, 0) };

        var state = new State("GoLeft", 100, start, cellCost, 1, chests);

        var finder = new DijkstraPathFinder();
        var paths = finder.GetPathsByDijkstra(state, start, chests);

        var firstPath = paths.FirstOrDefault();
        if (firstPath != null)
        {
            Console.WriteLine($"  Cost: {firstPath.Cost} (ожидалось: 1)");
            Console.WriteLine($"  Path: [{string.Join(", ", firstPath.Path.Select(p => $"({p.X},{p.Y})"))}]");
            Console.WriteLine($"  Результат: {(firstPath.Cost == 1 ? "✅ ПРОЙДЕН" : "❌ НЕ ПРОЙДЕН")}");
        }
        else
        {
            Console.WriteLine("  ❌ Путь не найден!");
        }
    }
}