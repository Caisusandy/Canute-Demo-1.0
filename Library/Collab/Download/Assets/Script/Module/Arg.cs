using Canute;
using Canute.Module;
using UnityEngine;
using System.Linq;

namespace System.Collections.Generic
{
    [Serializable]
    public struct Arg : INameable, IEquatable<Arg>
    {
        [SerializeField] private string key;
        [SerializeField] private string value;

        public string Key { get => key; set => key = value; }
        public string Value { get => value; set => this.value = value; }
        public string Name => Key;

        public Arg(string key, string value)
        {
            this.key = key;
            this.value = value;
        }

        public static implicit operator KeyValuePair<string, string>(Arg arg)
        {
            return new KeyValuePair<string, string>(arg.key, arg.value);
        }

        public static implicit operator Arg(KeyValuePair<string, string> arg)
        {
            return new Arg(arg.Key, arg.Value);
        }

        public static implicit operator ArgType<string>(Arg arg)
        {
            return new ArgType<string>(arg.Key, arg.Value);
        }

        public static implicit operator Arg(string @string)
        {
            string[] vs;
            Arg arg;

            if (@string is null)
            {
                throw new ArgumentException();
            }

            if (@string.Split(':').Length > 2 || @string.Split(':').Length < 1)
            {
                throw new ArgumentException();
            }

            vs = @string.Split(':');
            arg = new Arg(vs[0], "");

            if (vs.Length > 1)
            {
                arg.value = vs[1];
            }
            return arg;
        }

        public bool Equals(Arg other)
        {
            return other.value == value && other.key == key;
        }

        public override bool Equals(object obj)
        {
            return obj is Arg ? Equals((Arg)obj) : false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Arg left, Arg right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Arg left, Arg right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return '{' + key + ": " + value + '}';
        }
    }

    [Serializable]
    public struct ArgList : IEnumerable<Arg>
    {
        [SerializeField] private Arg[] args;

        public string this[string index]
        {
            get { return Get(index); }
            set { Set(index, value); }
        }

        private string Get(string index)
        {
            foreach (var item in args)
            {
                if (item.Key == index)
                {
                    return item.Value;
                }
            }

            return null;
        }

        public List<string> FindAll(string key)
        {
            List<string> vs = new List<string>();
            foreach (var item in args)
            {
                if (item.Key == key)
                {
                    vs.Add(item.Value);
                }
            }

            return vs;
        }

        private void Set(string index, string value)
        {
            for (int i = 0; i < args.Length; i++)
            {
                Arg item = args[i];
                if (item.Key == index)
                {
                    if (value is null)
                        Remove(item);
                    else
                        args[i].Value = value;
                    return;
                }
            }

            Add(index, value);
        }

        public ArgList(IEnumerable<Arg> args)
        {
            this.args = args.ToArray();
        }

        private void Remove(Arg i)
        {
            args = args.Except(new Arg[] { i }).ToArray();
        }

        private void Add(string index, string value)
        {
            args = args.Append(new Arg(index, value)).ToArray();
        }

        public IEnumerator<Arg> GetEnumerator()
        {
            return ((IEnumerable<Arg>)args).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Arg>)args).GetEnumerator();
        }

        public static implicit operator Args(ArgList args)
        {
            return new Args(args);
        }
    }
}