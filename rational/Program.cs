using System;
using System.Data;

namespace rational
{
    class RationalNumbers : IEquatable<RationalNumbers?>
    {                                       // m/n  n/z
        public int N { get; }
        public int Z { get; }
        public RationalNumbers(int n, int z)
        {
            if (z < 0)
            {
                n = -n;
                z = -z;
            }
            if (z == 0)
                throw new ArgumentException();
            (N,Z)= Normalize(n, z);
        }


        public (int N, int Z) Normalize(int n, int z)
        {
            var a = Math.Abs(n);
            var b = Math.Abs(z);
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return (n/a, z/a);
        }

        public override string ToString()
        {
            if (Z == 0) 
                return N.ToString();
            return $"{N}/{Z})";
        }

        public override bool Equals(object? obj)
        {
            return Equals(obj as RationalNumbers);
        }

        public bool Equals(RationalNumbers? other)
        {
            return other is not null &&
                   N == other.N &&
                   Z == other.Z;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(N, Z);
        }

        public static RationalNumbers operator +(RationalNumbers a, RationalNumbers b)
        {
            var commonDenominator = a.N * b.Z;
            var newNumerator = a.N * b.Z + b.N * a.Z;
            return new RationalNumbers(newNumerator, commonDenominator);
        }

        public static RationalNumbers operator -(RationalNumbers a, RationalNumbers b)
        {
            int commonDenominator = a.N * b.Z;
            int newNumerator = a.N * b.Z - b.N * a.Z;
            return new RationalNumbers(newNumerator, commonDenominator);
        }

        public static (int,int) operator *(RationalNumbers a, RationalNumbers b)
        {
            return new(a.N * b.N, a.Z * b.Z);
        }

        public static (int, int) operator /(RationalNumbers a, RationalNumbers b)
        {
            return new(a.N * b.Z, a.Z * b.N);
        }

        public static bool operator ==(RationalNumbers? left, RationalNumbers? right)
        {
            return EqualityComparer<RationalNumbers>.Default.Equals(left, right);
        }

        public static bool operator !=(RationalNumbers? left, RationalNumbers? right)
        {
            return !(left == right);
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            var q = new RationalNumbers(-6,3);
            var w = new RationalNumbers(3,3);

            Console.WriteLine((q+w).ToString());
        }
    }
}
