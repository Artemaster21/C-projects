namespace Vigenere_scode
{
    using System;
    using System.Linq;

    class Program
    {
        static void Main()
        {
            string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789' ";

            Console.WriteLine("Введите зашифрованное сообщение:");
            string cipherText = Console.ReadLine() ?? "";

            Console.WriteLine("Введите ключ:");
            string key = Console.ReadLine() ?? "";

            string decrypted = VigenereDecrypt(cipherText, key, alphabet);
            Console.WriteLine("\nРасшифрованное сообщение:");
            Console.WriteLine(decrypted);
        }

        static string VigenereDecrypt(string cipherText, string key, string alphabet)
        {
            int n = alphabet.Length;

            // Преобразуем ключ в индексы в алфавите
            int[] keyIndices = key.Where(k => alphabet.Contains(k))
                                  .Select(k => alphabet.IndexOf(k))
                                  .ToArray();
            if (keyIndices.Length == 0)
                return "Ошибка: ключ не содержит допустимых символов";

            int keyLen = keyIndices.Length;
            string result = "";

            for (int i = 0; i < cipherText.Length; i++)
            {
                char c = cipherText[i];
                if (alphabet.Contains(c))
                {
                    int cIdx = alphabet.IndexOf(c);
                    int kIdx = keyIndices[i % keyLen];
                    int origIdx = (cIdx - kIdx + n) % n;
                    result += alphabet[origIdx];
                }
                else
                {
                    // заменяем все недопустимые символы на пробел
                    result += ' ';
                }
            }

            // убираем повторяющиеся пробелы
            while (result.Contains("  "))
            {
                result = result.Replace("  ", " ");
            }

            return result.Trim();
        }
    }

}
