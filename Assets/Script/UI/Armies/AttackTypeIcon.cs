using Canute.BattleSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.UI
{
    public class AttackTypeIcon : ArmyPropertyInfoIcon
    {
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Label.text = army.Properties.Lang("AttackType") + ": " + Languages.Lang(army.Properties.Attack);
        }

        public override void SetArmyItem(IArmy armyItem)
        {
            base.SetArmyItem(armyItem);
            //IconImage.sprite = GameData.SpriteLoader.Get(SpriteAtlases.armyTypeIcon, armyItem.Type + "WBG");
        }
    }
}