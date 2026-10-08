namespace CutRopes
{
    internal class Program
    {
        static int NumberRopes(int mid, int[] ropes) 
        {
            var kol=0;
            for (var i = 0; i < ropes.Length; i++)
                kol += ropes[i] / mid;
            return kol;
        }
        static void Main(string[] args)
        {
            var ropes = new int[] { 5, 6, 6, 8 };
            var k = 4;

            var left = 1;
            var right = ropes.Max() + 1;

            while (left < right)
            {
                var mid = (right - left) / 2 + left;

                if (NumberRopes(mid, ropes) >= k)
                    left = mid + 1;
                else
                    right = mid;
            }
            Console.WriteLine(left - 1);
        }
    }
}
