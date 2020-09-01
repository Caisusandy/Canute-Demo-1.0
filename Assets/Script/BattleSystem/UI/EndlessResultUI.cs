using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class EndlessResultUI : EndPanel
    {
        public int playerScore;

        public Text title;
        public Text subtitle;

        public Text score;


        public Text fg;
        public Text mp;
        public Text ma;

        // Start is called before the first frame update
        public override void Start()
        {
            playerScore = Game.CurrentBattle.ScoreBoard.GetScore();
            ShowTitle();
            ShowScore();

            BattleUI.SetUIInteractive(false);
            OnMapEntity.SetAllEntityCollider(false);
        }

        public void ShowScore()
        {
            score.text = playerScore.ToString();
        }

        public void ShowPrize(Prize fgPrize, Prize mpPrize, Prize maPrize)
        {
            Debug.Log(fgPrize.Count);
            fg.text = fgPrize.Count.ToString();
            mp.text = mpPrize.Count.ToString();
            ma.text = maPrize.Count.ToString();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void ShowTitle()
        {
            title.text = Game.CurrentLevel.Lang("title");
            subtitle.text = Game.CurrentLevel.Lang("subtitle");
        }


    }
}