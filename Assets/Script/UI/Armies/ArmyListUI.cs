﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Canute.BattleSystem;
using System;
using UnityEngine.UI;
using System.Linq;

namespace Canute.UI
{

    public delegate List<ArmyItem> ArmyArrangement(List<ArmyItem> armyItems);

    public class ArmyListUI : MonoBehaviour, IMonoinstanceMonoBehaviour
    {
        public enum SortType
        {
            none,
            armyType,
            standPosition,
        }

        public static ArmyListUI instance;
        public static ArmyCardSelection CardSelection;
        public static ArmyArrangement ArmyArrangement;
        public static MainScene lastMainScene;
        public static SortType currentSortType;
        public static PropertyType listType;
        public static bool reverseArrangement;
        public static object currentSortTypeParam;
        public static Canute.Legion legion;
        public static List<ArmyItem> currentDisplayingArmy;

        public static List<ArmyItem> GetPlayerArmiesDisplayed()
        {
            List<ArmyItem> list = Game.PlayerData.Armies.Except(legion?.Armies ?? new List<ArmyItem>()).ToList();
            Debug.Log(list.Count);
            return list;

        }

        public GameObject armyCardPrefab;
        public GameObject scroll;

        public List<ArmyCardUI> armies;

        // Use this for initialization
        public void Awake()
        {
            ArmyArrangement = ArmyArrangements.ByLevel;
            currentDisplayingArmy = GetPlayerArmiesDisplayed();
            instance = this;
        }

        public void OnDestroy()
        {
            CardSelection = null;
            instance = null;
        }

        void Start()
        {
            DisplayArmyList();
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region Arrangement

        public void ArrangeByLevel()
        {
            listType = PropertyType.none;
            ArmyArrangement = ArmyArrangements.ByLevel;
            DisplayArmyList();
        }

        public void ArrangeByDamage()
        {
            listType = PropertyType.damage;
            ArmyArrangement = ArmyArrangements.ByDamage;
            DisplayArmyList();
        }

        public void ArrangeByHealth()
        {
            listType = PropertyType.health;
            ArmyArrangement = ArmyArrangements.ByHealth;
            DisplayArmyList();
        }

        public void ArrangeByDefense()
        {
            listType = PropertyType.defense;
            ArmyArrangement = ArmyArrangements.ByDefense;
            DisplayArmyList();
        }

        #endregion

        public void ShowArmy(List<ArmyItem> armyItems)
        {
            foreach (var item in armyItems)
            {
                GameObject gameObject = Instantiate(armyCardPrefab, scroll.transform);
                ArmyCardUI armyCardUI = gameObject.GetComponent<ArmyCardUI>();
                armies.Add(armyCardUI);
                armyCardUI.transform.localScale = Vector3.one;
                armyCardUI.Display(item);
                Label label = Instantiate(GameData.Prefabs.Get("label"), armyCardUI.transform).GetComponent<Label>();
                label.image.color = new Color(0, 0, 0, 0);
                switch (listType)
                {
                    case PropertyType.damage:
                        label.text.text = item.MaxDamage.ToString();
                        break;
                    case PropertyType.health:
                        label.text.text = item.MaxHealth.ToString();
                        break;
                    case PropertyType.defense:
                        label.text.text = item.Defense.ToString();
                        break;
                    case PropertyType.moveRange:
                        break;
                    case PropertyType.attackRange:
                        break;
                    case PropertyType.critRate:
                        break;
                    case PropertyType.critBounes:
                        break;
                    case PropertyType.pop:
                        break;
                    default:
                        break;
                }
            }
        }

        #region FilterSelection

        public void FilterByArmyType(int armyType)
        {
            Army.Types param = (Army.Types)Enum.Parse(typeof(Army.Types), armyType.ToString());
            if (currentSortType == SortType.armyType && currentSortTypeParam.Equals(param))
            {
                currentDisplayingArmy = GetPlayerArmiesDisplayed();
                currentSortType = SortType.none;
            }
            else
            {
                currentDisplayingArmy = ArmyFilters.ByArmyType(param);
                currentSortType = SortType.armyType;
                currentSortTypeParam = param;
            }
            DisplayArmyList();
        }

        public void FilterByStandPostion(int standpostion)
        {
            BattleProperty.Position param = (BattleProperty.Position)standpostion;
            if (currentSortType == SortType.standPosition && currentSortTypeParam.Equals(param))
            {
                currentDisplayingArmy = GetPlayerArmiesDisplayed();
                currentSortType = SortType.none;
                currentSortTypeParam = param;
            }
            else
            {
                currentDisplayingArmy = ArmyFilters.ByStandPosition(param);
                currentSortType = SortType.standPosition;
            }
            DisplayArmyList();
        }

        #endregion

        public void DisplayArmyList()
        {
            ClearArmy();
            List<ArmyItem> armyItems = ArmyArrangement?.Invoke(currentDisplayingArmy);
            armyItems = armyItems ?? GetPlayerArmiesDisplayed();
            if (reverseArrangement)
            {
                armyItems.Reverse();
            }
            ShowArmy(armyItems);
        }

        public void ClearArmy()
        {
            foreach (Transform item in scroll.transform)
            {
                Destroy(item.gameObject);
            }
        }

        public void BackToLastScene()
        {
            SceneControl.GotoSceneImmediate(lastMainScene);
        }

        public void ReverseArrangement()
        {
            reverseArrangement = !reverseArrangement;
            DisplayArmyList();
        }

    }

    public static class ArmyFilters
    {
        public static List<ArmyItem> ByArmyType(Army.Types armyType)
        {
            List<ArmyItem> armyItems = new List<ArmyItem>();

            foreach (var item in ArmyListUI.GetPlayerArmiesDisplayed())
            {
                if (item.Type == armyType)
                {
                    armyItems.Add(item);
                }
            }

            return armyItems;
        }

        public static List<ArmyItem> ByStandPosition(BattleProperty.Position position)
        {
            List<ArmyItem> armyItems = new List<ArmyItem>();

            foreach (var item in ArmyListUI.GetPlayerArmiesDisplayed())
            {
                if (item.StandPosition == position)
                {
                    armyItems.Add(item);
                }
            }

            return armyItems;
        }
    }

    public static class ArmyArrangements
    {
        public static List<ArmyItem> ByLevel(List<ArmyItem> armyItems)
        {
            List<ArmyItem> organizedList = new List<ArmyItem>();
            for (int i = 0; i < armyItems.Count; i++)
            {
                ArmyItem armyItem = armyItems[i];
                if (organizedList.Count == 0)
                {
                    organizedList.Add(armyItem);
                    continue;
                }
                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].Level <= armyItem.Level)
                    {
                        organizedList.Insert(j, armyItem);
                        break;
                    }
                    else if (j == organizedList.Count - 1)
                    {
                        organizedList.Add(armyItem);
                        break;
                    }
                }
            }

            Debug.Log(armyItems.Count + ": " + organizedList.Count);
            return (organizedList);
        }

        public static List<ArmyItem> ByDamage(List<ArmyItem> armyItems)
        {
            List<ArmyItem> organizedList = new List<ArmyItem>();
            for (int i = 0; i < armyItems.Count; i++)
            {
                ArmyItem armyItem = armyItems[i];
                if (organizedList.Count == 0)
                {
                    organizedList.Add(armyItem);
                    continue;
                }
                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].MaxDamage <= armyItem.MaxDamage)
                    {
                        organizedList.Insert(j, armyItem);
                        break;
                    }
                    else if (j == organizedList.Count - 1)
                    {
                        organizedList.Add(armyItem);
                        break;
                    }
                }
            }

            Debug.Log(armyItems.Count + ": " + organizedList.Count);
            return (organizedList);
        }

        public static List<ArmyItem> ByHealth(List<ArmyItem> armyItems)
        {
            List<ArmyItem> organizedList = new List<ArmyItem>();
            for (int i = 0; i < armyItems.Count; i++)
            {
                ArmyItem armyItem = armyItems[i];
                if (organizedList.Count == 0)
                {
                    organizedList.Add(armyItem);
                    continue;
                }
                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].MaxHealth <= armyItem.MaxHealth)
                    {
                        organizedList.Insert(j, armyItem);
                        break;
                    }
                    else if (j == organizedList.Count - 1)
                    {
                        organizedList.Add(armyItem);
                        break;
                    }
                }
            }

            Debug.Log(armyItems.Count + ": " + organizedList.Count);
            return (organizedList);
        }

        public static List<ArmyItem> ByDefense(List<ArmyItem> armyItems)
        {
            List<ArmyItem> organizedList = new List<ArmyItem>();
            for (int i = 0; i < armyItems.Count; i++)
            {
                ArmyItem armyItem = armyItems[i];
                if (organizedList.Count == 0)
                {
                    organizedList.Add(armyItem);
                    continue;
                }
                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].Defense <= armyItem.Defense)
                    {
                        organizedList.Insert(j, armyItem);
                        break;
                    }
                    else if (j == organizedList.Count - 1)
                    {
                        organizedList.Add(armyItem);
                        break;
                    }
                }
            }

            Debug.Log(armyItems.Count + ": " + organizedList.Count);
            return (organizedList);
        }
    }
}