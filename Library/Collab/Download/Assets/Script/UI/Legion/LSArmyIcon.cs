namespace Canute.UI
{
    public class LSArmyIcon : ArmyIcon
    {
        public void SelectArmy(ArmyIcon armyIcon)
        {
            LSLegionPanel.instance.Select(armyIcon);
        }
    }
}