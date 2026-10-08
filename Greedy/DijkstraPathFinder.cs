using System;
using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class DijkstraPathFinder
{
    public IEnumerable<PathWithCost> GetPathsByDijkstra(State state, Point start, IEnumerable<Point> targets)
    {
        var targetsSet = new HashSet<Point>(targets);
        if (targetsSet.Count == 0)
            yield break;

        var distances = new Dictionary<Point, int>();
        var previous = new Dictionary<Point, Point>();
        var visited = new HashSet<Point>();
        var queue = new PriorityQueue<Point, int>();

        distances[start] = 0;
        queue.Enqueue(start, 0);

        var foundCount = 0;

        while (queue.Count > 0 && foundCount < targetsSet.Count)
        {
            var current = queue.Dequeue();
            var currentDist = distances[current];

            if (visited.Contains(current))
                continue;

            visited.Add(current);

            if (targetsSet.Contains(current))
            {
                foundCount++;

                var path = new List<Point>();
                var node = current;
                while (node != start)
                {
                    path.Add(node);
                    node = previous[node];
                }

                yield return new PathWithCost(currentDist, path.ToArray());
                continue;
            }

            var directions = new[]
            {
                new Point(0, -1), new Point(0, 1),
                new Point(-1, 0), new Point(1, 0)
            };

            foreach (var dir in directions)
            {
                var neighbor = new Point(current.X + dir.X, current.Y + dir.Y);

                if (!state.InsideMap(neighbor))
                    continue;

                if (state.CellCost[neighbor.X, neighbor.Y] == 0)
                    continue;

                var newDist = currentDist + state.CellCost[neighbor.X, neighbor.Y];

                if (!distances.ContainsKey(neighbor) || newDist < distances[neighbor])
                {
                    distances[neighbor] = newDist;
                    previous[neighbor] = current;
                    queue.Enqueue(neighbor, newDist);
                }
            }
        }
    }
}