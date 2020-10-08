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
            BattleUI.ChangePlayerUI(Player, false);

            enabled = false;
            gameObject.SetActive(false);
            transform.parent.gameObject.SetActive(false);
        }
    }
}