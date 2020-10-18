using System.Linq;
using UnityEngine;

namespace System.Collections.Generic
{
    [Serializable]
    [Obsolete]
    public class KeyValuePairs<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] protected bool allowSerialization = true;
        [SerializeField] protected List<TKey> keys = new List<TKey>();
        [SerializeField] protected List<TValue> values = new List<TValue>();

        [Obsolete]
        public new ICollection<TKey> Keys => keys.Union(base.Keys).ToList();

        [Obsolete]
        public new ICollection<TValue> Values => values.Union(base.Values).ToList();

        [Obsolete]
        public new int Count => keys.Count + base.Keys.Count;

        [Obsolete]
        public new TValue this[TKey key] { get { Reorganize(); return base[key]; } set { base[key] = value; Reorganize(); } }

        [Obsolete]
        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (!allowSerialization)
            {
                return;
            }

            base.Clear();
#if !UNITY_EDITOR
                    if (keys.Count != values.Count)
                    {
                        throw new InValidKeyValuePairException();
                    }
#endif
            for (int i = 0; i < keys.Count; i++)
            {
                if (values.Count < i && !ContainsKey(keys[i]))
                {
                    Add(keys[i], values[i]);
                    Debug.Log(keys[i] + ": " + values[i]);
                }
                else
                {
                    break;
                }
            }
        }

        [Obsolete]
        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            foreach (KeyValuePair<TKey, TValue> keyValuePair in this)
            {
                if (!keys.Contains(keyValuePair.Key))
                {
                    keys.Add(keyValuePair.Key);
                    values.Add(keyValuePair.Value);
                }

            }
        }

        public void Reorganize()
        {
            if (!allowSerialization)
            {
                return;
            }

            for (int i = 0; i < keys.Count; i++)
            {
                if (values.Count < i && !ContainsKey(keys[i]))
                {
                    Add(keys[i], values[i]);
                    Debug.Log(keys[i] + ": " + values[i]);
                }
                else
                {
                    break;
                }
            }

            keys.Clear();
            values.Clear();

            foreach (var item in this)
            {
                keys.Add(item.Key);
                values.Add(item.Value);
            }
        }

        public new void Add(TKey key, TValue value)
        {
            base.Add(key, value);
        }

        public new bool ContainsKey(TKey key)
        {
            Reorganize();
            return base.ContainsKey(key);
        }

        [Obsolete]
        public new bool Remove(TKey key)
        {
            Reorganize();
            return base.Remove(key);
        }

        [Obsolete]
        public new bool TryGetValue(TKey key, out TValue value)
        {
            Reorganize();
            return base.TryGetValue(key, out value);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            Add(item.Key, item.Value);
        }

        [Obsolete]
        public new void Clear()
        {
            base.Clear();
            values.Clear();
            keys.Clear();
        }

        [Obsolete]
        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            foreach (var pair in this)
            {
                if (pair.Key.Equals(item.Key) && pair.Value.Equals(item.Value))
                {
                    return true;
                }
            }
            return false;
        }


        [Obsolete]
        public KeyValuePairs() : base()
        {

        }

        [Obsolete]
        public KeyValuePairs(IDictionary<TKey, TValue> keyValuePairs)
        {
            foreach (var item in keyValuePairs)
            {
                Add(item);
            }
        }

    }

    public class InValidKeyValuePairException : Exception
    {
        public InValidKeyValuePairException() { }
        [Obsolete]
        public InValidKeyValuePairException(string message) : base(message) { } 
        [Obsolete]
        public InValidKeyValuePairException(string message, Exception inner) : base(message, inner) { } 
       [Obsolete]
        protected InValidKeyValuePairException(
          Runtime.Serialization.SerializationInfo info,
          Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
