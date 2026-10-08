using System;

namespace Incapsulation.Weights
{
    /// <summary>
    /// Обертка над массивом double[], предоставляющая доступ к его подмассиву
    /// </summary>
    public class Indexer
    {
        private readonly double[] _array;
        private readonly int _start;
        private readonly int _length;

        /// <summary>
        /// Длина подмассива (количество доступных элементов)
        /// </summary>
        public int Length => _length;

        /// <summary>
        /// Конструктор Indexer
        /// </summary>
        /// <param name="array">Исходный массив</param>
        /// <param name="start">Начальный индекс в исходном массиве (включительно)</param>
        /// <param name="length">Длина подмассива</param>
        /// <exception cref="ArgumentException">Если диапазон выходит за границы массива</exception>
        public Indexer(double[] array, int start, int length)
        {
            if (array == null)
                throw new ArgumentException("Array cannot be null", nameof(array));

            if (start < 0 || start > array.Length)
                throw new ArgumentException($"Start index {start} is out of range [0, {array.Length}]");

            if (length < 0)
                throw new ArgumentException($"Length {length} cannot be negative");

            if (start + length > array.Length)
                throw new ArgumentException($"Range [{start}, {start + length}) is out of array bounds [0, {array.Length}]");

            _array = array;
            _start = start;
            _length = length;
        }

        /// <summary>
        /// Индексатор для доступа к элементам подмассива
        /// </summary>
        /// <param name="index">Индекс в подмассиве (от 0 до Length-1)</param>
        /// <returns>Значение элемента</returns>
        /// <exception cref="IndexOutOfRangeException">Если индекс вне диапазона [0, Length-1]</exception>
        public double this[int index]
        {
            get
            {
                if (index < 0 || index >= _length)
                    throw new IndexOutOfRangeException($"Index {index} is out of range [0, {_length - 1}]");

                return _array[_start + index];
            }
            set
            {
                if (index < 0 || index >= _length)
                    throw new IndexOutOfRangeException($"Index {index} is out of range [0, {_length - 1}]");

                _array[_start + index] = value;
            }
        }
    }
}