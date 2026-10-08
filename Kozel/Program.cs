using System.ComponentModel.Design;

namespace Kozel
{
    class Program
    {
        static void Main()
        {
            var All = Console.ReadLine().Split();
            var width = double.Parse(All[0]);
            var radius = double.Parse(All[1]);
            width /= 2.0;
            double a = width * Math.Sqrt(2);
            
            if (width>=radius)
            {
                Console.WriteLine(radius * radius* Math.PI);
            }
            else 
            {
                if (a <= radius)
                {
                    Console.WriteLine(width * width * 4);
                }
                else
                {
                    var sinAngle = 1 - width * width / radius / radius;// получаем синус угла
                    double radAngle = (Math.Asin(sinAngle) * Math.PI / 180);
                    Console.WriteLine(radius * radius * (Math.PI - 2 * (radAngle - sinAngle)));
                }
            }
        }
    }
}
var All = Console.ReadLine().Split();
var width = int.Parse(All[0]);
var circle = int.Parse(All[1]);
double square = 0;
if (circle <= width / 2.0)
{
    square = Math.PI * circle * circle;
}
else if (circle >= (Math.Sqrt(2) * (width)) / 2)
{
    square = width * width;
}
else
{
    double angle = Math.Acos(width / 2.0 / circle) * 2;
    var seg = (circle * circle / 2.0) * (angle - Math.Sin(angle));
    square = Math.PI * circle * circle - 4 * seg;
}
Console.WriteLine(square);

