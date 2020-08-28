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
        public Text level;
        public Text armyName;
        public ArmyItem displayingArmy;

        public void Display(ArmyItem armyItem)
        {
            displayingArmy = armyItem;

            //TODO get portrait
            //portrait.sprite = armyItem.Portrait;
            armyName.text = armyItem.DisplayingName;
            level.text = "Lv." + armyItem.Level.ToString();
        }

        public void Selected()
        {
            selectEvent?.Invoke(this);
            ArmyListUI.CardSelection?.Invoke(this);
        }
    }
}