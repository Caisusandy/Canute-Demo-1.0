using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem.UI
{
    public class EnemyHandCardBar : HandCardBar
    {
        public override Player Player => Battle.Enemy;

        public override void Start()
        {
            base.Start();
            BattleUI.SetPlayerUI(Player, false);

            enabled = false;
            gameObject.SetActive(false);
            transform.parent.gameObject.SetActive(false);
        }

        public override void OnMouseDown()
        {
#if UNITY_EDITOR
            Game.CurrentBattle.GetHandCard(Game.CurrentBattle.Enemy, 1);
#endif
        }
    }
}