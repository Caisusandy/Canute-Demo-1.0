namespace Canute.UI
{
    public class LSLeaderIcon : LeaderIcon
    {
        public void Select(LeaderIcon leaderIcon)
        {
            LSLegionPanel.instance.Select(leaderIcon);
        }
    }
}