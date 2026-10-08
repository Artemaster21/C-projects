using System;

namespace Incapsulation.RationalNumbers
{
    /// <summary>
    /// Рациональное число (дробь)
    /// </summary>
    public class Rational
    {
        private readonly int _numerator;
        private readonly int _denominator;

        /// <summary>
        /// Числитель
        /// </summary>
        public int Numerator => _numerator;

        /// <summary>
        /// Знаменатель
        /// </summary>
        public int Denominator => _denominator;

        /// <summary>
        /// Является ли число NaN (деление на ноль)
        /// </summary>
        public bool IsNan => _denominator == 0;

        /// <summary>
        /// Конструктор с числителем и знаменателем
        /// </summary>
        public Rational(int numerator, int denominator = 1)
        {
            if (denominator == 0)
            {
                _numerator = numerator;
                _denominator = 0;
                return;
            }

            // Если знаменатель отрицательный, переносим знак в числитель
            if (denominator < 0)
            {
                numerator = -numerator;
                denominator = -denominator;
            }

            // Сокращаем дробь
            int gcd = GCD(Math.Abs(numerator), Math.Abs(denominator));
            _numerator = numerator / gcd;
            _denominator = denominator / gcd;
        }

        /// <summary>
        /// Алгоритм Евклида для нахождения НОД
        /// </summary>
        private static int GCD(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            return a;
        }

        // ============ ОПЕРАТОРЫ СЛОЖЕНИЯ ============

        public static Rational operator +(Rational a, Rational b)
        {
            if (a.IsNan || b.IsNan)
                return new Rational(0, 0);

            return new Rational(
                a._numerator * b._denominator + b._numerator * a._denominator,
                a._denominator * b._denominator
            );
        }

        // ============ ОПЕРАТОРЫ ВЫЧИТАНИЯ ============

        public static Rational operator -(Rational a, Rational b)
        {
            if (a.IsNan || b.IsNan)
                return new Rational(0, 0);

            return new Rational(
                a._numerator * b._denominator - b._numerator * a._denominator,
                a._denominator * b._denominator
            );
        }

        // ============ ОПЕРАТОРЫ УМНОЖЕНИЯ ============

        public static Rational operator *(Rational a, Rational b)
        {
            if (a.IsNan || b.IsNan)
                return new Rational(0, 0);

            return new Rational(
                a._numerator * b._numerator,
                a._denominator * b._denominator
            );
        }

        // ============ ОПЕРАТОРЫ ДЕЛЕНИЯ ============

        public static Rational operator /(Rational a, Rational b)
        {
            if (a.IsNan || b.IsNan || b._numerator == 0)
                return new Rational(0, 0);

            return new Rational(
                a._numerator * b._denominator,
                a._denominator * b._numerator
            );
        }

        // ============ НЕЯВНОЕ ПРЕОБРАЗОВАНИЕ В DOUBLE ============

        public static implicit operator double(Rational r)
        {
            if (r.IsNan)
                return double.NaN;

            return (double)r._numerator / r._denominator;
        }

        // ============ НЕЯВНОЕ ПРЕОБРАЗОВАНИЕ ИЗ INT ============

        public static implicit operator Rational(int value)
        {
            return new Rational(value, 1);
        }

        // ============ ЯВНОЕ ПРЕОБРАЗОВАНИЕ В INT ============

        public static explicit operator int(Rational r)
        {
            if (r.IsNan)
                throw new InvalidOperationException("Cannot convert NaN to int");

            if (r._numerator % r._denominator != 0)
                throw new InvalidOperationException("Rational number is not an integer");

            return r._numerator / r._denominator;
        }

        /// <summary>
        /// Строковое представление рационального числа
        /// </summary>
        public override string ToString()
        {
            if (IsNan)
                return "NaN";

            return _denominator == 1
                ? $"{_numerator}"
                : $"{_numerator}/{_denominator}";
        }

        /// <summary>
        /// Сравнение рациональных чисел
        /// </summary>
        public override bool Equals(object obj)
        {
            if (obj is Rational other)
            {
                if (IsNan && other.IsNan)
                    return true;
                if (IsNan || other.IsNan)
                    return false;

                return _numerator == other._numerator &&
                       _denominator == other._denominator;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_numerator, _denominator);
        }
    }
}