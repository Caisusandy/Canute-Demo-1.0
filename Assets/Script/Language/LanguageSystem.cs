using Canute.BattleSystem;
using System;
using System.Collections.Generic;

namespace Canute.Languages
{
    public static class LanguageSystem
    {
        public static LanguageName Language => Game.Language;
        public static Args Dictionary => GameData.Language.Dictionary;

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
            string v = instance.GetFullTypeName() + "." + instance.ToString();
            try
            {
                return Dictionary[v];
            }
            catch (KeyNotFoundException)
            {
                return v;
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

            if (status.IsResonance)
            {
                ret += "Resonance\n";
            }
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