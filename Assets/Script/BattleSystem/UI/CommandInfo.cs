using System;
using UnityEngine;

namespace Canute.Testing
{
    [Serializable]
    public class CommandInfo : INameable, IEquatable<CommandInfo>
    {
        private const char parameterSeparator = ',';

        [SerializeField] private string name;
        [SerializeField] private CommandParameter[] @params;

        public string Name => name;
        public CommandParameter[] Params { get => @params; set => @params = value; }



        public bool Equals(CommandInfo command)
        {
            return command.name == name && command.Params.Equals(Params);
        }

        public override bool Equals(object obj)
        {
            return !(obj is CommandInfo) ? false : Equals(obj as CommandInfo);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(CommandInfo left, CommandInfo right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CommandInfo left, CommandInfo right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            string ret = "/";
            ret += name;
            foreach (var item in Params)
            {
                ret += ",";
                ret += item.ToString();
            }
            return ret;
        }
    }
}