using System;
using System.Collections.Generic;

namespace Antiplagiarism;

public static class LongestCommonSubsequenceCalculator
{
    public static List<string> Calculate(List<string> first, List<string> second)
    {
        var opt = CreateOptimizationTable(first, second);
        return RestoreAnswer(opt, first, second);
    }

    private static int[,] CreateOptimizationTable(List<string> first, List<string> second)
    {
        var n = first.Count;
        var m = second.Count;
        var opt = new int[n + 1, m + 1];

        // Граничные условия: opt[0, j] = 0 и opt[i, 0] = 0 уже по умолчанию

        for (int i = 1; i <= n; i++)
        {
            for (int j = 1; j <= m; j++)
            {
                if (first[i - 1] == second[j - 1])
                {
                    // Токены совпадают — увеличиваем длину НОП
                    opt[i, j] = opt[i - 1, j - 1] + 1;
                }
                else
                {
                    // Берём максимум из двух вариантов
                    opt[i, j] = Math.Max(opt[i - 1, j], opt[i, j - 1]);
                }
            }
        }

        return opt;
    }

    private static List<string> RestoreAnswer(int[,] opt, List<string> first, List<string> second)
    {
        var result = new List<string>();
        var i = first.Count;
        var j = second.Count;

        // Идём с конца таблицы к началу
        while (i > 0 && j > 0)
        {
            if (first[i - 1] == second[j - 1])
            {
                // Токены совпадают — добавляем в результат
                result.Add(first[i - 1]);
                i--;
                j--;
            }
            else if (opt[i - 1, j] > opt[i, j - 1])
            {
                // Идём вверх (уменьшаем i)
                i--;
            }
            else
            {
                // Идём влево (уменьшаем j)
                j--;
            }
        }

        // Так как мы шли с конца, результат получился в обратном порядке
        result.Reverse();
        return result;
    }
}