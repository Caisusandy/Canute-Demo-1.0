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
}