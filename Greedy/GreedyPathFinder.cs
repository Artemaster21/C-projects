using System.Collections.Generic;
using System.Linq;
using Greedy.Architecture;

namespace Greedy;

public class GreedyPathFinder : IPathFinder
{
    public List<Point> FindPathToCompleteGoal(State state)
    {
        if (state.Goal == 0)
            return new List<Point>();

        if (state.Chests.Count < state.Goal)
            return new List<Point>();

        var fullPath = new List<Point>();
        var currentPos = state.Position;
        var remainingChests = new HashSet<Point>(state.Chests);
        var energy = state.InitialEnergy;

        for (int i = 0; i < state.Goal; i++)
        {
            var paths = new DijkstraPathFinder().GetPathsByDijkstra(state, currentPos, remainingChests);

            var bestPath = paths.FirstOrDefault();

            if (bestPath == null || energy < bestPath.Cost)
                return new List<Point>();

            fullPath.AddRange(bestPath.Path.Skip(1));

            currentPos = bestPath.End;
            energy -= bestPath.Cost;
            remainingChests.Remove(bestPath.End);
        }

        return fullPath;
    }
}