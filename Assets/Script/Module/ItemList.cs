using Canute.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public class CountableItem : INameable
    {
        [SerializeField] private string name;
        [SerializeField] private int count;

        public CountableItem(string name, int count)
        {
            this.name = name;
            this.count = count;
        }

        public string Name { get => name; set => name = value; }
        public int Count { get => count; set => count = value; }

        [Obsolete]
        public static implicit operator CountableItem(KeyValuePair<string, int> keyValuePair)
        {
            return new CountableItem(keyValuePair.Key, keyValuePair.Value);
        }

        public bool Equals(CountableItem countableItem)
        {
            if (countableItem is null) return false;
            return name == countableItem.name && count == countableItem.count;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as CountableItem);
        }

        public static bool operator ==(CountableItem a, CountableItem b)
        {
            if (a is null && b is null)
            {
                return true;
            }
            return a?.Equals(b) == true;
        }
        [Obsolete]
        public static bool operator !=(CountableItem a, CountableItem b)
        {
            return !(a == b);
        }


    }

    [Serializable]
    [Obsolete]
    public class ItemList : DataList<CountableItem>, IDictionary<string, int>
    {
        public ICollection<string> Keys => GetKeys();
        [Obsolete]
        public ICollection<int> Values => GetValues();

        [Obsolete]
        public ItemList() : base()
        {

        }

        public new int this[string index]
        {
            get => base[index].Count;
            set => SetParam(index, value);
        }

        [Obsolete]
        public ItemList(IEnumerable<CountableItem> args)
        {
            foreach (var item in args)
            {
                Add(item);
            }
        }

        [Obsolete]
        public int TryGet(string key)
        {
            if (ContainsKey(key))
            {
                return this[key];
            }
            else
            {
                return 0;
            }
        }

        public bool ContainsKey(string key)
        {
            foreach (var item in this)
            {
                if (item.Name == key)
                {
                    return true;
                }
            }
            return false;
        }

        [Obsolete]
        public bool TryGet(string key, out int value)
        {
            foreach (var item in list)
            {
                if (item.Name == key)
                {
                    value = item.Count;
                    return true;
                }
            }
            value = 0;
            return false;
        }

        public void SetParam(string key, int value)
        {
            base[key].Count = value;
        }

        public void Add(string key, int value)
        {
            base.Add(new CountableItem(key, value));
        }

        [Obsolete]
        public bool Remove(string key)
        {
            try
            {
                var arg = base[key];
                int v = IndexOf(base[key]);
                RemoveAt(v);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private ICollection<string> GetKeys()
        {
            List<string> a = new List<string>();
            foreach (var item in list)
            {
                a.Add(item.Name);
            }
            return a;
        }

        private ICollection<int> GetValues()
        {
            List<int> a = new List<int>();
            foreach (var item in list)
            {
                a.Add(item.Count);
            }
            return a;
        }

        [Obsolete]
        public bool TryGetValue(string key, out int value)
        {
            try
            {
                var arg = base[key];
                value = arg.Count;
                return true;
            }
            catch
            {
                value = 0;
                return false;
            }
        }

        public void Add(KeyValuePair<string, int> item)
        {
            base.Add(item);
        }

        public bool Contains(KeyValuePair<string, int> item)
        {
            return base.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, int>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, int> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<string, int>> IEnumerable<KeyValuePair<string, int>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            string ans = "";
            if (Count == 0)
            {
                return ans;
            }
            foreach (var item in this)
            {
                ans += "\n  -";
                ans += item.Name;
                ans += ": ";
                ans += item.Count;
                ans += ";";
            }
            return ans;
        }
    }
}
