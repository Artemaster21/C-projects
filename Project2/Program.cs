namespace Project2
{
    internal class Program
    {
        static bool Lucky(int n)
        {
            var n123 = n % 10 + n / 10 % 10 + n / 100 % 10;
            var n456 = n / 1000 % 10 + n / 10000 % 10 + n / 100000 % 10;
            return n123 == n456;
        }
        static void Main(string[] args)
        {
            var n = int.Parse(Console.ReadLine());
            Console.WriteLine(Lucky(n - 1) || Lucky(n + 1) ? "Yes" : "No");
        }
    }
}
