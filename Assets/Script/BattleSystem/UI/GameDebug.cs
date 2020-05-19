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
            enabled = GameData.BuildSetting.IsInDebugMode;
            base.Awake();
#if !UNITY_EDITOR
            Destroy(gameObject);
#endif
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
            displayer.text = ret;
        }
    }
}