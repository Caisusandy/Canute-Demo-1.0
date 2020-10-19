using Canute.BattleSystem;
using UnityEngine;

namespace Canute.UI
{
    [Obsolete]
    public class AttackPostionIcon : ArmyPropertyInfoIcon
    {
        [Obsolete]
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Label.text = army.Properties.Lang("AttackPosition") + ": " + army.Properties.AttackPosition.Lang();
        }

        public override void SetArmyItem(IArmy armyItem)
        {
            base.SetArmyItem(armyItem);
            //Debug.Log(armyItem.Properties.AttackPosition);
            IconImage.sprite = GameData.SpriteLoader.Get(SpriteAtlases.armyPositionIcon, armyItem.Properties.AttackPosition.ToString());
        }
    }
}