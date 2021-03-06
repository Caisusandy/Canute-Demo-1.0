﻿using Canute.Shops;
using Canute.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.Legion
{
    public class LSArmyUpgradePanel : MonoBehaviour
    {
        public ArmyCardUI upgradingArmyCard;

        public ArmyItem left;
        public ArmyItem right;

        public ArmyCardUI leftUI;
        public ArmyCardUI rightUI;

        public Button upgrade;

        public Text cost;

        public ArmyItem SelectingArmy => upgradingArmyCard.displayingArmy;

        // Start is called before the first frame update
        void Start()
        {
            upgradingArmyCard.Display(LSSingleArmyPanel.SelectingArmy);
        }

        // Update is called once per frame
        void Update()
        {
            leftUI.Display(left);
            rightUI.Display(right);
            //if (left)
            //{
            //    leftCard.image.color = Color.white;
            //    leftCard.image.sprite = left.Prototype.Portrait;
            //}
            //else
            //{
            //    Color white = Color.white;
            //    white.a = 0.1f;
            //    leftCard.image.color = white;
            //}


            //if (right)
            //{
            //    rightCard.image.color = Color.white;
            //    rightCard.image.sprite = right.Prototype.Portrait;
            //}
            //else
            //{
            //    Color white = Color.white;
            //    white.a = 0.1f;
            //    rightCard.image.color = white;
            //}

            upgrade.interactable = CanUpgrade();

            cost.text = "Aethium: " + SelectingArmy.Star * 2;
        }

        public bool CanUpgrade()
        {
            if (!right)
            {
                return false;
            }
            if (!left)
            {
                return false;
            }

            if (right.Prototype != SelectingArmy.Prototype || left.Prototype != SelectingArmy.Prototype)
            {
                return false;
            }
            if (Game.PlayerData.Aethium < SelectingArmy.Star * 2)
            {
                return false;
            }
            return true;
        }

        public void TryUpgrade()
        {
            var ans = SelectingArmy.TryUpgrade(left, right);
            if (!ans) return;
            left = null;
            right = null;
            PlayerFile.SaveCurrentData();
        }

        public void TrySelectCard(int a)
        {
            if (a == 0)
            {
                if (left)
                {
                    left = null;
                    //leftCard.image.sprite = null;
                }
                else
                {
                    ArmyListUI.OpenArmyList();
                    ArmyListUI.hidingArmy = Game.PlayerData.Armies.Where((item) => item.Prototype != LSSingleArmyPanel.SelectingArmy.Prototype || item.Star != SelectingArmy.Star).Union(new List<ArmyItem> { SelectingArmy, right });
                    ArmyListUI.SelectEvent += ChangeLeft;
                    ArmyListUI.lastMainScene = MainScene.legionSetting;

                    void ChangeLeft(ArmyItem item)
                    {
                        Debug.Log("Change Army");

                        left = item;
                        ArmyListUI.CloseArmyList();
                        ArmyListUI.hidingArmy = new List<ArmyItem>();
                        ArmyListUI.SelectEvent -= ChangeLeft;
                        LSLegionDisplay.instance.ReloadLegion();
                        PlayerFile.SaveCurrentData();
                    }
                }
            }
            else
            {

                if (right)
                {
                    right = null;
                    //rightCard.image.sprite = null;
                }
                else
                {
                    ArmyListUI.OpenArmyList();
                    ArmyListUI.hidingArmy = Game.PlayerData.Armies.Where((item) => item.Prototype != SelectingArmy.Prototype || item.Star != SelectingArmy.Star).Union(new List<ArmyItem> { SelectingArmy, left });
                    ArmyListUI.SelectEvent += ChangeRight;
                    ArmyListUI.lastMainScene = MainScene.legionSetting;

                    void ChangeRight(ArmyItem item)
                    {
                        Debug.Log("Change Army");

                        right = item;
                        ArmyListUI.CloseArmyList();
                        ArmyListUI.hidingArmy = new List<ArmyItem>();
                        ArmyListUI.SelectEvent -= ChangeRight;
                        LSLegionDisplay.instance.ReloadLegion();
                        PlayerFile.SaveCurrentData();
                    }
                }
            }
        }
    }
}