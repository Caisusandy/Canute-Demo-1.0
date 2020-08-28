namespace Canute.BattleSystem.UI
{
    public class EnemyRightPanel : RightPanel
    {
        public override Player Player => Game.CurrentBattle.Enemy;

        public override void EndTurn()
        {
            if (!Player.TryEndTurn())
            {
                return;
            }

            endTurnButton.gameObject.SetActive(false);
            actionPointInfo.gameObject.SetActive(false);
        }
    }
}
