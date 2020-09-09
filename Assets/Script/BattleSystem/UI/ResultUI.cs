using Canute.Shops;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class ResultUI : EndPanel
    {
        public int playerScore;

        public Text title;
        public Text subtitle;

        public Text score;
        public Text passed;

        public Text fg;
        public Text mp;
        public Text ma;

        public GameObject armyIcons;

        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            playerScore = Game.CurrentBattle.ScoreBoard.GetScore();

            ShowTitle();
            ShowScore();
            ShowPrize();
            ShowLevelPassStatus();
            ShowArmyExpIncrease();

            BattleUI.SetUIInteractive(false);
            OnMapEntity.SetAllEntityCollider(false);
        }

        public void ShowScore()
        {
            if (Game.CurrentBattle.BattleType == Battle.Type.endless)
                score.text = playerScore.ToString();
            else
                Destroy(score.gameObject);
        }

        public void ShowLevelPassStatus()
        {
            if (Game.CurrentBattle.CurrentStat == Battle.Stat.win)
                passed.text = "Canute.BattleSystem.UI.ResultUI.Passed".Lang();
            else
                passed.text = "Canute.BattleSystem.UI.ResultUI.Lost".Lang();
        }

        public void ShowArmyExpIncrease()
        {
            int exp = Game.CurrentBattle.Prizes.GetPrizes(Item.Type.exp).Sum((e) => e.Count);
            List<Transform> icons = new List<Transform>();
            foreach (Transform item in armyIcons.transform)
            {
                icons.Add(item);
            }

            for (int i = 0; i < Game.CurrentBattle.Player.LegionSet.Legion.RealArmyCount; i++)
            {
                var item = icons[i];
                ArmyItem armyItem = Game.CurrentBattle.Player.LegionSet.Legion.RealArmies.ToArray()[i];
                if (!armyItem)
                {
                    Destroy(icons[icons.Count - 1].gameObject);
                    continue;
                }
                item.Find("icon").GetComponent<Image>().sprite = armyItem.Icon;
                item.Find("lvl").GetComponent<Text>().text = "Lv." + armyItem.Level;
                item.Find("add").GetComponent<Text>().text = "+" + exp;
                item.Find("cur").GetComponent<Text>().text = armyItem.Exp + "/" + armyItem.NextLevelExp;
            }
            for (int i = Game.CurrentBattle.Player.LegionSet.Legion.RealArmyCount; i < 6; i++)
            {
                Destroy(icons[i].gameObject);
            }
        }

        public void ShowPrize()
        {
            fg.text = Game.CurrentBattle.Prizes.GetCurrencyCount(Currency.Type.fedgram).ToString();
            mp.text = Game.CurrentBattle.Prizes.GetCurrencyCount(Currency.Type.manpower).ToString();
            ma.text = Game.CurrentBattle.Prizes.GetCurrencyCount(Currency.Type.mantleAlloy).ToString();
        }

        public void ShowPrize(int fgPrize, int mpPrize, int maPrize)
        {
            fg.text = fgPrize.ToString();
            mp.text = mpPrize.ToString();
            ma.text = maPrize.ToString();
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