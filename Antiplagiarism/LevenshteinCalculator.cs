using System;
using System.Collections.Generic;
using System.Linq;

// Каждый документ — это список токенов. То есть List<string>.
// Вместо этого будем использовать псевдоним DocumentTokens.
// Это поможет избежать сложных конструкций:
// вместо List<List<string>> будет List<DocumentTokens>
using DocumentTokens = System.Collections.Generic.List<string>;

namespace Antiplagiarism;

public class LevenshteinCalculator
{
    public List<ComparisonResult> CompareDocumentsPairwise(List<DocumentTokens> documents)
    {
        var result = new List<ComparisonResult>();

        // Перебираем все пары документов (i < j)
        for (int i = 0; i < documents.Count; i++)
        {
            for (int j = i + 1; j < documents.Count; j++)
            {
                var distance = GetLevenshteinDistance(documents[i], documents[j]);
                result.Add(new ComparisonResult(documents[i], documents[j], distance));
            }
        }

        return result;
    }

    private double GetLevenshteinDistance(DocumentTokens first, DocumentTokens second)
    {
        // Оптимизация памяти: храним только две строки
        var prev = new double[second.Count + 1];
        var curr = new double[second.Count + 1];

        // Инициализация первой строки (преобразование пустого документа в second)
        for (int j = 0; j <= second.Count; j++)
            prev[j] = j;  // стоимость вставки j токенов

        for (int i = 1; i <= first.Count; i++)
        {
            curr[0] = i;  // стоимость удаления i токенов

            for (int j = 1; j <= second.Count; j++)
            {
                // Стоимость замены токена first[i-1] на second[j-1]
                double replacementCost = TokenDistanceCalculator.GetTokenDistance(first[i - 1], second[j - 1]);

                // Три варианта:
                // 1. Удаление токена из первой строки
                // 2. Вставка токена во вторую строку
                // 3. Замена токена (или совпадение, если replacementCost == 0)
                curr[j] = Math.Min(
                    Math.Min(prev[j] + 1,           // удаление
                             curr[j - 1] + 1),      // вставка
                    prev[j - 1] + replacementCost); // замена/совпадение
            }

            // Меняем местами строки для следующей итерации
            (prev, curr) = (curr, prev);
        }

        return prev[second.Count];
    }
}