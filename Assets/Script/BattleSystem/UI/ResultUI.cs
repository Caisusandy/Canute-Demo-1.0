﻿using Canute.Shops;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class ResultUI : MonoBehaviour
    {
        [Serializable]
        public class ResultItem
        {
            public Image icon;
            public Text text;
        }

        public int playerScore;

        public Text title;
        public Text subtitle;

        public Text score;
        public Text passed;

        public Text fg;
        public Text mp;
        public Text ma;

        public List<ResultItem> resultItems;

        public GameObject armyIcons;

        public void OnMouseUp()
        {
            StartCoroutine(Fade());
        }

        public void Close()
        {
            StartCoroutine(Fade());
        }

        public IEnumerator Fade()
        {
            yield return BattleUI.FadeOutBattle();
            yield return new EntityEventPack(Clear).Execute();

            void Clear(params object[] vs)
            {
                Debug.Log("Clear scene");

                Game.ClearBattle();
                SceneControl.GotoScene(MainScene.mainHall);
            }
        }

        public static void Clear(params object[] vs)
        {
            Debug.Log("Clear scene");

            Game.ClearBattle();
            SceneControl.GotoScene(MainScene.mainHall);
        }

        // Start is called before the first frame update
        public void Start()
        {
            //if (Game.CurrentBattle.CurrentStat != Battle.Stat.win || Game.CurrentBattle.CurrentStat != Battle.Stat.lose)
            //{
            //    throw new Exception("Unable to determine");
            //}

            playerScore = Game.CurrentBattle.ScoreBoard.GetScore();

            ShowLevelPassStatus();
            ShowArmyExpIncrease();
            ShowTitle();
            ShowScore();
            ShowPrize();

            BattleUI.SetUIInteractable(false);
            BattleUI.SetUICanvasActive(false);
            OnMapEntity.SetAllEntityCollider(false);

            if (Game.CurrentBattle.CurrentStat != Battle.Stat.win) Game.CurrentLevel.NotPass();
            else Game.CurrentLevel.Pass();
        }

        public void ShowScore()
        {
            if (Game.CurrentBattle.BattleType == Battle.Type.endless) score.text = playerScore.ToString();
            else Destroy(score.gameObject);
        }

        public void ShowLevelPassStatus()
        {
            if (Game.CurrentBattle.CurrentStat != Battle.Stat.win) passed.text = "Canute.BattleSystem.UI.ResultUI.Lost".Lang();
            else passed.text = "Canute.BattleSystem.UI.ResultUI.Passed".Lang();
        }

        public void ShowArmyExpIncrease()
        {
            Debug.Log(Game.CurrentBattle.Prizes.GetPrizes(Item.Type.exp).Count());
            int exp = Game.CurrentBattle.Prizes.GetPrizes(Item.Type.exp).Sum((e) => e.Count);
            Debug.Log(exp);

            List<Transform> icons = new List<Transform>();
            foreach (Transform item in armyIcons.transform) { icons.Add(item); }

            int realArmyCount = 0;
            if (Game.CurrentBattle.Player.LegionSet.Legion != null) realArmyCount = Game.CurrentBattle.Player.LegionSet.Legion.RealArmyCount;

            for (int i = 0; i < realArmyCount; i++)
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
                item.Find("cur").GetComponent<Text>().text = armyItem.CurExp + "/" + armyItem.NextLevelExp;
                item.Find("add").GetComponent<Text>().text = Game.CurrentBattle.CurrentStat != Battle.Stat.win ? "+ 0" : ("+" + exp);
            }
            for (int i = realArmyCount; i < 6; i++)
            {
                Destroy(icons[i].gameObject);
            }
        }

        public void ShowPrize()
        {
            if (Game.CurrentBattle.CurrentStat != Battle.Stat.win) { return; }

            int count = 0;
            IEnumerable<Prize> prizes = Game.CurrentBattle.Prizes.Where((p) => !(p.PrizeType == Item.Type.currency || p.PrizeType == Item.Type.exp || p.PrizeType == Item.Type.commonItem));
            if (!Game.CurrentLevel.IsPassed) prizes = prizes.Union(Game.CurrentBattle.FirstTimePrize);

            foreach (var item in prizes)
            {
                if (count < resultItems.Count)
                {
                    resultItems[count].icon.sprite = item.Icon;
                    resultItems[count].text.text = item.Name;
                    count++;
                }
                else { break; }
            }

            for (; count < resultItems.Count; count++)
            {
                resultItems[count].icon.transform.parent.gameObject.SetActive(false);
            }
            //fg.text = Game.CurrentBattle.Prizes.GetCurrencyCount(Currency.Type.fedgram).ToString();
            //mp.text = Game.CurrentBattle.Prizes.GetCurrencyCount(Currency.Type.manpower).ToString();
            //ma.text = Game.CurrentBattle.Prizes.GetCurrencyCount(Currency.Type.mantleAlloy).ToString();
        }

        public void ShowPrize(int fgPrize, int mpPrize, int maPrize)
        {
            fg.text = fgPrize.ToString();
            mp.text = mpPrize.ToString();
            ma.text = maPrize.ToString();
            Debug.Log("Set prize " + fgPrize + " " + mpPrize + " " + maPrize + " ");
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