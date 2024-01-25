using System;
using System.Collections;
using System.Collections.Generic;

namespace TicTacToe
{
    public class ListQueue<T> : IEnumerable<T>
    {
        private Node _head;
        private Node _tail;
        private int _count;

        public void Enqueue(T data)
        {
            var node = new Node(data);
            var tempNode = _tail;
            _tail = node;
            if (_count == 0)
            {
                _head = _tail;
            }
            else
            {
                tempNode.Next = _tail;
            }

            _count++;
        }

        public T Dequeue()
        {
            if (_count == 0)
            {
                throw new InvalidOperationException();
            }

            var output = _head.Data;
            _head = _head.Next;
            _count--;
            return output;
        }

        public T First
        {
            get
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException();
                }

                return _head.Data;
            }
        }

        public T Last
        {
            get
            {
                if (IsEmpty)
                {
                    throw new InvalidOperationException();
                }

                return _tail.Data;
            }
        }

        public int Count => _count;

        public bool IsEmpty => _count == 0;

        public void Clear()
        {
            _head = null;
            _tail = null;
            _count = 0;
        }

        public bool Contains(T data)
        {
            var current = _head;
            while (current != null)
            {
                if (current.Data.Equals(data))
                {
                    return true;
                }

                current = current.Next;
            }

            return false;
        }

        public void Remove(T data)
        {
            Node prev = null;
            for (var current = _head; current != null; current = current.Next)
            {
                if (current.Data.Equals(data))
                {
                    if (prev == null)
                    {
                        _head = current.Next;
                    }
                    else
                    {
                        prev.Next = current.Next;
                    }

                    _count--;
                }

                prev = current;
            }
        }
        
        public void Remove(Func<T, bool> where)
        {
            Node prev = null;
            for (var current = _head; current != null; current = current.Next)
            {
                if (where(current.Data))
                {
                    if (prev == null)
                    {
                        _head = current.Next;
                    }
                    else
                    {
                        prev.Next = current.Next;
                    }
                }

                prev = current;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) this).GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            var current = _head;
            while (current != null)
            {
                yield return current.Data;
                current = current.Next;
            }
        }

        private class Node
        {
            public Node(T data)
            {
                Data = data;
            }

            public T Data { get; set; }
            public Node Next { get; set; }
        }
    }
}