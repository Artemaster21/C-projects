namespace Delegates.PairsAnalysis;

public static class Analysis
{
    public static IEnumerable<Tuple<T, T>> Pairs<T>(this IEnumerable<T> data)
    {
        T last = default;
        bool first = true;

        foreach (var current in data)
        {
            if (first)
            {
                last = current;
                first = false;
                continue;
            }

            yield return Tuple.Create(last, current);
            last = current;
        }
    }

    public static int MaxIndex<T>(this IEnumerable<T> data) where T : IComparable<T>
    {
        var maxValue = default(T);
        var maxIndex = 0;
        var index = 0;
        var found = false;

        foreach (var current in data)
        {
            if (!found)
            {
                maxValue = current;
                maxIndex = 0;
                found = true;
            }
            else if (current.CompareTo(maxValue) > 0)
            {
                maxValue = current;
                maxIndex = index;
            }
            index++;
        }

        if (!found)
            throw new InvalidOperationException("Sequence contains no elements");

        return maxIndex;
    }

    public static int FindMaxPeriodIndex(params DateTime[] data)
    {
        if (data.Length < 2)
            throw new InvalidOperationException();

        return data
            .Pairs()
            .Select(pair => (pair.Item2 - pair.Item1).TotalSeconds)
            .MaxIndex();
    }

    public static double FindAverageRelativeDifference(params double[] data)
    {
        if (data.Length < 2)
            throw new InvalidOperationException();

        return data
            .Pairs()
            .Average(pair => (pair.Item2 - pair.Item1) / pair.Item1);
    }
}