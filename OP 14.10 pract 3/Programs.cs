using System;
using System.Globalization;
using System.Text;

namespace OP_14._10_pract_3
{
    internal class Programs
    {
        static void Main()
        {
            /// стенаграфия 
            //int b = int.Parse(Console.ReadLine());
            //string a = Console.ReadLine();
            //StringBuilder result = new StringBuilder();

            //while (a.ToLower() != "stop")
            //{
            //    if (a.Length >= b)
            //        result.Append(a[b - 1]);
            //    a = Console.ReadLine();
            //}
            //Console.WriteLine(result.ToString());

            // полименальный путь
            //Console.WriteLine();
            Console.WriteLine();
            double a = double.Parse(Console.ReadLine());
            double b = double.Parse(Console.ReadLine());
            double c = double.Parse(Console.ReadLine());

            // Линейное уравнение: a == 0
            if (Math.Abs(a) < 1e-12)
            {
                if (Math.Abs(b) < 1e-12)
                {
                    Console.WriteLine("no roots");
                }
                else
                {
                    double x = -c / b;
                    Console.WriteLine(x.ToString("F5", CultureInfo.InvariantCulture));
                }
                return;
            }

            // Квадратное уравнение
            double d = b * b - 4.0 * a * c;

            if (d < 0.0)
            {
                Console.WriteLine("no roots");
                return;
            }

            double sqrtD = Math.Sqrt(d);
            double x1 = (-b + sqrtD) / (2.0 * a);
            double x2 = (-b - sqrtD) / (2.0 * a);
            bool printed = false;

            if (Math.Abs(a * x1 * x1 + b * x1 + c) < 0.001)
            {
                Console.WriteLine(x1.ToString("F5", CultureInfo.InvariantCulture));
                printed = true;
            }
            if (Math.Abs(a * x2 * x2 + b * x2 + c) < 0.001)
            {
                Console.WriteLine(x2.ToString("F5", CultureInfo.InvariantCulture));
                printed = true;
            }
            if (!printed)
                Console.WriteLine("no roots");
        }
    }
}