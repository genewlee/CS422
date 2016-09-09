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

        Node _front, _back, _dummy;

        public PCQueue()
        {
            _front = _back = new Node(0);
        }

        /*public bool Dequeue(ref int out_value)
        {
            if (object.ReferenceEquals(_back, _dummy))
                return false;

            if (object.ReferenceEquals(_front, _dummy))
            {
                if (_front.Next != null) // only one in queue
                {
                    out_value = _front.Next.Data;
                    _front = _front.Next;
                    if (_front.Next != null)
                        _front = _front.Next;
                    return true;
                }
                return false;
            }

            // more than one value in queue
            out_value = _front.Data;
            if (object.ReferenceEquals(_front, _back)) // only one item
            {
                _front = _back = _dummy;
                return true;
            }
            _front = _front.Next;
            return true;
        }*/

        /// <summary>
        /// Dequeue the specified out_value.
        /// </summary>
        /*public bool Dequeue(ref int out_value)
        {
            if (object.ReferenceEquals(_back, _dummy))
                return false;

            if (object.ReferenceEquals(_front, _dummy))
            {
                if (_front.Next != null) // atleast one in queue
                {
                    out_value = _front.Next.Data;
                    if (_front.Next.Next == null) // only two in queue
                    {
                        _back = _dummy;
                        return true;
                    }
                    _front = _front.Next;
                    return true;
                }
                _back = _dummy;
                return false;
            }
            else // more than two values
            {
                out_value = _front.Data;
                if (_front.Next.Next == null)
                {
                    _front = _dummy;
                    _front.Next = _back;
                    return true;
                }
                _front = _front.Next;
                return true;
            }
        }*/

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

