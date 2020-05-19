namespace System.Collections.Generic
{
    [Serializable]
    public class Args : DataList<Arg>, IDictionary<string, string>
    {
        public ICollection<string> Keys => GetKeys();

        public ICollection<string> Values => GetValues();


        public Args() : base()
        {

        }

        public new string this[string index]
        {
            get => string.IsNullOrEmpty(base[index].Value) ? throw new KeyNotFoundException() : base[index].Value;
            set => SetParam(index, value);
        }

        public Args(IEnumerable<Arg> args)
        {
            foreach (var item in args)
            {
                Add(item);
            }
        }

        public string TryGet(string key)
        {
            if (ContainsKey(key))
            {
                return this[key];
            }
            else
            {
                return null;
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

        public bool TryGet(string key, out string value)
        {
            foreach (var item in list)
            {
                if (item.Key == key)
                {
                    value = item.Value;
                    return true;
                }
            }
            value = null;
            return false;
        }

        public void SetParam(string key, string value)
        {
            Arg arg = base[key];
            arg.Value = value;
            int v = IndexOf(base[key]);
            RemoveAt(v);
            Insert(v, arg);
        }

        public void Add(string key, string value)
        {
            base.Add(new Arg(key, value));
        }

        public bool Remove(string key)
        {
            try
            {
                Arg arg = base[key];
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

        private ICollection<string> GetValues()
        {
            List<string> a = new List<string>();
            foreach (var item in list)
            {
                a.Add(item.Value);
            }
            return a;
        }

        public bool TryGetValue(string key, out string value)
        {
            try
            {
                Arg arg = base[key];
                value = arg.Value;
                return true;
            }
            catch
            {
                value = null;
                return false;
            }
        }

        public void Add(KeyValuePair<string, string> item)
        {
            base.Add(item);
        }

        public bool Contains(KeyValuePair<string, string> item)
        {
            return base.Contains(item);
        }

        public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<string, string> item)
        {
            throw new NotImplementedException();
        }

        IEnumerator<KeyValuePair<string, string>> IEnumerable<KeyValuePair<string, string>>.GetEnumerator()
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
                ans += item.Key;
                ans += ": ";
                ans += item.Value;
                ans += ";";
            }
            return ans;
        }


        public static implicit operator ArgList(Args args)
        {
            return new ArgList(args);
        }
    }
}
