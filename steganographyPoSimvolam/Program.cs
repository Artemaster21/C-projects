namespace steganographyPoSimvolam
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;

    class Program
    {
        // Console.WriteLine();


        static readonly Dictionary<string, int> Elements = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            {"H", 1}, {"He", 2}, {"Li", 3}, {"Be", 4}, {"B", 5}, {"C", 6}, {"N", 7}, {"O", 8}, {"F", 9}, {"Ne", 10},
            {"Na", 11}, {"Mg", 12}, {"Al", 13}, {"Si", 14}, {"P", 15}, {"S", 16}, {"Cl", 17}, {"Ar", 18}, {"K", 19}, {"Ca", 20},
            {"Sc", 21}, {"Ti", 22}, {"V", 23}, {"Cr", 24}, {"Mn", 25}, {"Fe", 26}, {"Co", 27}, {"Ni", 28}, {"Cu", 29}, {"Zn", 30},
            {"Ga", 31}, {"Ge", 32}, {"As", 33}, {"Se", 34}, {"Br", 35}, {"Kr", 36}, {"Rb", 37}, {"Sr", 38}, {"Y", 39}, {"Zr", 40},
            {"Nb", 41}, {"Mo", 42}, {"Tc", 43}, {"Ru", 44}, {"Rh", 45}, {"Pd", 46}, {"Ag", 47}, {"Cd", 48}, {"In", 49}, {"Sn", 50},
            {"Sb", 51}, {"Te", 52}, {"I", 53}, {"Xe", 54}, {"Cs", 55}, {"Ba", 56}, {"La", 57}, {"Ce", 58}, {"Pr", 59}, {"Nd", 60},
            {"Pm", 61}, {"Sm", 62}, {"Eu", 63}, {"Gd", 64}, {"Tb", 65}, {"Dy", 66}, {"Ho", 67}, {"Er", 68}, {"Tm", 69}, {"Yb", 70},
            {"Lu", 71}, {"Hf", 72}, {"Ta", 73}, {"W", 74}, {"Re", 75}, {"Os", 76}, {"Ir", 77}, {"Pt", 78}, {"Au", 79}, {"Hg", 80},
            {"Tl", 81}, {"Pb", 82}, {"Bi", 83}, {"Po", 84}, {"At", 85}, {"Rn", 86}, {"Fr", 87}, {"Ra", 88}, {"Ac", 89}, {"Th", 90},
            {"Pa", 91}, {"U", 92}, {"Np", 93}, {"Pu", 94}, {"Am", 95}, {"Cm", 96}, {"Bk", 97}, {"Cf", 98}, {"Es", 99}, {"Fm", 100},
            {"Md", 101}, {"No", 102}, {"Lr", 103}, {"Rf", 104}, {"Db", 105}, {"Sg", 106}, {"Bh", 107}, {"Hs", 108}, {"Mt", 109}, {"Ds", 110},
            {"Rg", 111}, {"Cn", 112}, {"Nh", 113}, {"Fl", 114}, {"Mc", 115}, {"Lv", 116}, {"Ts", 117}, {"Og", 118}
        };

        static void Main()
        {
            Console.WriteLine("Строка из кроссворда");
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            var nonogramString = Console.ReadLine();
            Console.WriteLine("Преобразованная строка нонограммы:");

            string[] rows = nonogramString.Split(';', StringSplitOptions.RemoveEmptyEntries);
            StringBuilder transformedString = new StringBuilder();

            for (int i = 0; i < rows.Length; i++)
            {
                string row = rows[i];
                string transformedRow = row.Replace('.', '0').Replace('#', '1');
                transformedString.Append(transformedRow);
                if (i < rows.Length - 1)
                {
                    transformedString.Append(';');
                }
            }

            Console.WriteLine(transformedString.ToString());

            // Преобразуем строки в матрицу символов
            char[,] matrix = new char[rows.Length, rows[0].Length];
            for (int i = 0; i < rows.Length; i++)
            {
                for (int j = 0; j < rows[i].Length; j++)
                {
                    matrix[i, j] = rows[i][j];
                }
            }

            int rowCount = rows.Length;
            int colCount = rows[0].Length;

            List<int> bottomToTopNumbers = new List<int>();

            for (int col = 0; col < colCount; col++)
            {
                StringBuilder binaryStr = new StringBuilder();
                for (int row = rowCount - 1; row >= 0; row--)
                {
                    binaryStr.Append(matrix[row, col] == '#' ? '1' : '0');
                }

                string binary = binaryStr.ToString();
                int decimalNumber = Convert.ToInt32(binary, 2);
                bottomToTopNumbers.Add(decimalNumber);
            }

            Console.WriteLine("\nДесятичные числа (снизу вверх) через пробел:");
            Console.WriteLine(string.Join(" ", bottomToTopNumbers));

            List<int> topToBottomNumbers = new List<int>();

            for (int col = 0; col < colCount; col++)
            {
                StringBuilder binaryStr = new StringBuilder();
                for (int row = 0; row < rowCount; row++)
                {
                    binaryStr.Append(matrix[row, col] == '#' ? '1' : '0');
                }

                string binary = binaryStr.ToString();
                int decimalNumber = Convert.ToInt32(binary, 2);
                topToBottomNumbers.Add(decimalNumber);

            }

            Console.WriteLine("\nДесятичные числа (снизу вверх) через пробел:");
            Console.WriteLine(string.Join(" ", topToBottomNumbers));

            Console.WriteLine("Введите индексы или элементы через пробел (например: 1 5 [Na] [C] 10):");
            var indexesInput = Console.ReadLine();
            string[] indexesStr = indexesInput.Split(' ', StringSplitOptions.RemoveEmptyEntries);

            int[] indexes = new int[indexesStr.Length];
            for (var i = 0; i < indexesStr.Length; i++)
            {
                var token = indexesStr[i].Trim();

                if (token.StartsWith("[") && token.EndsWith("]"))
                {
                    var symbol = token.Substring(1, token.Length - 2);
                    if (Elements.TryGetValue(symbol, out int atomicNumber))
                        indexes[i] = atomicNumber;
                    else
                    {
                        Console.WriteLine($"Ошибка: элемент '{symbol}' не найден в таблице элементов.");
                        return;
                    }
                }
                else if (int.TryParse(token, NumberStyles.Integer, CultureInfo.InvariantCulture, out int num))
                    indexes[i] = num;
                else
                {
                    Console.WriteLine($"Ошибка: '{token}' не является числом или химическим элементом.");
                    return;
                }
            }

            Console.WriteLine("Введите текст:");
            var text = Console.ReadLine();

            char[] messageChars = new char[indexes.Length];
            for (var i = 0; i < indexes.Length; i++)
            {
                var idx = indexes[i] - 1;
                if (idx < 0 || idx >= text.Length)
                {
                    Console.WriteLine($"Ошибка: индекс {indexes[i]} вне диапазона текста (длина {text.Length}).");
                    return;
                }
                messageChars[i] = text[idx];
            }

            var message = new string(messageChars);
            Console.WriteLine("Скрытое сообщение:");
            Console.WriteLine(message);
        }
    }
}
