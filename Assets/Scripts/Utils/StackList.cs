using System.Collections.Generic;

public class StackList<T> : List<T>
    {
        public void Push(T item)
        {
            Add(item);
        }

        public T Pop()
        {
            T item = this[Count - 1];
            RemoveAt(Count - 1);
            return item;
        }
        
        public T Shift()
        {
            T item = this[0];
            RemoveAt(0);
            return item;
        }
        
        public T PeekLast()
        {
            return Count != 0  ?  this[Count - 1] : default;
        }
        
        public T PeekFirst()
        {
            return Count != 0 ? this[0] : default;
        }
    }