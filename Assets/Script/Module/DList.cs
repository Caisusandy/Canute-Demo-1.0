using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.Module
{
    [Serializable]
    public class DList<T> : IEnumerable<T> where T : class
    {
        [SerializeField] protected List<T> list;

        [Obsolete]
        public T this[int index] { get => list[index]; set => list[index] = value; }

        [Obsolete]
        public virtual void Add(T item)
        {
            list.Add(item);
        }

        [Obsolete]
        public virtual void AddRange(IEnumerable<T> enumerable)
        {
            list.AddRange(enumerable);
        }

        [Obsolete]
        public virtual bool Remove(T item)
        {
            return list.Remove(item);
        }

        [Obsolete]
        public virtual void Clear()
        {
            list.Clear();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<T>)list).GetEnumerator();
        }
        public virtual IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T>)list).GetEnumerator();
        }

        [Obsolete]
        public virtual int IndexOf(T item)
        {
            return list.IndexOf(item);
        }

        [Obsolete]
        public virtual void Insert(int index, T item)
        {
            list.Insert(index, item);
        }

        [Obsolete]
        public virtual void RemoveAt(int index)
        {
            list.RemoveAt(index);
        }

        [Obsolete]
        public virtual bool Contains(T item)
        {
            return list.Contains(item);
        }

        [Obsolete]
        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            list.CopyTo(array, arrayIndex);
        }

        [Obsolete]
        public virtual int Count => list.Count;
        [Obsolete]
        public virtual bool IsReadOnly { get; set; }
        [Obsolete]
        public virtual T[] ToArray()
        {
            return list.ToArray();
        }
        [Obsolete]
        public DList()
        {

        }

        public DList(IEnumerable<T> list)
        {
            this.list = list.ToList();
        }

        [Obsolete]
        public DList(int capacity)
        {
            list.Capacity = capacity;
        }

        public static implicit operator List<T>(DList<T> list)
        {
            return list.list;
        }

        public static explicit operator DList<T>(List<T> ts)
        {
            return new DList<T>(ts);
        }
    }
}