using System.Text;

namespace steganography
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(((6 + 7 / 12) * (6 + 11 + 4) % 6) + ((3 % 1 + 2) * (4 * 0 * 6 - 9) % 12 + (5 + 2 + 14)));
            int b = int.Parse(Console.ReadLine());
            string a = Console.ReadLine();
            StringBuilder result = new StringBuilder();

            while (a.ToLower() != "stop")
            {
                if (a.Length >= b)
                    result.Append(a[b - 1]);
                a = Console.ReadLine();
            }
            Console.WriteLine(result.ToString());

        }
    }
}
