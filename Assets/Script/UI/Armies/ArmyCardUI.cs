using Canute.BattleSystem;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public delegate void ArmyCardSelection(ArmyCardUI armyCardUI);
    public class ArmyCardUI : MonoBehaviour
    {
        public ArmyCardSelection selectEvent;
        public Image frame;
        public Image portrait;
        public Image careerIcon;
        public Image typeIcon;
        public Text level;
        public Text armyName;

        [HideInInspector] public ArmyItem displayingArmy;

        public void Display(ArmyItem armyItem)
        {
            displayingArmy = armyItem;

            //TODO get portrait
            //portrait.sprite = armyItem.Portrait;
            armyName.text = armyItem.DisplayingName;
            careerIcon.sprite = GameData.SpriteLoader.Get(SpriteAtlases.careerIcon, armyItem.Career.ToString());
            typeIcon.sprite = GameData.SpriteLoader.Get(SpriteAtlases.armyTypeIcon, armyItem.Type.ToString());
            level.text = armyItem.Level.ToString();
        }

        public void Select()
        {
            selectEvent?.Invoke(this);
            ArmyListUI.CardSelection?.Invoke(this);
        }
    }
}