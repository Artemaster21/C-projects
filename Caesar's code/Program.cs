namespace Caesar_s_code
{
    using System;

    class Program
    {
        static void Main()
        {
            Console.Write("Введите сообщение: ");
            string input = Console.ReadLine();

            Console.Write("Введите шаг сдвига (например, 1 или -1): ");
            int shift = -int.Parse(Console.ReadLine());

            string encoded = CaesarCipher(input, shift);
            Console.WriteLine(encoded);
        }

        static string CaesarCipher(string text, int shift)
        {
            // Множество символов, участвующих в шифре:
            string alphabet = "abcdefghijklmnopqrstuvwxyz0123456789' ";
            int n = alphabet.Length;

            char[] result = new char[text.Length];

            for (int i = 0; i < text.Length; i++)
            {
                char c = text[i];
                int index = alphabet.IndexOf(c);

                if (index >= 0)
                {
                    // Сдвиг по кругу
                    int newIndex = (index + shift) % n;
                    if (newIndex < 0) newIndex += n;
                    result[i] = alphabet[newIndex];
                }
                else
                {
                    // Если символ не в алфавите — не меняем
                    result[i] = c;
                }
            }

            return new string(result);
        }
    }
}
