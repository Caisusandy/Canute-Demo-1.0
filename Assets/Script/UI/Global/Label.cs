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
        public HorizontalLayoutGroup layoutGroup;

        public static Label GetLabel()
        {
            return Instantiate(GameData.Prefabs.Get("label")).GetComponent<Label>();
        }
        public static Label GetLabel(Transform transform)
        {
            return Instantiate(GameData.Prefabs.Get("label"), transform).GetComponent<Label>();
        }
    }
}