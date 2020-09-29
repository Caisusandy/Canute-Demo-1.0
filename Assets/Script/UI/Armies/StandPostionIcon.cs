using Canute.BattleSystem;

namespace Canute.UI
{
    public class StandPostionIcon : ArmyPropertyInfoIcon
    {
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Label.text = army.Properties.Lang("StandPosition") + ": " + Languages.Lang(army.Properties.StandPosition);
        }

        public override void SetArmyItem(IArmy armyItem)
        {
            base.SetArmyItem(armyItem);
            IconImage.sprite = GameData.SpriteLoader.Get(SpriteAtlases.armyPositionIcon, armyItem.Properties.StandPosition.ToString());
        }
    }
}