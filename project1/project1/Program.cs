internal class Program
{
    private static void Main(string[] args)
    {
        var All = Console.ReadLine().Split();
        var width = double.Parse(All[0]);
        var radius = double.Parse(All[1]);
        width = width / 2;
        double a = width * Math.Sqrt(2);

        if (width >= radius)
        {
            Console.WriteLine(radius * radius * Math.PI);
        }
        else
        {
            if (a <= radius)
            {
                Console.WriteLine(width * width);
            }
            else
            {
                var sinAngle = 1 - width * width / radius / radius;// получаем синус угла
                double radAngle = Math.Asin(sinAngle) * Math.PI / 180;
                Console.WriteLine(radius * radius * (Math.PI - 2 * (radAngle - sinAngle)));
            }
        }
    }
}