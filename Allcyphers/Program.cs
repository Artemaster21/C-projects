using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace UnifiedDecoder
{
    class Program
    {
        private const string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789' ";
        static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("=== Универсальный дешифратор ===");
            Console.WriteLine("Поддерживаемые форматы:");
            Console.WriteLine("  #Caesar's code=1#сообщение#");
            Console.WriteLine("  #Vigenere=ключ#сообщение#");
            Console.WriteLine("  #primeSypher=число#сообщение#");
            Console.WriteLine("  #reversed#сообщение#");
            Console.WriteLine("================================\n");

            while (true)
            {
                Console.WriteLine("Введите сообщение для расшифровки (или 'exit' для выхода):");
                string input = Console.ReadLine()?.Trim() ?? "";

                // Выход из программы
                if (string.IsNullOrEmpty(input) ||
                    input.Equals("exit", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("Завершение работы программы...");
                    break;
                }

                try
                {
                    // Проверяем формат ввода
                    if (!input.StartsWith("#"))
                    {
                        Console.WriteLine("Ошибка: сообщение должно начинаться с символа #");
                        continue;
                    }

                    // Разделяем входную строку по #
                    string[] parts = input.Split('#', StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Ошибка: неверный формат входных данных");
                        Console.WriteLine("Ожидается: #команда#сообщение#");
                        continue;
                    }

                    string command = parts[0];  // Команда с параметром (если есть)
                    string message = parts[1];  // Сообщение для расшифровки

                    string result = "";

                    if (command.Contains("Caesar's code"))
                    {
                        // Формат: Caesar's code=1
                        string[] commandParts = command.Split('=');
                        if (commandParts.Length == 2 && int.TryParse(commandParts[1], out int shift))
                            result = CaesarDecrypt(message, shift);
                        else
                        {
                            Console.WriteLine("Ошибка: неверный формат команды Цезаря");
                            Console.WriteLine("Ожидается: Caesar's code=число");
                            continue;
                        }
                    }
                    else if (command.Contains("Vigenere"))
                    {
                        // Формат: Vigenere=ключ
                        string[] commandParts = command.Split('=');
                        if (commandParts.Length == 2)
                        {
                            string key = commandParts[1];
                            result = VigenereDecrypt(message, key);
                        }
                        else
                        {
                            Console.WriteLine("Ошибка: неверный формат команды Виженера");
                            Console.WriteLine("Ожидается: Vigenere=ключ");
                            continue;
                        }
                    }
                    else if (command.Contains("primeSypher"))
                    {
                        // Формат: primeSypher=число
                        string[] commandParts = command.Split('=');
                        if (commandParts.Length == 2 && int.TryParse(commandParts[1], out int multiplicator))
                            result = PrimeDecrypt(message, multiplicator);
                        else
                        {
                            Console.WriteLine("Ошибка: неверный формат команды Prime");
                            Console.WriteLine("Ожидается: primeSypher=число");
                            continue;
                        }
                    }
                    else if (command == "reversed")
                    {
                        // Просто переворачиваем строку
                        char[] charArray = message.ToCharArray();
                        Array.Reverse(charArray);
                        result = new string(charArray);
                    }
                    else
                    {
                        Console.WriteLine($"Ошибка: неизвестная команда '{command}'");
                        continue;
                    }

                    Console.WriteLine("\n========================================");
                    Console.WriteLine("Результат расшифровки:");
                    Console.WriteLine($"Входные данные: {input}");
                    Console.WriteLine($"Расшифрованный текст: {result}");
                    Console.WriteLine("========================================\n");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Произошла ошибка: {ex.Message}");
                    Console.WriteLine("Попробуйте снова");
                }
            }
        }

        // Метод для дешифровки Цезаря
        static string CaesarDecrypt(string text, int shift)
        {
            int n = alphabet.Length;
            char[] result = new char[text.Length];

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                int index = alphabet.IndexOf(c);

                if (index >= 0)
                {
                    // Инвертируем сдвиг для дешифровки
                    int newIndex = (index - shift) % n;
                    if (newIndex < 0) newIndex += n;
                    result[i] = alphabet[newIndex];
                }
                else
                    result[i] = c;
            }

            return new string(result);
        }

        // Метод для дешифровки Виженера
        static string VigenereDecrypt(string cipherText, string key)
        {
            int n = alphabet.Length;

            int[] keyIndices = key.Where(k => alphabet.Contains(k))
                                  .Select(k => alphabet.IndexOf(k))
                                  .ToArray();

            if (keyIndices.Length == 0)
                return "Ошибка: ключ не содержит допустимых символов";

            int keyLen = keyIndices.Length;
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < cipherText.Length; i++)
            {
                char c = cipherText[i];
                if (alphabet.Contains(c))
                {
                    int cIdx = alphabet.IndexOf(c);
                    int kIdx = keyIndices[i % keyLen];
                    int origIdx = (cIdx - kIdx + n) % n;
                    result.Append(alphabet[origIdx]);
                }
                else
                    result.Append(' ');
            }

            // Убираем повторяющиеся пробелы
            string finalResult = result.ToString();
            while (finalResult.Contains("  "))
            {
                finalResult = finalResult.Replace("  ", " ");
            }

            return finalResult.Trim();
        }

        // Метод для дешифровки Prime
        static string PrimeDecrypt(string cipherText, int multiplicator)
        {
            int mod = alphabet.Length + 1;

            int inv = ModularInverse(multiplicator, mod);
            if (inv == -1)
                return "Ошибка: не могу найти обратный элемент";

            StringBuilder result = new StringBuilder();

            foreach (char ch in cipherText)
            {
                if (!alphabet.Contains(ch))
                {
                    result.Append(' ');
                    continue;
                }

                int encryptedIdx = alphabet.IndexOf(ch);
                int originalPlusOne = ((encryptedIdx + 1) * inv) % mod;
                int originalIdx = originalPlusOne - 1;

                if (originalIdx >= 0 && originalIdx < alphabet.Length)
                    result.Append(alphabet[originalIdx]);
                else
                    result.Append('?');
            }

            // Убираем повторяющиеся пробелы
            string cleaned = Regex.Replace(result.ToString(), @"\s+", " ").Trim();
            return cleaned;
        }

        // Поиск обратного элемента по модулю
        static int ModularInverse(int a, int m)
        {
            for (int x = 1; x < m; x++)
            {
                if ((a * x) % m == 1)
                    return x;
            }
            return -1;
        }
    }
}