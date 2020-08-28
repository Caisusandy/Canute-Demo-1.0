using Canute.BattleSystem;
using Canute.BattleSystem.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Canute.Testing
{
    public class GameDebug : BattleUIBase
    {
        public Text displayer;

        public override void Awake()
        {
            enabled = Game.Configuration.IsDebugMode;
            base.Awake();
            if (!Game.Configuration.IsDebugMode)
                Destroy(gameObject);

        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            ShowDisplayer();
        }

        private void ShowDisplayer()
        {
            string ret = "";
            ret += "Round Stat: " + Battle.Round.CurrentStat + "\n";
            ret += "Game Stat: " + Battle.CurrentStat + "\n";
            ret += "Current Player: " + Game.CurrentBattle.Round.CurrentPlayer.Name + "\n";
            ret += "Motion: " + Module.Motion.ongoingMotions.Count + "\n";
            ret += "Animation: " + Game.CurrentBattle.OngoingAnimation.Count + "\n";
            displayer.text = ret;
        }
    }
}