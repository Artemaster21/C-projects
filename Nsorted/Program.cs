using System;
using System.Collections.Generic;

public class RangeQuerySimple
{
    // Храним: значение -> список его индексов
    private Dictionary<int, List<int>> valuePositions;

    public RangeQuerySimple(int[] array)
    {
        // Метод 1: Создаем пары (значение, индекс) и группируем
        valuePositions = new Dictionary<int, List<int>>();

        // Проходим по всему массиву
        for (var i = 0; i < array.Length; i++)
        {
            // Если это значение встречается впервые - создаем список
            if (!valuePositions.ContainsKey(array[i]))
                valuePositions[array[i]] = new List<int>();

            // Добавляем текущий индекс в список для этого значения
            valuePositions[array[i]].Add(i);
        }

        // Сортируем индексы для каждого значения (для бинарного поиска)
        foreach (var kvp in valuePositions)
            kvp.Value.Sort();
    }

    // Метод 2: Ищем все индексы значения X и проверяем диапазон
    public bool ContainsInRange(int L, int R, int X)
    {
        // 1. Проверяем, есть ли такое значение вообще
        if (!valuePositions.ContainsKey(X))
            return false;

        // 2. Получаем отсортированный список индексов
        List<int> indices = valuePositions[X];

        // 3. Ищем первый индекс, который >= L
        // Используем бинарный поиск вручную
        var left = 0;
        var right = indices.Count - 1;
        var resultIndex = -1; // -1 значит "не нашли"

        while (left <= right)
        {
            var middle = (left + right) / 2;
            var currentIndex = indices[middle];

            if (currentIndex >= L)
            {
                // Нашли подходящий, но может быть еще левее
                resultIndex = currentIndex;
                right = middle - 1; // Продолжаем искать левее
            }
            else
                // Текущий индекс слишком мал, идем вправо
                left = middle + 1;
        }

        // 4. Проверяем, попадает ли найденный индекс в диапазон [L, R]
        return resultIndex != -1 && resultIndex <= R;
    }

    // Альтернативный метод: простой линейный поиск
    public bool ContainsInRangeSimple(int L, int R, int X)
    {
        if (!valuePositions.ContainsKey(X))
            return false;

        List<int> indices = valuePositions[X];

        // Просто перебираем все индексы
        for (var i = 0; i < indices.Count; i++)
        {
            var currentIndex = indices[i];

            // Если индекс попадает в диапазон
            if (currentIndex >= L && currentIndex <= R)
                return true; // Нашли!

            // Если индекс уже больше R, дальше можно не искать
            // (т.к. индексы отсортированы)
            if (currentIndex > R)
                break;
        }

        return false;
    }
}
public class Program
{
    public static void Main()
    {
        // Тестовый массив
        int[] numbers = { 5, 2, 8, 2, 9, 1, 5, 2, 7 };
        int[] numbers2 = { 1, 1, 2, 1, 1, 2 };
        RangeQuerySimple sol= new RangeQuerySimple(numbers2);
        RangeQuerySimple solver = new RangeQuerySimple(numbers);

        // Тест 1: Значение 2 в диапазоне [3, 6]
        Console.WriteLine("Тест 1: Есть ли 2 в [3, 6]?");
        bool result1 = solver.ContainsInRange(3, 6, 2);
        Console.WriteLine($"Бинарный поиск: {result1}");
        Console.WriteLine($"Простой поиск: {solver.ContainsInRangeSimple(3, 6, 2)}");

        // Тест 2: Значение 5 в диапазоне [0, 3]
        Console.WriteLine("\nТест 2: Есть ли 5 в [0, 3]?");
        bool result2 = solver.ContainsInRange(0, 3, 5);
        Console.WriteLine($"Бинарный поиск: {result2}");
        Console.WriteLine($"Простой поиск: {solver.ContainsInRangeSimple(0, 3, 5)}");

        // Тест 3: Значение 9 в диапазоне [5, 7]
        Console.WriteLine("\nТест 3: Есть ли 9 в [5, 7]?");
        bool result3 = solver.ContainsInRange(5, 7, 9);
        Console.WriteLine($"Бинарный поиск: {result3}");
        Console.WriteLine($"Простой поиск: {solver.ContainsInRangeSimple(5, 7, 9)}");

        // Тест 4: Значение 10 (которого нет)
        Console.WriteLine("\nТест 4: Есть ли 10 в [0, 8]?");
        bool result4 = solver.ContainsInRange(0, 8, 10);
        Console.WriteLine($"Бинарный поиск: {result4}");
        Console.WriteLine($"Простой поиск: {solver.ContainsInRangeSimple(0, 8, 10)}");

        //bool Contains(int[] arr, int L, int R, int X)
        //{
        //    for (int i = L; i <= R; i++)  // O(N) на запрос
        //        if (arr[i] == X)
        //            return true;
        //    return false;
        //}
        //Console.WriteLine(Contains([1, 1, 2, 1, 1, 2], 3, 4, 2));
    }
}