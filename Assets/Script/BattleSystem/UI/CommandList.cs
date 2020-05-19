using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.Testing
{
    [Serializable]
    public class CommandList : DataList<Command>
    {
    }

    [Serializable]
    public struct Command : INameable
    {
        public string name;
        public CommandParameter[] @params;

        public string Name => name;

        public override bool Equals(object obj)
        {
            if (obj is Command)
            {
                return ((Command)obj).name == name && ((Command)obj).@params.Equals(@params);
            }
            else return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Command left, Command right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Command left, Command right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            string ret = "/";
            ret += name;
            foreach (var item in @params)
            {
                ret += ",";
                ret += Brackets(item, 0);
                ret += item.type.ToString();
                ret += " ";
                ret += item.key;
                if (!string.IsNullOrEmpty(item.value))
                {
                    ret += "=";
                    ret += item.value;
                }
                ret += Brackets(item, 1);
            }
            return ret;
        }

        private static string Brackets(CommandParameter item, int side)
        {
            string intType = "[]";
            string stringType = "<>";
            string jsonType = "{}";
            switch (item.type)
            {
                case CommandParameter.Type.@int:
                case CommandParameter.Type.ints:
                    return intType[side].ToString();
                case CommandParameter.Type.@string:
                case CommandParameter.Type.strings:
                    return stringType[side].ToString();
                case CommandParameter.Type.json:
                    return jsonType[side].ToString();
                default:
                    return "";
            }
        }
    }
    [Serializable]
    public struct CommandParameter : INameable
    {
        public enum Type
        {
            @int,
            @string,
            @ints,
            @strings,
            json,
        }

        [SerializeField] public Type type;
        [SerializeField] public string key;
        [SerializeField] public string value;

        public string Name => key;

        public CommandParameter(Type type, string key, string value)
        {
            this.type = type;
            this.key = key;
            this.value = value;
        }


        public bool Equals(CommandParameter other)
        {
            return other.value == value && other.key == key && other.type == type;
        }

        public override bool Equals(object obj)
        {
            return obj is CommandParameter ? Equals((CommandParameter)obj) : false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(CommandParameter left, CommandParameter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CommandParameter left, CommandParameter right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return '{' + key + ": " + value + '}';
        }
    }
}