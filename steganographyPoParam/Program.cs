using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

class Program
{
    static void Main()
    {
        string input = Console.ReadLine();
        string[] vvod = input.Split('\n');
        string coordinates = Console.ReadLine();
        string text = Console.ReadLine();

        // 1. Находим все пары типа (x,y)
        MatchCollection matches = Regex.Matches(coordinates, @"\d+");
        List<int> numbers = matches.Cast<Match>().Select(m => int.Parse(m.Value)).ToList();

        List<int> nums = new List<int>(numbers); // копия
        List<int> result = new List<int>();

        // старт: число перед 0
        int i = nums.IndexOf(0);
        int cur = nums[i - 1];
        result.Add(cur);
        nums.RemoveAt(i);     // удалить 0
        nums.RemoveAt(i - 1); // удалить cur

        // пока cur встречается — продолжаем цепочку
        while (nums.Contains(cur))
        {
            i = nums.IndexOf(cur);
            cur = nums[i - 1]; // число перед найденным
            result.Add(cur);
            nums.RemoveAt(i);     // удалить cur
            nums.RemoveAt(i - 1); // удалить предыдущий
        }

        List<char> ans = new List<char>();
        for (int j = 0; j < result.Count; j++)
        {
            ans.Add(text[result[j] - 1]);
        }

        string ansS = new string(ans.ToArray());
        Console.WriteLine(ansS);
    }
}