using System;
using System.Collections.Generic;

namespace yield
{
    public static class MovingAverageTask
    {
        public static IEnumerable<DataPoint> MovingAverage(
            this IEnumerable<DataPoint> data, int windowWidth)
        {
            // Проверка входных параметров
            if (data == null)
                throw new ArgumentNullException(nameof(data));

            if (windowWidth <= 0)
                throw new ArgumentOutOfRangeException(nameof(windowWidth),
                    "Window width must be positive");

            // Очередь для хранения элементов текущего окна
            var window = new Queue<double>();

            // Сумма элементов в окне (для быстрого вычисления среднего)
            double sum = 0;

            // Перебираем все точки входной последовательности
            foreach (var point in data)
            {
                // 1. Добавляем текущее значение в окно
                window.Enqueue(point.OriginalY);
                sum += point.OriginalY;

                // 2. Если окно переполнено - удаляем самый старый элемент
                if (window.Count > windowWidth)
                    sum -= window.Dequeue();

                // 3. Вычисляем среднее по текущему окну
                var average = sum / window.Count;

                // 4. Возвращаем точку с добавленным значением скользящего среднего
                yield return point.WithAvgSmoothedY(average);
            }
        }
    }
}