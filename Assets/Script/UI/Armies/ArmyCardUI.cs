using Canute.BattleSystem;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public delegate void ArmySelection(ArmyItem armyCardUI);
    public class ArmyCardUI : MonoBehaviour
    {
        public static ArmySelection selectEvent;
        public Image frame;
        public Image portrait;
        public Image careerIcon;
        public Image typeIcon;
        public Text level;
        public Text armyName;

        [HideInInspector] public ArmyItem displayingArmy;

        public void Display(ArmyItem armyItem)
        {  // display the army item
            displayingArmy = armyItem;
            if (displayingArmy)
            {
                portrait.sprite = armyItem.Prototype.Portrait;
                portrait.color = Color.white;
                armyName.text = armyItem.DisplayingName;
                careerIcon.sprite = GameData.SpriteLoader.Get(SpriteAtlases.careerIcon, armyItem.Career.ToString());
                typeIcon.sprite = GameData.SpriteLoader.Get(SpriteAtlases.armyTypeIcon, armyItem.Type.ToString());
                level.text = armyItem.Level.ToString();
            }
            else
            {
                portrait.sprite = null;
                portrait.color = Color.grey;
                armyName.text = "";
                careerIcon.sprite = GameData.SpriteLoader.Get(SpriteAtlases.careerIcon, Career.none.ToString());
                typeIcon.sprite = GameData.SpriteLoader.Get(SpriteAtlases.armyTypeIcon, Army.Types.none.ToString());
                level.text = "";
            }

        }

        public void Select()
        { 
            selectEvent?.Invoke(displayingArmy);
            ArmyListUI.SelectEvent?.Invoke(displayingArmy);
        }
    }
}