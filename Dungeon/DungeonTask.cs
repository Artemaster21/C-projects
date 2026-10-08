using System;
using System.Collections.Generic;
using System.Linq;

namespace Dungeon
{
    public class DungeonTask
    {
        /// <summary>
        /// Главный метод: находит кратчайший путь через сундук
        /// </summary>
        public static MoveDirection[] FindShortestPath(Map map)
        {
            // ШАГ 1: Находим пути от старта до всех сундуков
            // pathsFromStart — словарь: точка сундука → путь от старта
            var pathsFromStart = GetPathsToChests(map, map.InitialPosition);

            // ШАГ 2: Находим пути от всех сундуков до выхода
            // pathsToExit — словарь: точка сундука → путь до выхода
            var pathsToExit = GetPathsToChests(map, map.Exit);

            // ШАГ 3: Ищем лучший путь через сундук
            var bestPath = FindBestPathThroughChests(map.Chests, pathsFromStart, pathsToExit);
            if (bestPath != null)
                return bestPath;

            // ШАГ 4: Если нет пути через сундук — ищем прямой путь
            var directPath = GetDirectPath(map);
            if (directPath != null)
                return directPath;

            // ШАГ 5: Если пути нет — возвращаем пустой массив
            return Array.Empty<MoveDirection>();
        }

        /// <summary>
        /// Находит пути от заданной точки до всех сундуков
        /// </summary>
        /// <param name="map">Карта</param>
        /// <param name="start">Начальная точка (старт или выход)</param>
        /// <returns>Словарь: координаты сундука → путь к нему</returns>
        private static Dictionary<Point, SinglyLinkedList<Point>> GetPathsToChests(Map map, Point start)
        {
            // BfsTask.FindPaths возвращает все пути до сундуков
            // ToDictionary превращает их в словарь для быстрого доступа
            return BfsTask.FindPaths(map, start, map.Chests)
                .ToDictionary(path => path.Value, path => path);
        }

        /// <summary>
        /// Находит лучший путь, проходящий через сундук
        /// </summary>
        private static MoveDirection[] FindBestPathThroughChests(
            Chest[] chests,
            Dictionary<Point, SinglyLinkedList<Point>> pathsFromStart,
            Dictionary<Point, SinglyLinkedList<Point>> pathsToExit)
        {
            MoveDirection[] bestPath = null;
            int bestLength = int.MaxValue;      // минимальная длина
            byte bestWeight = 0;                // максимальный вес

            foreach (var chest in chests)
            {
                // Получаем путь от старта до сундука
                var startToChest = pathsFromStart.GetValueOrDefault(chest.Location);
                // Получаем путь от сундука до выхода
                var chestToExit = pathsToExit.GetValueOrDefault(chest.Location);

                // Если хотя бы одного пути нет — пропускаем
                if (startToChest == null || chestToExit == null)
                    continue;

                // Длина общего пути = длина(старт→сундук) + длина(сундук→выход) - 1
                // Вычитаем 1, чтобы не считать сундук дважды
                int length = startToChest.Length + chestToExit.Length - 1;
                byte weight = chest.Value;

                // Сравниваем с лучшим найденным
                if (IsBetterPath(length, weight, bestLength, bestWeight))
                {
                    bestLength = length;
                    bestWeight = weight;
                    bestPath = CombinePaths(startToChest, chestToExit);
                }
            }

            return bestPath;
        }

        /// <summary>
        /// Сравнивает два пути: сначала по длине, потом по весу сундука
        /// </summary>
        private static bool IsBetterPath(int length, byte weight, int bestLength, byte bestWeight)
        {
            // Путь лучше, если:
            // 1. Он короче, или
            // 2. Он такой же длины, но сундук тяжелее
            return length < bestLength || (length == bestLength && weight > bestWeight);
        }

        /// <summary>
        /// Соединяет два пути: от старта до сундука и от сундука до выхода
        /// </summary>
        private static MoveDirection[] CombinePaths(
            SinglyLinkedList<Point> startToChest,
            SinglyLinkedList<Point> chestToExit)
        {
            // startToChest хранит путь в обратном порядке (от сундука к старту)
            // Разворачиваем, чтобы получить путь от старта к сундуку
            var points = startToChest.Reverse().ToList();

            // chestToExit хранит путь от выхода к сундуку (тоже в обратном порядке)
            // Пропускаем первый элемент (сундук), чтобы не дублировать
            // Добавляем остальные точки (они уже в правильном порядке)
            points.AddRange(chestToExit.Skip(1));

            // Превращаем точки в направления
            return ConvertToDirections(points);
        }

        /// <summary>
        /// Находит прямой путь от старта до выхода (без сундуков)
        /// </summary>
        private static MoveDirection[] GetDirectPath(Map map)
        {
            // Создаём фиктивный сундук на выходе с весом 0
            // BfsTask найдет путь от старта до этой точки
            var directPath = BfsTask.FindPaths(map, map.InitialPosition, new[] { new EmptyChest(map.Exit) })
                .FirstOrDefault();

            if (directPath == null)
                return null;

            // Путь хранится в обратном порядке (от выхода к старту)
            // Разворачиваем и превращаем в направления
            return ConvertToDirections(directPath.Reverse().ToList());
        }

        /// <summary>
        /// Превращает список точек в массив направлений
        /// </summary>
        /// <example>
        /// Точки: (0,0) → (1,0) → (1,1)
        /// Результат: [Right, Down]
        /// </example>
        private static MoveDirection[] ConvertToDirections(List<Point> points)
        {
            if (points.Count < 2)
                return Array.Empty<MoveDirection>();

            var directions = new List<MoveDirection>();

            for (int i = 0; i < points.Count - 1; i++)
            {
                var from = points[i];
                var to = points[i + 1];

                // Определяем направление по разнице координат
                if (to.X > from.X)
                    directions.Add(MoveDirection.Right);
                else if (to.X < from.X)
                    directions.Add(MoveDirection.Left);
                else if (to.Y > from.Y)
                    directions.Add(MoveDirection.Down);
                else if (to.Y < from.Y)
                    directions.Add(MoveDirection.Up);
            }

            return directions.ToArray();
        }
    }
}