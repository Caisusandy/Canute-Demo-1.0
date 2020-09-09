using Canute.BattleSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.U2D;

namespace Canute.UI
{
    public class ArmyTypeIcon : Icon
    {
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Label.text = Languages.Lang(army.Type);
        }

        public override void SetArmyItem(IArmy armyItem)
        {
            base.SetArmyItem(armyItem);
            IconImage.sprite = GameData.SpriteLoader.Get(SpriteAtlases.armyTypeIcon, armyItem.Type + "WBG");
        }
    }
}