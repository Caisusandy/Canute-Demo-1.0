using Canute.BattleSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LanguageAddress
{
    public const string BattleSystem = "Canute.BattleSystem";
    public const string Story = "";
}

namespace Canute.Languages
{
    public enum LanguageName
    {
        zh_cn = SystemLanguage.ChineseSimplified,
        en_us = SystemLanguage.English,
    }

    [CreateAssetMenu(fileName = "Lang", menuName = "Game Data/Language")]
    public class Language : ScriptableObject
    {

        [ContextMenuItem("Reload language pack", "ForceLoadLang")]
        [SerializeField] protected Args dictionary;
        [SerializeField] protected LanguageName dicLang;

        public LanguageName GameLanguage => Game.Configuration is null ? LanguageName.en_us : Game.Language;
        public Args Dictionary { get => dictionary; set => dictionary = value; }

        /// <summary>
        /// 语言文件解释器, 加载语言包
        /// </summary>
        public IEnumerator LoadLang()
        {
            if (dicLang == GameLanguage && dictionary != null)
            {
                yield return null;
            }
            dicLang = GameLanguage;

            Debug.Log("loading language pack " + dicLang);
            TextAsset LanguagePack = Resources.Load("Lang/" + GameLanguage.ToString()) as TextAsset;
            yield return LanguagePack;
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
                if (!dictionary.ContainsKey(word[0]))
                {
                    dictionary.Add(word[0], word[1]);
                }
            }
        }
        /// <summary>
        /// 强行加载语言包
        /// </summary>
        public void ForceLoadLang()
        {
            dictionary.Clear();
            dicLang = GameLanguage;
            Debug.Log("loading language pack " + dicLang);
            TextAsset LanguagePack = Resources.Load("Lang/" + GameLanguage.ToString()) as TextAsset;
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
                if (!dictionary.ContainsKey(word[0]))
                {
                    //Debug.Log("Add " + word[0] + ":" + word[1]);
                    dictionary.Add(word[0], word[1]);
                }
            }
        }
    }

    public static class Languages
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

        //#region Reflect-Lang

        ///// <summary>
        ///// 获取全局地址
        ///// </summary>
        ///// <param name="member"></param>
        ///// <returns></returns>
        //public static string GetKeyName(this MemberInfo member)
        //{
        //    return (member.DeclaringType.FullName + '.' + member.Name).Lang();
        //}

        ///// <summary>
        ///// 获取全局地址
        ///// </summary>
        ///// <param name="instance"></param>
        ///// <param name="memberName"></param>
        ///// <returns></returns>
        //public static string GetKeyName(this object instance, string memberName)
        //{
        //    var members = instance.GetType().GetMember(memberName);
        //    Debug.Log(members.Length);
        //    if (members.Length == 1)
        //    {
        //        return members[0].GetKeyName();
        //    }

        //    foreach (var member in members)
        //    {
        //        if (member.DeclaringType == instance.GetType())
        //        {
        //            return member.GetKeyName();
        //        }
        //    }

        //    //{
        //    //    throw new InvalidCastException("member name is incorrect");
        //    //}
        //    return default;
        //}

        //#endregion



        //public static string Lang<T>(T instance) where T : class
        //{
        //    string info = string.Empty;
        //    FieldInfo[] fields = instance.GetType().GetFields();
        //    PropertyInfo[] properties = instance.GetType().GetProperties();

        //    for (int i = 0; i < fields.Length; i++)
        //    {
        //        FieldInfo fieldInfo = fields[i];
        //        info += fieldInfo.GetKeyName() + ":" + fieldInfo.GetValue(instance) + "\n";
        //    }

        //    for (int i = 0; i < fields.Length; i++)
        //    {
        //        PropertyInfo propertyInfo = properties[i];
        //        info += propertyInfo.GetKeyName() + ":" + propertyInfo.GetValue(instance) + "\n";
        //    }
        //    return info;
        //}

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