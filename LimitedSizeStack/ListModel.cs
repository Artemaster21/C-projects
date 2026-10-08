using System;
using System.Collections.Generic;

namespace LimitedSizeStack
{
    public interface ICommand
    {
        void Execute();
        void Undo();
    }

    // новое
    public class MoveUpCommand<T> : ICommand
    {
        private readonly List<T> _items;
        private readonly int _index;

        public MoveUpCommand(List<T> items, int index)
        {
            _items = items;
            _index = index;
        }

        public void Execute()
        {
            if (_index > 0 && _index < _items.Count)
            {
                T temp = _items[_index];
                _items[_index] = _items[_index - 1];
                _items[_index - 1] = temp;
            }
        }

        public void Undo()
        {
            if (_index > 0 && _index < _items.Count)
            {
                T temp = _items[_index - 1];
                _items[_index - 1] = _items[_index];
                _items[_index] = temp;
            }
        }
    }

    public class ListModel<TItem>
    {
        public List<TItem> Items { get; }
        private readonly LimitedSizeStack<ICommand> _undoStack;

        public ListModel(int undoLimit) : this(new List<TItem>(), undoLimit) { }

        public ListModel(List<TItem> items, int undoLimit)
        {
            Items = items;
            _undoStack = new LimitedSizeStack<ICommand>(undoLimit);
        }

        public void AddItem(TItem item)
        {
            var command = new AddCommand<TItem>(Items, item);
            command.Execute();
            _undoStack.Push(command);
        }

        public void RemoveItem(int index)
        {
            var command = new RemoveCommand<TItem>(Items, index);
            command.Execute();
            _undoStack.Push(command);
        }

        //новый метод
        public void MoveUpItem(int index)
        {
            if (index <= 0 || index >= Items.Count)
                throw new InvalidOperationException("Нельзя переместить первый элемент вверх");

            var command = new MoveUpCommand<TItem>(Items, index);
            command.Execute();
            _undoStack.Push(command);
        }

        public bool CanUndo() => _undoStack.Count > 0;

        public void Undo()
        {
            if (!CanUndo())
                throw new InvalidOperationException("Нет действий для отмены");

            _undoStack.Pop().Undo();
        }
    }

    public class AddCommand<T> : ICommand
    {
        private readonly List<T> _items;
        private readonly T _item;
        private int _addedIndex = -1;

        public AddCommand(List<T> items, T item)
        {
            _items = items;
            _item = item;
        }

        public void Execute()
        {
            _items.Add(_item);
            _addedIndex = _items.Count - 1;
        }

        public void Undo()
        {
            if (_addedIndex >= 0 && _addedIndex < _items.Count)
                _items.RemoveAt(_addedIndex);
        }
    }

    public class RemoveCommand<T> : ICommand
    {
        private readonly List<T> _items;
        private readonly int _index;
        private T _removedItem;

        public RemoveCommand(List<T> items, int index)
        {
            _items = items;
            _index = index;
        }

        public void Execute()
        {
            _removedItem = _items[_index];
            _items.RemoveAt(_index);
        }

        public void Undo()
        {
            _items.Insert(_index, _removedItem);
        }
    }
}