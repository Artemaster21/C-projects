namespace OP2110nom23
{
    internal class Program
    {

        static void Main()
        {
            var x = int.Parse(Console.ReadLine());
            var N = Console.ReadLine().Split().Select(int.Parse).ToArray();
            MakeCouple(x, N);
        }

        private static void MakeCouple(int x, int[] N)
        {
            var p0 = 0;
            var p2 = N.Length - 1;
            while (p0 < p2)
            {
                if (N[p0] + N[p2] > x)
                    p2--;
                if (N[p0] + N[p2] < x)
                    p0++;
                if (N[p0] + N[p2] == x)
                    Console.WriteLine(N[p0].ToString() + ' ' + N[p2].ToString());
                break;
            }
        }
    }
}
