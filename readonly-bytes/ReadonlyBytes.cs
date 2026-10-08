using System;
using System.Collections;
using System.Collections.Generic;

namespace hashes
{
    public class ReadonlyBytes : IEnumerable<byte>
    {
        private readonly byte[] _bytes;
        private readonly int _hashCode;

        // Конструктор из массива
        public ReadonlyBytes(params byte[] bytes)
        {
            if (bytes == null)
                throw new ArgumentNullException(nameof(bytes));

            _bytes = new byte[bytes.Length];
            Array.Copy(bytes, _bytes, bytes.Length);

            // Вычисляем хеш-код при создании (кэшируем)
            _hashCode = ComputeHashCode();
        }

        // Конструктор без параметров (пустой массив)
        public ReadonlyBytes() : this(new byte[0]) { }

        // Длина массива
        public int Length => _bytes.Length;

        // Индексатор
        public byte this[int index]
        {
            get
            {
                if (index < 0 || index >= _bytes.Length)
                    throw new IndexOutOfRangeException();
                return _bytes[index];
            }
        }

        // Вычисление хеш-кода (полиномиальный хеш)
        private int ComputeHashCode()
        {
            // Используем числа, рекомендованные в тесте для избежания коллизий
            unchecked // Разрешаем переполнение
            {
                int hash = 1;
                foreach (byte b in _bytes)
                {
                    hash = hash * 37 + b;  // 37 - простое число
                }
                return hash;
            }
        }

        // Переопределение Equals
        public override bool Equals(object obj)
        {
            // Проверка на null и тип
            if (obj == null || GetType() != obj.GetType())
                return false;

            var other = (ReadonlyBytes)obj;

            // Если ссылки на один объект
            if (ReferenceEquals(this, other))
                return true;

            // Если разная длина
            if (_bytes.Length != other._bytes.Length)
                return false;

            // Поэлементное сравнение
            for (int i = 0; i < _bytes.Length; i++)
            {
                if (_bytes[i] != other._bytes[i])
                    return false;
            }

            return true;
        }

        // Переопределение GetHashCode
        public override int GetHashCode()
        {
            return _hashCode;  // Возвращаем кэшированное значение
        }

        // Переопределение ToString
        public override string ToString()
        {
            return "[" + string.Join(", ", _bytes) + "]";
        }

        // Реализация IEnumerable<byte>
        public IEnumerator<byte> GetEnumerator()
        {
            foreach (byte b in _bytes)
                yield return b;
        }

        // Реализация IEnumerable (необобщённый)
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}