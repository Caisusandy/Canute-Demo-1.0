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
            Debug.Log(armyItem.AttackPosition.ToString());
            IconImage.sprite = GameData.SpriteLoader.Get(SpriteAtlases.armyPositionIcon, armyItem.AttackPosition.ToString());

        }
    }
}