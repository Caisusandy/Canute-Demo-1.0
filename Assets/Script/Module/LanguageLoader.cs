using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.Module
{
    public class LanguageLoader : MonoBehaviour
    {
        public static List<LanguageLoader> loaders = new List<LanguageLoader>();

        public List<LanguagePair> languagePairs;

        public void Start()
        {
            Load();
        }

        public void Load()
        {
            foreach (var item in languagePairs)
            {
                if (item.textField)
                    item.textField.text = item.key.Lang();
            }
        }

        public void Awake()
        {
            loaders.Add(this);
        }

        public void OnDestroy()
        {
            loaders.Remove(this);
        }

        public static void ReloadLanguage()
        {
            foreach (var item in loaders)
            {
                item?.Load();
            }
        }


    }

    [Serializable]
    public struct LanguagePair
    {
        public Text textField;
        public string key;
    }
}
