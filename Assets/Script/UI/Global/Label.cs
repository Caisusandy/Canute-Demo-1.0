using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public class Label : MonoBehaviour
    {
        public Text text;
        public Image image;
        public bool tryInsertEnter = true;
        public HorizontalLayoutGroup layoutGroup;

        public void Start()
        {
            if (GetComponent<Canvas>())
            {
                GetComponent<Canvas>().overrideSorting = true;
                GetComponent<Canvas>().sortingLayerName = "UI";
                GetComponent<Canvas>().sortingOrder = 10000;
            }
            if (tryInsertEnter)
                for (int i = 0, j = 0; i < text.text.Length; i++, j++)
                {
                    if (text.text[i].ToString() == "\n")
                        j = 0;
                    if (j < 40)
                        continue;
                    if (text.text[i].ToString() == " " && text.text.Length - i > 5)
                    {
                        text.text = text.text.Insert(i, "\n");
                        j = 0;
                    }
                }
        }

        public static Label GetLabel()
        {
            return Instantiate(GameData.Prefabs.Get("label")).GetComponent<Label>();
        }
        public static Label GetLabel(Transform transform)
        {
            return Instantiate(GameData.Prefabs.Get("label"), transform).GetComponent<Label>();
        }
        public static Label GetLabelNoCanvas()
        {
            var l = Instantiate(GameData.Prefabs.Get("label")).GetComponent<Label>();
            Destroy(l.GetComponent<Canvas>());
            return l;
        }
        public static Label GetLabelNoCanvas(Transform transform)
        {
            var l = Instantiate(GameData.Prefabs.Get("label"), transform).GetComponent<Label>();
            Destroy(l.GetComponent<Canvas>());
            return l;
        }
    }
}