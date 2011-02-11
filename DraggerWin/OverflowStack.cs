using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CrackSoft.Collections
{
    class OverflowStack<T> : IEnumerable<T>, ICollection
    {
        object syncRoot = new object();
        LinkedList<T> items;
        int maxItems;

        public OverflowStack(int capacity)
        {
            maxItems = capacity;
            items = new LinkedList<T>();
        }

        public OverflowStack(IEnumerable<T> collection)
        {
            items = new LinkedList<T>(collection);
            maxItems = items.Count;
        }

        public void Push(T item)
        {
            if (items.Count == maxItems)
                items.RemoveLast();
            items.AddFirst(item);
        }

        public T Pop()
        {
            T item = items.First.Value;
            items.RemoveFirst();
            return item;
        }

        public T Peek()
        {
            return items.First.Value;
        }

        public void Clear()
        {
            items.Clear();
        }

        public bool IsEmpty
        {
            get { return Count == 0; }
        }        

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        #endregion

        #region ICollection Members

        public void CopyTo(Array array, int index)
        {
            if (array is T[])
                items.CopyTo((T[])array, index);
        }
        
        public int Count
        {
            get { return items.Count; }
        }

        public bool IsSynchronized
        {
            get { return false; }
        }

        public object SyncRoot
        {
            get { return syncRoot; }
        }

        #endregion        
    }
}
