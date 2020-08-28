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
        public static Dictionary<string, string> Dictionary { get => LanguageSystem.Dictionary; set => LanguageSystem.Dictionary = value; }

        /// <summary>
        /// 语言文件解释器, 加载语言包
        /// </summary>
        public IEnumerator LoadLang()
        {
            Debug.Log("loading language pack " + Game.Language);
            TextAsset LanguagePack = Resources.Load("Lang/" + Game.Language) as TextAsset;
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
                if (!Dictionary.ContainsKey(word[0]))
                {
                    Dictionary.Add(word[0], word[1]);
                }
            }
        }
        /// <summary>
        /// 强行加载语言包
        /// </summary>
        public void ForceLoadLang()
        {
            Debug.Log("loading language pack " + Game.Language);
            Dictionary = new Dictionary<string, string>();
            TextAsset LanguagePack = Resources.Load("Lang/" + Game.Language) as TextAsset;
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
    }
}