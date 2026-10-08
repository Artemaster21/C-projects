using System;
using System.Collections.Generic;

namespace LimitedSizeStack
{
    public class LimitedSizeStack<T>
    {
        private readonly LinkedList<T> _list = new LinkedList<T>();
        private readonly int _limit;

        public LimitedSizeStack(int undoLimit)
        {
            _limit = undoLimit;
        }

        public int Count => _list.Count;

        public void Push(T item)
        {
            _list.AddFirst(item); 
            
            if (_list.Count > _limit)
                _list.RemoveLast(); 
        }

        public T Pop()
        {
            if (_list.Count == 0)
                throw new InvalidOperationException("Stack is empty");
            
            var value = _list.First.Value;
            _list.RemoveFirst(); 
            return value;
        }
    }
}