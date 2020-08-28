using System;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class RightPanel : BattleUIBase
    {
        public Text actionPointInfo;
        public Button endTurnButton;
        public Button cancleActionButton;
        public Button moveCheckButton;

        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            if (Player is null)
            {
                return;
            }

            ActionPointDisplay();
            EndTurnButtonDisplay();
            CancleActionDisplay();
            MoveStartButtonDisplay();
        }

        private void CancleActionDisplay()
        {
            cancleActionButton.gameObject.SetActive(Game.CurrentBattle.Round.CurrentPlayer == Player && Game.CurrentBattle.Round.CurrentStat == Round.Stat.action);
        }

        private void EndTurnButtonDisplay()
        {
            endTurnButton.gameObject.SetActive(Game.CurrentBattle.Round.CurrentPlayer == Player && Game.CurrentBattle.CurrentStat != Battle.Stat.begin);
        }

        private void MoveStartButtonDisplay()
        {
            moveCheckButton.gameObject.SetActive(Game.CurrentBattle.Round.CurrentPlayer == Player && Game.CurrentBattle.CurrentStat == Battle.Stat.move);
        }


        private void ActionPointDisplay()
        {
            actionPointInfo.text = Game.CurrentBattle.CurrentStat == Battle.Stat.begin ? "---" : "Action Point: " + Player.ActionPoint + " / 8";
        }

        public void CancelAction()
        {
            if (Game.CurrentBattle.CurrentStat == Battle.Stat.move || Game.CurrentBattle.CurrentStat == Battle.Stat.attack || Game.CurrentBattle.Round.CurrentStat == Round.Stat.action)
            {
                Game.CurrentBattle.CancelAction();
            }
        }

        public void MoveCheck()
        {
            if (Game.CurrentBattle.CurrentStat == Battle.Stat.move)
            {
                ArmyMovement.TryMove();
            }
        }

        public virtual void EndTurn()
        {
            if (!Game.CurrentBattle.TryEndTurn())
            {
                return;
            }
            BattleUI.SetDownBarsActive(false);
            endTurnButton.gameObject.SetActive(false);
        }
    }
}
