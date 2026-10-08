using System;
using System.Collections;
using System.Collections.Generic;

namespace Generics.BinaryTrees
{
    public class BinaryTree<T> : IEnumerable<T> where T : IComparable<T>
    {
        private T _value;
        private BinaryTree<T> _left;
        private BinaryTree<T> _right;
        private bool _hasValue;
        private int _count;

        public T Value => _value;
        public BinaryTree<T> Left => _left;
        public BinaryTree<T> Right => _right;
        public int Count => _count;

        public BinaryTree() { }

        private BinaryTree(T value)
        {
            _value = value;
            _hasValue = true;
            _count = 1;
        }

        public void Add(T value)
        {
            if (!_hasValue)
            {
                _value = value;
                _hasValue = true;
                _count = 1;
                return;
            }

            if (value.CompareTo(_value) <= 0)
            {
                if (_left == null)
                    _left = new BinaryTree<T>(value);
                else
                    _left.Add(value);
            }
            else
            {
                if (_right == null)
                    _right = new BinaryTree<T>(value);
                else
                    _right.Add(value);
            }
            _count++;
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_left != null)
                foreach (var item in _left)
                    yield return item;

            if (_hasValue)
                yield return _value;

            if (_right != null)
                foreach (var item in _right)
                    yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public static class BinaryTree
    {
        public static BinaryTree<T> Create<T>(params T[] values) where T : IComparable<T>
        {
            var tree = new BinaryTree<T>();
            foreach (var value in values)
                tree.Add(value);
            return tree;
        }
    }
}