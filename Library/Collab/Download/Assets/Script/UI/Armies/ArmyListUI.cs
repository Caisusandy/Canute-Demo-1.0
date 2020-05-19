using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Canute.BattleSystem;
using System;

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
        public static bool reverseArrangement;
        public static object currentSortTypeParam;
        public static List<ArmyItem> currentDisplayingArmy;

        public GameObject armyCardPrefab;
        public GameObject scroll;

        public List<ArmyCardUI> armies;

        // Use this for initialization
        public void Awake()
        {
            ArmyArrangement = ArmyArrangements.ByLevel;
            currentDisplayingArmy = Game.PlayerData.Armies;
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
            ArmyArrangement = ArmyArrangements.ByLevel;
            DisplayArmyList();
        }
        public void ArrangeByDamage()
        {
            ArmyArrangement = ArmyArrangements.ByDamage;
            DisplayArmyList();
        }
        public void ArrangeByHealth()
        {
            ArmyArrangement = ArmyArrangements.ByHealth;
            DisplayArmyList();
        }
        public void ArrangeByDefense()
        {
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
            }
        }

        #region FilterSelection

        public void FilterByArmyType(int armyType)
        {
            Army.Types param = (Army.Types)Enum.Parse(typeof(Army.Types), armyType.ToString());
            if (currentSortType == SortType.armyType && currentSortTypeParam.Equals(param))
            {
                currentDisplayingArmy = Game.PlayerData.Armies;
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
            ArmyProperty.Position param = (ArmyProperty.Position)standpostion;
            if (currentSortType == SortType.standPosition && currentSortTypeParam.Equals(param))
            {
                currentDisplayingArmy = Game.PlayerData.Armies;
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
            armyItems = armyItems ?? Game.PlayerData.Armies;
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

            foreach (var item in Game.PlayerData.Armies)
            {
                if (item.Type == armyType)
                {
                    armyItems.Add(item);
                }
            }

            return armyItems;
        }

        public static List<ArmyItem> ByStandPosition(ArmyProperty.Position position)
        {
            List<ArmyItem> armyItems = new List<ArmyItem>();

            foreach (var item in Game.PlayerData.Armies)
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
                    if (organizedList[j].MaxDefense <= armyItem.MaxDefense)
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