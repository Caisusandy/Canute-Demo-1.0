using Canute.Shops;
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
        public int playerScore;

        public Text title;
        public Text subtitle;

        public Text score;
        public Text passed;

        public Text fg;
        public Text mp;
        public Text ma;

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
            playerScore = Game.CurrentBattle.ScoreBoard.GetScore();

            ShowTitle();
            ShowScore();
            ShowPrize();
            ShowLevelPassStatus();
            ShowArmyExpIncrease();

            BattleUI.SetUIInteractable(false);
            BattleUI.SetUICanvasActive(false);
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
            if (Game.CurrentBattle.CurrentStat != Battle.Stat.win)
            {
            }
            int exp = Game.CurrentBattle.Prizes.GetPrizes(Item.Type.exp).Sum((e) => e.Count);
            List<Transform> icons = new List<Transform>();
            foreach (Transform item in armyIcons.transform)
            {
                icons.Add(item);
            }

            int realArmyCount = 0;
            if (Game.CurrentBattle.Player.LegionSet.Legion != null)
                realArmyCount = Game.CurrentBattle.Player.LegionSet.Legion.RealArmyCount;

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
                item.Find("cur").GetComponent<Text>().text = armyItem.Exp + "/" + armyItem.NextLevelExp;
                item.Find("add").GetComponent<Text>().text = Game.CurrentBattle.CurrentStat != Battle.Stat.win ? "+0" : "+" + exp;
            }
            for (int i = realArmyCount; i < 6; i++)
            {
                Destroy(icons[i].gameObject);
            }
        }

        public void ShowPrize()
        {
            if (Game.CurrentBattle.CurrentStat != Battle.Stat.win) { return; }

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