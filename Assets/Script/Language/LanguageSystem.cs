using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute
{
    public static class Languages
    {
        public static string Language => Game.Language;
        public static Dictionary<string, string> Dictionary { get => dictionary; set => dictionary = value; }

        [ContextMenuItem("Reload language pack", "ForceLoadLang")]
        private static Dictionary<string, string> dictionary;

        /// <summary>
        /// 强行加载语言包
        /// </summary>
        public static void ForceLoadLang()
        {
            Debug.Log("loading language pack " + Game.Language);
            Dictionary = new Dictionary<string, string>();
            TextAsset LanguagePack = Resources.Load("Lang/" + Game.Language) as TextAsset;
            if (!LanguagePack)
            {
                Debug.LogError("Language pack not found: " + Game.Language);
                LanguagePack = Resources.Load("Lang/en_us") as TextAsset;
            }
            string lang = LanguagePack.text;
            string[] langtoarray = lang.Split('\n');
            foreach (string wordset in langtoarray)
            {
                if (wordset.StartsWith("#") || wordset.StartsWith("//"))
                {
                    continue;
                }
                if (!wordset.Contains("="))
                {
                    continue;
                }
                string[] word = wordset.Split('=');
                if (!Dictionary.ContainsKey(word[0]))
                {
                    //Debug.Log("Add " + word[0] + ":" + word[1]);
                    Dictionary.Add(word[0], word[1]);
                }
            }
        }

        #region 基本

        /// <summary>
        /// Get the matched display language from dictionary by key
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string Lang(this string key, LangType type = LangType.RawDisplay)
        {
            try
            {
                return Dictionary[key];
            }
            catch (KeyNotFoundException)
            {
                Debug.LogWarning(key);
                //Debug.LogWarning(key);
                string[] vs = key.Split('.');
                if (vs[vs.Length - 1] == "name" || vs[vs.Length - 1] == "info")
                {
                    return vs[vs.Length - 2];
                }
                return vs[vs.Length - 1];
            }
        }

        /// <summary>
        /// Get the matched display language from dictionary from a instance and a parameter
        /// </summary> 
        /// <param name="param"></param>
        /// <returns></returns> 

        public static string Lang(params string[] param)
        {
            string fullName = "";

            if (param.Length >= 1)
            {
                fullName = param[0];
            }

            for (int i = 1; i < param.Length; i++)
            {
                fullName += "." + param[i];
            }

            return fullName.Lang();
        }


        /// <summary>
        /// Get the matched display language from dictionary from a instance and a parameter
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="instance"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        /// 
        public static string Lang<T>(this T instance, params string[] param) where T : INameable
        {
            List<string> vs = new List<string>() { instance.GetFullTypeName() + "." + instance.Name };
            vs.AddRange(param);
            return Lang(vs.ToArray());
        }

        /// <summary>
        /// Get the matched display language from dictionary from a instance and a parameter
        /// </summary> 
        /// <param name="instance"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        /// 
        public static string Lang(this object instance, string name, params string[] param)
        {
            List<string> vs = new List<string>() { instance.GetFullTypeName() + "." + name };
            vs.AddRange(param);
            return Lang(vs.ToArray());
        }

        public static string Lang(this PropertyBonus propertyBonus, int level = 1)
        {
            string ret = "";
            foreach (var item in PropertyTypes.Types)
            {
                if ((item & propertyBonus.Type) != PropertyType.none)
                {
                    List<string> vs = new List<string>() { propertyBonus.GetFullTypeName() + "." + (item & propertyBonus.Type) };
                    var raw = Lang(vs.ToArray());
                    int value = propertyBonus.GetValue(level);

                    ret += " ";
                    ret += value >= 0 ? "+" : "-";
                    ret += value;
                    ret += propertyBonus.BonusType == BonusType.percentage ? "% " : " ";
                    ret += raw + " |";
                }
            }
            return ret.Remove(ret.Length - 1).Remove(0, 1);

        }

        /// <summary>
        /// Get the matched display language from dictionary from a instance and a parameter
        /// </summary>
        /// <typeparam name="T"></typeparam> 
        /// <param name="param"></param>
        /// <returns></returns>
        /// 
        public static string Lang<T>(params string[] param)
        {
            List<string> vs = new List<string>() { GetFullTypeName<T>() };
            vs.AddRange(param);
            return Lang(vs.ToArray());
        }

        //public static string Lang<T>(params string[] param) where T : class
        //{
        //    string suffix = string.Empty;
        //    foreach (string part in param)
        //    {
        //        suffix += "." + part;
        //    }
        //    string fullName = T.GetFullTypeName() + suffix;

        //    try
        //    {
        //        return Dictionary[fullName];
        //    }
        //    catch (KeyNotFoundException)
        //    {
        //        return fullName;
        //    }
        //}

        public static string Lang<T>(this T instance) where T : Enum
        {
            string key = instance.GetFullTypeName() + "." + instance.ToString();
            try
            {
                return Dictionary[key];
            }
            catch (KeyNotFoundException)
            {
                string[] vs = key.Split('.');
                if (vs[vs.Length - 1] == "name" || vs[vs.Length - 1] == "info")
                {
                    return vs[vs.Length - 2];
                }
                return vs[vs.Length - 1];
            }
        }



        //public static string EnglishPretenting(string word, LangType type)
        //{
        //    if (type == LangType.AllowEmpty)
        //    {
        //        return string.Empty;
        //    }
        //    else if (type == LangType.RawDisplay)
        //    {
        //        return word;
        //    }
        //    else if (Language == LanguageSetting.en_us)
        //    {
        //        string[] Word = word.Split('.');
        //        return Word[Word.Length - 1] == "info" ? Word[Word.Length - 2] : Word[Word.Length - 1];
        //    }
        //    return word;
        //}

        public static string GetFullTypeName<T>(this T instance)
        {
            return instance.GetType().FullName;
        }
        public static string GetFullTypeName<T>()
        {
            T a = default;
            return a.GetType().FullName;
        }
        #endregion

        #region Effect and Status 
        public static string GetDisplayingName(this IEffect effect)
        {
            return Lang<Effect.Types>(effect.Name);
        }

        public static string GetDisplayingName(this Status stat)
        {
            return stat.Effect.GetDisplayingName();
        }

        public static string Info(this IEffect effect)
        {
            string ret = "";
            ret += Lang<Effect.Types>(effect.Name, "info");

            ret = ret.Replace("@count", Math.Abs(effect.Count).ToString());
            ret = ret.Replace("@param", Math.Abs(effect.Parameter).ToString());

            foreach (var item in effect.Args)
            {
                ret = ret.Replace("@" + item.Key, item.Value);
            }

            return ret;
        }
        public static string Info(this Effect effect)
        {
            string ret = "";
            ret += Lang<Effect.Types>(effect.Name, "info");

            ret = ret.Replace("@count", Math.Abs(effect.Count).ToString());
            ret = ret.Replace("@param", Math.Abs(effect.Parameter).ToString());

            foreach (var item in effect.Args)
            {
                ret = ret.Replace("@" + item.Key, item.Value);
            }
            ret = ret.Replace("$sourceName", effect.Source?.Name);
            ret = ret.Replace("$targetName", effect.Target?.Name);

            return ret;
        }
        public static string Info(this Status status)
        {
            string ret = status.Effect.GetDisplayingName() + "\n" + status.Effect.Info();

            ret += "\n";

            //if (status.IsResonance)
            //{
            //    ret += "Resonance\n";
            //}
            if (status.IsDualBase)
            {
                ret += "Turn: " + status.TurnCount + ", Status Count: " + status.StatCount + "\n";
            }
            else if (status.IsBaseOnTurn)
            {
                ret += "Turn: " + status.TurnCount + "\n";
            }
            else if (status.IsBaseOnCount)
            {
                ret += "Status Count: " + status.StatCount + "\n";
            }
            else if (status.Type == Status.StatType.delay)
            {
                ret += "Remaining Turn: " + status.StatCount + "\n";
            }

            return ret;
        }

        public static string Name(this IEnumerable<Effect.Types> words)
        {
            string ret = "";
            foreach (Enum @enum in words)
            {
                ret += @enum.Lang() + "\n";
            }
            return ret;
        }

        #endregion

        #region 故事

        #endregion

        public enum LangType
        {
            ForceDisplay,
            RawDisplay,
            AllowEmpty,
        }
    }
}