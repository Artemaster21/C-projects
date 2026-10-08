using System;
using System.Collections.Generic;
using System.Linq;

namespace linq_slideviews;

public static class ExtensionsTask
{
    /// <summary>
    /// Медиана списка из нечетного количества элементов — это серединный элемент списка после сортировки.
    /// Медиана списка из четного количества элементов — это среднее арифметическое 
    /// двух серединных элементов списка после сортировки.
    /// </summary>
    /// <exception cref="InvalidOperationException">Если последовательность не содержит элементов</exception>
    public static double Median(this IEnumerable<double> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        // Важно: преобразуем в массив, чтобы перечислить только ОДИН раз
        double[] sortedArray = items.ToArray();

        if (sortedArray.Length == 0)
            throw new InvalidOperationException("Последовательность не содержит элементов");

        // Сортируем массив
        Array.Sort(sortedArray);

        int count = sortedArray.Length;
        int mid = count / 2;

        // Если нечетное количество элементов
        if (count % 2 == 1)
            return sortedArray[mid];

        // Если четное количество — среднее арифметическое двух центральных
        return (sortedArray[mid - 1] + sortedArray[mid]) / 2.0;
    }

    /// <returns>
    /// Возвращает последовательность, состоящую из пар соседних элементов.
    /// Например, по последовательности {1,2,3} метод должен вернуть две пары: (1,2) и (2,3).
    /// </returns>
    public static IEnumerable<(T First, T Second)> Bigrams<T>(this IEnumerable<T> items)
    {
        if (items == null)
            throw new ArgumentNullException(nameof(items));

        // Ленивая реализация с использованием foreach
        T? previous = default(T);
        bool isFirst = true;

        foreach (T current in items)
        {
            if (!isFirst)
                // Возвращаем пару (предыдущий, текущий)
                yield return (previous!, current);
            else
                isFirst = false;

            previous = current;
        }
    }
}