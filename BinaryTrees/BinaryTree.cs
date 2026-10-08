using System;
using System.Collections;
using System.Collections.Generic;

namespace BinaryTrees;

public class BinaryTree<T> : IEnumerable<T> where T : IComparable
{
    private class Node
    {
        public T Value { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }

        public Node(T value)
        {
            Value = value;
        }
    }

    private Node _root;

    public void Add(T key)
    {
        if (_root == null)
        {
            _root = new Node(key);
            return;
        }

        Node current = _root;
        while (true)
        {
            var comparison = key.CompareTo(current.Value);

            if (comparison < 0)
            {
                if (current.Left == null)
                {
                    current.Left = new Node(key);
                    break;
                }
                current = current.Left;
            }
            else if (comparison > 0)
            {
                if (current.Right == null)
                {
                    current.Right = new Node(key);
                    break;
                }
                current = current.Right;
            }
            else
            {
                break;
            }
        }
    }

    public bool Contains(T key)
    {
        Node current = _root;

        while (current != null)
        {
            var comparison = key.CompareTo(current.Value);

            if (comparison < 0)
                current = current.Left;
            else if (comparison > 0)
                current = current.Right;
            else
                return true;
        }

        return false;
    }

    public IEnumerator<T> GetEnumerator()
    {
        var stack = new Stack<Node>();
        var current = _root;

        while (stack.Count > 0 || current != null)
        {
            while (current != null)
            {
                stack.Push(current);
                current = current.Left;
            }

            current = stack.Pop();
            yield return current.Value;
            current = current.Right;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}