using UnityEngine;

namespace Canute.UI
{
    public class AttackPostionIcon : Icon
    {
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Label.text = Languages.LanguageSystem.Lang(armyItem.AttackPosition);
        }

        public override void SetArmyItem(ArmyItem armyItem)
        {
            base.SetArmyItem(armyItem);
            //IconImage.sprite = GameData.SpriteLoader.Get(SpriteAtlases.armyTypeIcon, armyItem.Type + "WBG");
        }
    }
}