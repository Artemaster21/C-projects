namespace project_3
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var n = int.Parse(Console.ReadLine());
            var b = 0;
            for (var i = 1; i < 10; i++)
            {
                for (var j = 0; j < 10; j++)
                {
                    for (var k = 0; k < 10; k++)
                    {
                        if ((i + j + k) == n)
                        {
                            b++;
                        }
                    }
                }
            }
            Console.WriteLine(b);

        }
    }
}
