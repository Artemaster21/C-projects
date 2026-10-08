using System;
using System.Collections.Generic;

namespace VersionSteck
{
    public class VersionedStack<T>
    {
        private class Yzel
        {
            public T Value { get; }
            public Yzel Next { get; }

            public Yzel(T value, Yzel next)
            {
                Value = value;
                Next = next;
            }
        }

        private readonly List<Yzel> history;

        private int currentVersion;

        public VersionedStack()
        {
            history = new List<Yzel>(); 
            currentVersion = 0;
        }

        public void Push(T item)
        {
            Yzel currentTop = history[currentVersion];

            Yzel newNode = new Yzel(item, currentTop);

            history.Add(newNode);

            currentVersion = history.Count - 1;
        }

        public T Pop()
        {
            if (history.Count == 0)
                throw new InvalidOperationException("Стек пуст");

            Yzel currentTop = history[^1];
            Yzel newTop = currentTop.Next;

            history.Add(newTop);

            currentVersion = history.Count - 1;
            return currentTop.Value;
        }

        public void Rollback(int version)
        {
            if (version < 0 || version >= history.Count)
                throw new ArgumentException("Неверный номер версии");

            currentVersion = version;

            history.Add(history[version]);
        }

        public void Forget()
        {
            Yzel currentTop = history[currentVersion];

            history.Clear();

            history.Add(currentTop);

            currentVersion = 0;
        }

        public Stack<T> GetCurrentStack()
        {
            var result = new Stack<T>();
            var temp = new Stack<T>();

            Yzel current = history[currentVersion];
            while (current != null)
            {
                temp.Push(current.Value);
                current = current.Next;
            }

            while (temp.Count > 0)
                result.Push(temp.Pop());

            return result;
        }

        public void PrintVersions()
        {
            for (var i = 0; i < history.Count; i++)
            {
                Console.Write($"Версия {i}: [");

                var elements = new List<T>();
                Yzel current = history[i];
                while (current != null)
                {
                    elements.Add(current.Value);
                    current = current.Next;
                }

                elements.Reverse();
                Console.Write(string.Join(", ", elements));

                Console.WriteLine(currentVersion == i ? "] ← текущая" : "]");
            }
        }
    }
}
