using UnityEngine;
using UnityEngine.UI;
using Canute.BattleSystem;
using System;

namespace Canute.UI.Legion
{
    public class LSResonanceInfo : MonoBehaviour
    {
        public Text count;
        public Text type;

        public ResonancePair ResonancePair;
        public Label label;

        public void OnMouseOver()
        {
            DisplayInfo();
        }

        public void OnMouseDown()
        {
            DisplayInfo();
        }

        public void OnMouseExit()
        {
            HideInfo();
        }


        public void OnMouseUp()
        {
            HideInfo();
        }

        public void Display()
        {
            count.text = ResonancePair.Count.ToString();
            type.text = ResonancePair.ArmyType.Lang();
        }

        private void HideInfo()
        {
            Destroy(label?.gameObject);
        }

        public void DisplayInfo()
        {
            if (label)
            {
                return;
            }
            label = Instantiate(GameData.Prefabs.Get("label"), transform).GetComponent<Label>();
            label.text.text = ((Status)ResonancePair.Resonance).Info();
            var canvas = label.gameObject.AddComponent<Canvas>();

            canvas.overrideSorting = true;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 2;
        }
    }
}
