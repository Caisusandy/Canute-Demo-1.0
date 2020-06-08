using Canute;
using UnityEngine;

namespace System.Collections.Generic
{
    [Serializable]
    public class Args : DataList<Arg>, IDictionary<string, string>
    {
        public ICollection<string> Keys => GetKeys();
        public ICollection<string> Values => GetValues();


        /// <summary>
        /// add args to the effect
        /// </summary>
        /// <param name="args"></param>
        public void AddParams(params Arg[] args)
        {
            foreach (var item in args)
            {
                this[item.Key] = item.Value;
            }
        }

        /// <summary>
        /// get a integer parameter by <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetIntParam(string key)
        {
            try
            {
                return int.Parse(string.IsNullOrEmpty(this[key]) ? "-1" : this[key]);
            }
            catch
            {
                Debug.LogError("Conversion Failed: " + this[key] + "is not a integer");
                return -1;
            }
        }

        /// <summary>
        /// get a boolean parameter by <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetBoolParam(string key)
        {
            try
            {
                return bool.Parse(string.IsNullOrEmpty(this[key]) ? "false" : this[key]);
            }
            catch
            {
                Debug.LogError("Conversion Failed: " + this[key] + "is not a boolean");
                return false;
            }
        }

        /// <summary>
        /// get a double parameter by <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public double GetDoubleParam(string key)
        {
            try
            {
                return double.Parse(string.IsNullOrEmpty(this[key]) ? "-1" : this[key]);
            }
            catch
            {
                Debug.LogError("Conversion Failed: " + this[key] + "is not a double");
                return -1;
            }
        }
        /// <summary>
        /// get a double parameter by <paramref name="key"/>
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public UUID GetUUIDParam(string key)
        {
            try
            {
                return Guid.Parse(string.IsNullOrEmpty(this[key]) ? Guid.Empty.ToString() : this[key]);
            }
            catch
            {
                Debug.LogError("Conversion Failed: " + this[key] + "is not a uuid");
                return Guid.Empty;
            }
        }

        /// <summary>
        /// Get a enum type <typeparamref name="T"/> params by <paramref name="key"/>
        /// </summary>
        /// <typeparam name="T">enum type</typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T GetEnumParam<T>(string key) where T : Enum
        {
            try
            {
                return (T)Enum.Parse(typeof(T), this[key]);
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return default;
            }
        }

        /// <summary>
        /// Check effect has a arg with <paramref name="key"/>
        /// </summary>
        /// <param name="key">key of the arg</param>
        /// <returns></returns>
        public bool HasParam(string key)
        {
            return string.IsNullOrEmpty(this[key]);
        }

        /// <summary>
        /// Check effect has a arg {<paramref name="key"/>: <paramref name="value"/>}
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool HasParam(string key, string value)
        {
            return this[key] == value;
        }




        public Args() : base()
        {

        }

        public new string this[string index]
        {
            get => string.IsNullOrEmpty(base[index].Value) ? null : base[index].Value;
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
