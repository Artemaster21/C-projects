using System;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public static class TestConsole
{
    public static void Run()
    {
        // Создаём state для теста Go Left
        var cellCost = new int[2, 1];
        cellCost[0, 0] = 1;
        cellCost[1, 0] = 1;

        var start = new Point(0, 0);
        var chests = new[] { new Point(1, 0) };

        var state = new State("test", 100, start, cellCost, 1, chests);

        var finder = new DijkstraPathFinder();
        var paths = finder.GetPathsByDijkstra(state, start, chests);

        var firstPath = paths.FirstOrDefault();
        if (firstPath != null)
        {
            Console.WriteLine($"Cost: {firstPath.Cost} (expected 1)");
            Console.WriteLine($"Path: {string.Join(" -> ", firstPath.Path)} (expected [(1,0)])");
        }
        else
        {
            Console.WriteLine("No path found!");
        }
    }
}