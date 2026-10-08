namespace ConsoleApp5
{
    // | M11 M12 | * | 1  1| = |m11+m12: m11|
    // | M21 M22 | * | 1  0| = |m21+m22: m21|
    public class SquareMatrix
    {
        public long M11, M12, M21, M22;
        public SquareMatrix(long m11, long m12, long m21, long m22)
        {
            M11 = m11;
            M12 = m12;
            M21 = m21;
            M22 = m22;
        }

        public static SquareMatrix Multiply(SquareMatrix a, SquareMatrix b)
        {
            return new SquareMatrix(a.M11*b.M11 + a.M12*b.M21,    a.M11*b.M12 + a.M12 * b.M22 , 
                                    a.M12*b.M11 + a.M22*b.M21  ,  a.M21*b.M12 + a.M22 * b.M22);
        }
    }
    public class FibonacciMatrixRecursiveSolver
    {
        private SquareMatrix MatrixPowerRecursive(SquareMatrix matrix, int power)
        {
            //???
            if (power == 0) 
                return new SquareMatrix(1, 0, 0, 1);
            if (power % 2 == 1)
                return SquareMatrix.Multiply(MatrixPowerRecursive(matrix, power - 1), new SquareMatrix(1, 1, 1, 0));
            else
            {
                var b = MatrixPowerRecursive(matrix, power/2);
                return SquareMatrix.Multiply(b,b);
            }
        }

        public long FibonacciRecursive(int i)
        {
            if (i == 0) return 0;
            if (i == 1) return 1;

            var baseMatrix = new SquareMatrix(1, 1, 1, 0);
            var resultMatrix = MatrixPowerRecursive(baseMatrix, i - 1);
            return resultMatrix.M11;
        }
    }

    public class Program
    {
        public static void Main()
        {
            var fibonacciMatrixRecursiveSolver = new FibonacciMatrixRecursiveSolver();

            for (var i = 0; i < 10; i++)
            {
                Console.WriteLine($"{i} число Фиббоначи = {GetFibonacci(i)}");
                Console.WriteLine($"{i} число Фиббоначи через рекурсивное умножение матриц = {fibonacciMatrixRecursiveSolver.FibonacciRecursive(i)}");
            }
        }

        private static long GetFibonacci(int n)
        {
            if (n == 0 || n == 1) return n;

            return GetFibonacci(n - 1) + GetFibonacci(n - 2);
        }
    }
}

