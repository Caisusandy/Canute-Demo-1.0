using Canute.BattleSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.UI
{
    public class AttackTypeIcon : Icon
    {
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Label.text = Languages.LanguageSystem.Lang(armyItem.AttackType);
        }

        public override void SetArmyItem(ArmyItem armyItem)
        {
            base.SetArmyItem(armyItem);
            //IconImage.sprite = GameData.SpriteLoader.Get(SpriteAtlases.armyTypeIcon, armyItem.Type + "WBG");
        }
    }
}