using Canute.Languages;
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
        public List<LanguagePair> languagePairs;

        public void Start()
        {
            foreach (var item in languagePairs)
            {
                item.textField.text = item.key.Lang();
            }
        }
    }

    public struct LanguagePair
    {
        public Text textField;
        public string key;
    }
}
