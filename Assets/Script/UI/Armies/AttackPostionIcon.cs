using UnityEngine;

namespace Canute.UI
{
    public class AttackPostionIcon : Icon
    {
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Debug.Log(armyItem.AttackPosition);
            Label.text = Languages.Languages.Lang(armyItem.AttackPosition);
        }

        public override void SetArmyItem(ArmyItem armyItem)
        {
            base.SetArmyItem(armyItem);
            //IconImage.sprite = GameData.SpriteLoader.Get(SpriteAtlases.armyTypeIcon, armyItem.Type + "WBG");
        }
    }
}