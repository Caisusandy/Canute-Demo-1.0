namespace Canute.UI
{
    public class StandPostionIcon : Icon
    {
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Label.text = Languages.LanguageSystem.Lang(armyItem.StandPosition);
        }

        public override void SetArmyItem(ArmyItem armyItem)
        {
            base.SetArmyItem(armyItem);
            //IconImage.sprite = GameData.SpriteLoader.Get(SpriteAtlases.armyTypeIcon, armyItem.Type + "WBG");
        }
    }
}