namespace practic7
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var ob = new List<int>();
            var raz = new List<int>();
            var per = new List<int>();
            int[] array1 = { 1, 1, 1, 3, 4 };
            int[] array2 = { 2, 4, 5, 7, 7, 8, 10, 12, 14 };
            var f1 = 0;
            var f2 = 0;
            while (f1 < array1.Length & f2 < array2.Length)
            {
                if (array1[f1] < array2[f2])
                {
                    ob.Add(array1[f1]);
                    raz.Add(array1[f1]);
                    f1++;
                }
                else if (array1[f1] > array2[f2])
                {
                    ob.Add(array2[f2]);
                    raz.Add(array1[f1]);
                    f2++;
                }
                else
                {
                    per.Add(array1[f1]);
                    ob.Add(array1[f1]);
                    ob.Add(array1[f1]);
                    f1++;
                    f2++;
                }
            }

            for (int i = f1; i < array1.Length; i++)
            {
                ob.Add(array1[i]);
            }

            for (int i = f2; i < array2.Length; i++)
            {
                ob.Add(array2[i]);
            }
            Console.WriteLine(string.Join(" ", ob));

        }
    }
}
