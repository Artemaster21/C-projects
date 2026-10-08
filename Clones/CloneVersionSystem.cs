using System;
using System.Collections.Generic;

namespace Clones
{
    // Узел односвязного списка для стека
    public class Node<T>
    {
        public readonly T Value;
        public readonly Node<T> Previous;

        public Node(T value, Node<T> previous)
        {
            Value = value;
            Previous = previous;
        }
    }

    // Стек на связных списках
    public class MyStack<T>
    {
        private Node<T> _head;

        public bool IsEmpty => _head == null;

        public void Push(T value)
        {
            _head = new Node<T>(value, _head);
        }

        public T Pop()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Stack is empty");

            T value = _head.Value;
            _head = _head.Previous;
            return value;
        }

        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Stack is empty");

            return _head.Value;
        }

        // Клонирование - просто копируем ссылку на вершину
        public MyStack<T> MakeCopy()
        {
            var newStack = new MyStack<T>();
            newStack._head = this._head;
            return newStack;
        }
    }

    // Класс, представляющий клона
    public class CloneItem
    {
        private MyStack<string> _programs;      // Стек выученных программ
        private MyStack<string> _rollbackStack; // Стек откатов

        public CloneItem()
        {
            _programs = new MyStack<string>();
            _rollbackStack = new MyStack<string>();
        }

        private CloneItem(MyStack<string> programs, MyStack<string> rollbackStack)
        {
            _programs = programs.MakeCopy();
            _rollbackStack = rollbackStack.MakeCopy();
        }

        public void Learn(string program)
        {
            _programs.Push(program);
            _rollbackStack = new MyStack<string>(); // Очищаем историю откатов
        }

        public void Rollback()
        {
            string lastProgram = _programs.Pop();
            _rollbackStack.Push(lastProgram);
        }

        public void Relearn()
        {
            string program = _rollbackStack.Pop();
            _programs.Push(program);
        }

        public string Check()
        {
            return _programs.IsEmpty ? "basic" : _programs.Peek();
        }

        public CloneItem MakeCopy()
        {
            return new CloneItem(_programs, _rollbackStack);
        }
    }

    // Основной класс системы
    public class CloneVersionSystem : ICloneVersionSystem
    {
        private List<CloneItem> _clones;

        public CloneVersionSystem()
        {
            _clones = new List<CloneItem>();
            _clones.Add(new CloneItem()); // Клон №1 (индекс 0)
        }

        public string Execute(string query)
        {
            string[] parts = query.Split(' ');
            string command = parts[0];
            int cloneNumber = int.Parse(parts[1]);
            int cloneIndex = cloneNumber - 1; // Номера с 1, индексы с 0

            CloneItem currentClone = _clones[cloneIndex];

            switch (command)
            {
                case "learn":
                    string program = parts[2];
                    currentClone.Learn(program);
                    return null;

                case "rollback":
                    currentClone.Rollback();
                    return null;

                case "relearn":
                    currentClone.Relearn();
                    return null;

                case "clone":
                    CloneItem newClone = currentClone.MakeCopy();
                    _clones.Add(newClone);
                    return null;

                case "check":
                    return currentClone.Check();

                default:
                    return null;
            }
        }
    }
}