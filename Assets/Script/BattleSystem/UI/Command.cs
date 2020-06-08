using System;
using Canute;
using UnityEngine;

namespace Canute.Testing
{
    public static class Command
    {
        /// <summary>
        /// Execute a command
        /// </summary>
        /// <param name="command">full string of the command <para>/<paramref name="command"/> [parameter: default value]</para></param>
        /// <returns></returns>
        public static bool Execute(string command)
        {
            if (!command.StartsWith("/"))
            {
                Console.WriteLine(command);
                return true;
            }

            string[] commandParams = command.Remove(0, 1).Split(',');

            switch (commandParams[0])
            {
                case "print":
                    Console.WriteLine(commandParams[1]);
                    Debug.Log(commandParams[1]);
                    return true;
                case "getArmyItem":
                    string name = commandParams[1];
                    BattleSystem.Army army = GameData.Prototypes.GetArmyPrototype(name);
                    if (!army)
                    {
                        Console.WriteLine("the army prototype is not exist!");
                    }
                    ArmyItem armyItem = new ArmyItem(army, 250000);
                    Game.PlayerData.AddArmyItem(armyItem);
                    PlayerFile.SaveCurrentData();
                    return true;
                default:
                    Console.WriteLine("Command not found: " + commandParams[0]);
                    break;
            }

            return false;
        }



        private static bool ToBool(out bool value, string input)
        {
            try
            {
                value = bool.Parse(input);
                return true;
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }

        private static bool ToInt(out int value, string input)
        {
            try
            {
                value = int.Parse(input);
                return true;
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }

        private static bool ToDouble(out double value, string input)
        {
            try
            {
                value = double.Parse(input);
                return true;
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }

        private static bool ToObject<T>(out T value, string json)
        {
            try
            {
                value = JsonUtility.FromJson<T>(json);
                return true;
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }
    }


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


    [Serializable]
    public struct CommandParameter : INameable
    {
        public enum ParameterType
        {
            /// <summary> single on map entity </summary>
            onMapEntity,
            /// <summary> single int param </summary>
            @int,
            /// <summary> single string param </summary>
            @string,
            /// <summary> single double param </summary>
            @double,
            /// <summary> single bool param </summary>
            @bool,
            /// <summary> multiple int param </summary>
            @ints,
            /// <summary> multiple string param </summary>
            @strings,
            /// <summary> multiple double param </summary>
            @doubles,
            /// <summary> multiple bool param </summary>
            @bools,
            /// <summary> json param </summary>
            json,
        }

        /// <summary> name of the parameter </summary>
        [SerializeField] private string key;
        /// <summary> type of the parameter </summary>
        [SerializeField] private ParameterType type;
        /// <summary> default value of the parameter </summary>
        [SerializeField] private string @default;
        /// <summary> default value of the parameter </summary>
        [SerializeField] private string[] possibleValue;

        public string Name => Key;

        public ParameterType Parameter { get => type; set => type = value; }
        public string Key { get => key; set => key = value; }
        public string Default { get => @default; set => this.@default = value; }
        public string[] PossibleValue { get => possibleValue; set => possibleValue = value; }

        public CommandParameter(ParameterType type, string key, string @default)
        {
            this.type = type;
            this.key = key;
            this.@default = @default;
            this.possibleValue = Array.Empty<string>();
        }


        public bool Equals(CommandParameter other)
        {
            return other.Default == Default && other.Key == Key && other.Parameter == Parameter;
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

        private static string Brackets(CommandParameter item, int side)
        {
            string intType = "[]";
            string stringType = "<>";
            string jsonType = "{}";
            switch (item.type)
            {
                case ParameterType.@int:
                case ParameterType.ints:
                case ParameterType.@double:
                case ParameterType.doubles:
                case ParameterType.@bool:
                case ParameterType.bools:
                    return intType[side].ToString();
                case ParameterType.@string:
                case ParameterType.strings:
                    return stringType[side].ToString();
                case ParameterType.json:
                    return jsonType[side].ToString();
                default:
                    return "";
            }
        }

        public override string ToString()
        {
            return Brackets(this, 0) + Parameter.ToString() + ": " + Key + (string.IsNullOrEmpty(Default) ? "" : (" = " + Default)) + Brackets(this, 1);
        }
    }
}