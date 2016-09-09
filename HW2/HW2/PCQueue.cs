using System;

namespace CS422
{
    public class PCQueue
    {
        private class Node
        {
            private int _data;
            private Node _next;
            public int Data { 
                get 
                {
                    return _data;
                }
                set
                {
                    _data = value;
                }
            }
            public Node Next { 
                get 
                {
                    return _next;
                }
                set
                {
                    _next = value;
                }
            }

            public Node(int newData)
            {
                _data = newData;
            }
        }

        Node _front, _back;

        public PCQueue()
        {
            _front = _back = new Node(-1);
        }

        /// <summary>
        /// Dequeue the specified out_value.
        /// </summary>
        public bool Dequeue(ref int out_value)
        {
            if (_front.Next != null)
            {
                out_value = _front.Next.Data;
                _front = _front.Next;
                return true;
            }
            else // _front and _back is the same
            {
                return false;
            }
        }

        /// <summary>
        /// Enqueue the specified dataValue.
        /// Enqueue only touches the back
        /// </summary>
        public void Enqueue (int dataValue)
        {
            _back.Next = new Node(dataValue);
            _back = _back.Next;
        }
    }
}

