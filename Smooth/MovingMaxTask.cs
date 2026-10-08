using System;
using System.Collections.Generic;
using System.Linq;

namespace yield
{
    public static class MovingMaxTask
    {
        public static IEnumerable<DataPoint> MovingMax(
            this IEnumerable<DataPoint> data, int windowWidth)
        {
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (windowWidth <= 0)
                throw new ArgumentOutOfRangeException(nameof(windowWidth),
                    "Window width must be positive");

            // Очередь для хранения элементов текущего окна
            var window = new Queue<double>();

            // Дек для хранения потенциальных максимумов (убывающий порядок)
            var potentialMax = new LinkedList<double>();

            foreach (var point in data)
            {
                double currentValue = point.OriginalY;

                // 1. Добавляем текущий элемент в окно
                window.Enqueue(currentValue);

                // 2. Обновляем список потенциальных максимумов
                // Удаляем все элементы в конце списка, которые меньше текущего
                while (potentialMax.Count > 0 && potentialMax.Last.Value < currentValue)
                    potentialMax.RemoveLast();
                // Добавляем текущий элемент в конец списка
                potentialMax.AddLast(currentValue);

                // 3. Если окно переполнено, удаляем самый старый элемент
                if (window.Count > windowWidth)
                {
                    double removedValue = window.Dequeue();

                    // Если удаляемый элемент был текущим максимумом, удаляем его из начала списка
                    if (potentialMax.Count > 0 && potentialMax.First.Value == removedValue)
                        potentialMax.RemoveFirst();
                }

                // 4. Текущий максимум - первый элемент в списке potentialMax

                // 5. Возвращаем точку с добавленным значением максимума
                yield return point.WithMaxY(potentialMax.First.Value);
            }
        }
    }
}