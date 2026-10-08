using System.Collections.Generic;
using System.Linq;

namespace Dungeon
{
    public class BfsTask
    {
        /// <summary>
        /// Находит пути от стартовой точки до всех сундуков
        /// </summary>
        /// <param name="map">Карта лабиринта</param>
        /// <param name="start">Начальная точка (старт или выход)</param>
        /// <param name="chests">Массив сундуков</param>
        /// <returns>Последовательность путей до каждого достижимого сундука</returns>
        public static IEnumerable<SinglyLinkedList<Point>> FindPaths(Map map, Point start, Chest[] chests)
        {
            // Быстрый доступ к координатам сундуков (O(1) проверка)
            var chestPoints = chests.Select(c => c.Location).ToHashSet();

            // Посещённые точки (чтобы не ходить по кругу)
            var visited = new HashSet<Point>();

            // Очередь для BFS: хранит пути (списки точек)
            var queue = new Queue<SinglyLinkedList<Point>>();

            // Начинаем с точки старта
            var startNode = new SinglyLinkedList<Point>(start);
            queue.Enqueue(startNode);
            visited.Add(start);

            // Пока есть что обрабатывать
            while (queue.Count > 0)
            {
                // Берём первый путь из очереди
                var currentPath = queue.Dequeue();
                var currentPoint = currentPath.Value;

                // Если в этой клетке есть сундук — возвращаем путь
                if (chestPoints.Contains(currentPoint))
                    yield return currentPath;

                // Проверяем всех соседей (4 направления)
                foreach (var neighbor in GetNeighbors(map, currentPoint))
                {
                    // Если сосед ещё не посещён и не является стеной
                    if (!visited.Contains(neighbor) && !IsWall(map, neighbor))
                    {
                        visited.Add(neighbor);
                        var newPath = new SinglyLinkedList<Point>(neighbor, currentPath);
                        queue.Enqueue(newPath);
                    }
                }
            }
        }

        /// <summary>
        /// Возвращает всех допустимых соседей для точки
        /// </summary>
        private static IEnumerable<Point> GetNeighbors(Map map, Point point)
        {
            // Четыре направления: вверх, вниз, влево, вправо
            var directions = new[]
            {
                new Point { X = 0, Y = -1 }, // вверх
                new Point { X = 0, Y = 1 },  // вниз
                new Point { X = -1, Y = 0 }, // влево
                new Point { X = 1, Y = 0 }   // вправо
            };

            foreach (var dir in directions)
            {
                var neighbor = new Point
                {
                    X = point.X + dir.X,
                    Y = point.Y + dir.Y
                };

                // Проверяем, что сосед в пределах карты
                if (map.InBounds(neighbor))
                    yield return neighbor;
            }
        }

        /// <summary>
        /// Проверяет, является ли клетка стеной
        /// </summary>
        private static bool IsWall(Map map, Point point)
        {
            return map.Dungeon[point.X, point.Y] == MapCell.Wall;
        }
    }
}