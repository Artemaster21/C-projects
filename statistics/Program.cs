using System;
using System.Linq;

class Program
{
    static void Main()
    {
        //Console.WriteLine();
        Console.WriteLine((((32) + 49) - (36 - 69 * 61) + (71 / 92 - (95+12) * 83) / 63) % 64 - ((85 + 14 + 49) + ((49) + 61) - (19 + 98 / 7) + (17 - (6) % 92 + (9*73))));
        Console.WriteLine("Введите числа через пробел:");
        string input = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(input))
        {
            Console.WriteLine("Нет введённых чисел.");
            return;
        }

        int[] numbers;
        try
        {
            numbers = input.Split(' ', StringSplitOptions.RemoveEmptyEntries)
                           .Select(int.Parse)
                           .ToArray();
        }
        catch
        {
            Console.WriteLine("Ошибка при разборе чисел.");
            return;
        }

        Console.WriteLine("Выберите действие: sum , min , max");
        string command = Console.ReadLine()?.ToLower();

        switch (command)
        {
            case "sum":
                Console.WriteLine("Сумма чисел: " + numbers.Sum());
                break;
            case "min":
                Console.WriteLine("Минимальное число: " + numbers.Min());
                break;
            case "max":
                Console.WriteLine("Максимальное число: " + numbers.Max());
                break;
            default:
                Console.WriteLine("Неверная команда.");
                break;
        }
    }
}