using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{
    /// <summary>
    /// A key-value pair of string/bool
    /// can construct a full list and check on the list  
    /// </summary>
    [Serializable]
    public class CheckList : DataList<Check>, IDictionary<string, bool>
    {
        public ICollection<string> Keys => GetKeys();

        public ICollection<bool> Values => GetValues();


        public CheckList() : base()
        {

        }

        public new bool this[string index]
        {
            get => base[index].Value;
            set => SetParam(index, value);
        }

        public CheckList(IEnumerable<Check> args)
        {
            foreach (var item in args)
            {
                Add(item);
            }
        }

        public bool TryGet(string key)
        {
            if (ContainsKey(key))
            {
                return this[key];
            }
            else
            {
                return default;
            }
        }

        public bool ContainsKey(string key)
        {
            foreach (var item in this)
            {
                if (item.Key == key)
                {
                    return true;
                }
            }
            return false;
        }

        public bool TryGet(string key, out bool value)
        {
            foreach (var item in list)
            {
                if (item.Key == key)
                {
                    value = item.Value;
                    return true;
                }
            }
            value = default;
            return false;
        }

        public void SetParam(string key, bool value)
        {
            Check arg = base[key];
            arg.Value = value;
            int v = IndexOf(base[key]);
            RemoveAt(v);
            Insert(v, arg);
        }

        public void Add(string key, bool value)
        {
            base.Add(new Check(key, value));
        }

        public bool Remove(string key)
        {
            try
            {
                Check arg = base[key];
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
                a.Add(item.Key);
            }
            return a;
        }

        private ICollection<bool> GetValues()
        {
            List<bool> a = new List<bool>();
            foreach (var item in list)
            {
                a.Add(item.Value);
            }
            return a;
        }

        public bool TryGetValue(string key, out bool value)
        {
            try
            {
                Check arg = base[key];
                value = arg.Value;
                return true;
            }
            catch
            {
                value = default;
                return false;
            }
        }

        public void Add(KeyValuePair<string, bool> item)
        {
            base.Add(item);
        }

        public bool Contains(KeyValuePair<string, bool> item)
        {
            return Contains((Check)item);
        }

        public void CopyTo(KeyValuePair<string, bool>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, bool> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<string, bool>> IEnumerable<KeyValuePair<string, bool>>.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public CheckList(IEnumerable<INameable> fullList, IEnumerable<INameable> check)
        {
            foreach (INameable item in fullList)
            {
                Add(item.Name, false);
            }
            foreach (INameable item in check)
            {
                this[item.Name] = true;
            }
        }

        public CheckList(IEnumerable<INameable> fullList, IEnumerable<string> check)
        {
            foreach (INameable item in fullList)
            {
                Add(item.Name, false);
            }
            foreach (string item in check)
            {
                this[item] = true;
            }
        }
    }

    [Serializable]
    public struct Check : INameable
    {
        [SerializeField] private string key;
        [SerializeField] private bool value;

        public string Key { get => key; set => key = value; }
        public bool Value { get => value; set => this.value = value; }
        public string Name => Key;

        public Check(string key, bool value)
        {
            this.key = key;
            this.value = value;
        }

        public static implicit operator KeyValuePair<string, bool>(Check arg)
        {
            return new KeyValuePair<string, bool>(arg.key, arg.value);
        }

        public static implicit operator Check(KeyValuePair<string, bool> arg)
        {
            return new Check(arg.Key, arg.Value);
        }
        public static implicit operator Arg(Check arg)
        {
            return new KeyValuePair<string, string>(arg.key, arg.value.ToString());
        }

        public static implicit operator Check(Arg arg)
        {
            return new Check(arg.Key, bool.Parse(arg.Value));
        }

        public static implicit operator Check(string arg)
        {
            if (arg is null)
            {
                throw new ArgumentException();
            }
            if (arg.Split(',').Length > 2 || arg.Split(',').Length < 1)
            {
                throw new ArgumentException();
            }

            string[] vs = arg.Split(',');

            Check arg1 = new Check(vs[0], false);
            if (vs.Length > 1)
            {
                arg1.value = bool.Parse(vs[1]);
            }
            return arg1;
        }

        public bool Equals(Check other)
        {
            return other.value == value && other.key == key;
        }

        public override bool Equals(object obj)
        {
            return obj is Arg ? Equals((Check)obj) : false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Check left, Check right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Check left, Check right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return '{' + key + ": " + value + '}';
        }
    }
}
