using System;
using System.Linq;
using System.Text;

namespace primeSypher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine();
            Console.OutputEncoding = Encoding.UTF8;

                string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789' ";
                Console.WriteLine("Введите  простое число:");
                var multiplicator = Console.ReadLine();
                int mod = alphabet.Length + 1;

                Console.WriteLine("Введите зашифрованное сообщение:");
                string cipherText = Console.ReadLine();

                string decrypted = DecryptOptimized(cipherText, multiplicator, alphabet, mod);

                Console.WriteLine("\nРасшифрованное сообщение:");
                Console.WriteLine(decrypted);
            

            // ----------- Расшифровка -----------
            static string DecryptOptimized(string cipherText, int multiplicator, string alphabet, int mod)
            {
                int inv = ModularInverse(multiplicator, mod);
                if (inv == -1)
                {
                    return "Ошибка: не могу найти обратный элемент";
                }

                StringBuilder result = new StringBuilder();

                foreach (char ch in cipherText)
                {
                    if (!alphabet.Contains(ch))
                    {
                        result.Append(' ');
                        continue;
                    }

                    int encryptedIdx = alphabet.IndexOf(ch);

                    // Обратная операция:
                    // encrypted = (multiplicator * (original + 1)) % mod - 1
                    // original + 1 = (encrypted + 1) * inv % mod
                    // original = ((encrypted + 1) * inv % mod) - 1

                    int originalPlusOne = ((encryptedIdx + 1) * inv) % mod;
                    int originalIdx = originalPlusOne - 1;

                    if (originalIdx >= 0 && originalIdx < alphabet.Length)
                        result.Append(alphabet[originalIdx]);
                    else
                        result.Append('?');
                }

                // Убираем повторяющиеся пробелы
                string cleaned = System.Text.RegularExpressions.Regex.Replace(result.ToString(), @"\s+", " ").Trim();
                return cleaned;
            }

            // ----------- Поиск обратного элемента по модулю -----------
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
}
