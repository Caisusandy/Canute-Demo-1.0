using Canute.BattleSystem;
using Canute.Languages;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;

namespace Canute.UI
{
    public class ArmyTypeIcon : Icon
    {
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Label.text = Languages.LanguageSystem.Lang(armyItem.Type);
        }

        public override void SetArmyItem(ArmyItem armyItem)
        {
            base.SetArmyItem(armyItem);
            IconImage.sprite = GameData.SpriteLoader.Get(SpriteAtlases.armyTypeIcon, armyItem.Type + "WBG");
        }
    }

    [RequireComponent(typeof(Image))]
    public abstract class Icon : MonoBehaviour
    {
        [HideInInspector] public GameObject label;
        [HideInInspector] public ArmyItem armyItem;

        protected Image IconImage => GetComponent<Image>();
        protected Text Label => label.GetComponent<Label>().text;

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

        public virtual void DisplayInfo()
        {
            if (!label)
            {
                label = Instantiate(GameData.Prefabs.Get("label"), transform);
                label.transform.localPosition = new Vector3(30, 30, 0);
            }
            label.SetActive(true);
        }

        public virtual void HideInfo()
        {
            label.SetActive(false);
        }

        public virtual void SetArmyItem(ArmyItem armyItem)
        {
            this.armyItem = armyItem;
        }
    }
}