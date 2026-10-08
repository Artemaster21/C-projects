using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class NotGreedyPathFinder : IPathFinder
{
    public List<Point> FindPathToCompleteGoal(State state)
    {
        if (state.Chests.Count == 0) return new List<Point>();

        var pathFinder = new DijkstraPathFinder();
        var chests = state.Chests.ToList();

        var startPaths = pathFinder.GetPathsByDijkstra(state, state.Position, chests)
            .ToDictionary(p => p.End, p => p);

        var chestPaths = new Dictionary<(Point, Point), PathWithCost>();
        for (var i = 0; i < chests.Count; i++)
            for (var j = 0; j < chests.Count; j++)
                if (i != j)
                    chestPaths[(chests[i], chests[j])] = pathFinder
                        .GetPathsByDijkstra(state, chests[i], new[] { chests[j] }).First();

        var best = (count: 0, order: new List<Point>());
        var used = new bool[chests.Count];

        void Search(List<Point> order, int count, Point last, int energy)
        {
            if (count > best.count)
                best = (count, new List<Point>(order));

            for (var i = 0; i < chests.Count; i++)
            {
                if (used[i]) continue;

                var path = count == 0 ? startPaths[chests[i]] : chestPaths[(last, chests[i])];
                if (energy < path.Cost) continue;

                used[i] = true;
                order.Add(chests[i]);
                Search(order, count + 1, chests[i], energy - path.Cost);
                order.RemoveAt(order.Count - 1);
                used[i] = false;
            }
        }

        Search(new List<Point>(), 0, state.Position, state.InitialEnergy);

        if (best.order.Count == 0) return new List<Point>();

        var fullPath = new List<Point>();
        var current = state.Position;
        for (int i = 0; i < best.order.Count; i++)
        {
            var segment = i == 0
                ? startPaths[best.order[i]].Path
                : chestPaths[(best.order[i - 1], best.order[i])].Path;
            fullPath.AddRange(segment.Skip(1));
        }
        return fullPath;
    }
}