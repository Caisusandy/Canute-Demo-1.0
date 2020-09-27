using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class LanguageAddress
{
    public const string BattleSystem = "Canute.BattleSystem";
    public const string Story = "";
}

namespace Canute.LanguageSystem
{
    [Obsolete]
    [CreateAssetMenu(fileName = "Lang", menuName = "Other/[Obsolete] Language")]
    public class Language : ScriptableObject
    {
        public static Dictionary<string, string> Dictionary { get => Languages.Dictionary; set => Languages.Dictionary = value; }

        /// <summary>
        /// �����ļ�������, �������԰�
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
        /// ǿ�м������԰�
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