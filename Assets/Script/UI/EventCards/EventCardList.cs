using Canute.BattleSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.UI
{
    public delegate List<EventCardItem> EventCardArrangement(List<EventCardItem> EventCardItems);

    public class EventCardList : MonoBehaviour
    {
        public enum SortType
        {
            none,
            armyType,
            standPosition,
        }

        public static EventCardList instance;
        public static EventCardSelection CardSelection;
        public static EventCardArrangement EventCardArrangement;
        public static MainScene lastScene;
        public static SortType currentSortType;
        public static PropertyType listType;
        public static bool reverseArrangement;
        public static object currentSortTypeParam;
        public static Canute.EventCardPile eventCardPile;
        public static List<EventCardItem> currentDisplayingArmy;

        public static List<EventCardItem> GetPlayerEventCards()
        {
            List<EventCardItem> list = Game.PlayerData.EventCards.Except(eventCardPile?.EventCards ?? new List<EventCardItem>()).ToList();
            Debug.Log(list.Count);
            return list;

        }

        public GameObject armyCardPrefab;
        public GameObject scroll;

        public List<ArmyCardUI> armies;

        // Use this for initialization
        public void Awake()
        {
            EventCardArrangement = EventCardArrangements.ByLevel;
            currentDisplayingArmy = GetPlayerEventCards();
            instance = this;
        }

        public void OnDestroy()
        {
            CardSelection = null;
            instance = null;
        }

        void Start()
        {
            DisplayEventCardList();
        }

        // Update is called once per frame
        void Update()
        {

        }

        #region Arrangement

        public void ArrangeByLevel()
        {
            listType = PropertyType.none;
            EventCardArrangement = EventCardArrangements.ByLevel;
            DisplayEventCardList();
        }

        public void ArrangeByDamage()
        {
            listType = PropertyType.damage;
            EventCardArrangement = EventCardArrangements.ByDamage;
            DisplayEventCardList();
        }

        public void ArrangeByHealth()
        {
            listType = PropertyType.health;
            EventCardArrangement = EventCardArrangements.ByHealth;
            DisplayEventCardList();
        }

        public void ArrangeByDefense()
        {
            listType = PropertyType.defense;
            EventCardArrangement = EventCardArrangements.ByDefense;
            DisplayEventCardList();
        }

        #endregion

        public void ShowEventCard(List<EventCardItem> EventCardItems)
        {
            foreach (var item in EventCardItems)
            {
                //GameObject gameObject = Instantiate(armyCardPrefab, scroll.transform);
                //ArmyCardUI armyCardUI = gameObject.GetComponent<ArmyCardUI>();
                //armies.Add(armyCardUI);
                //armyCardUI.transform.localScale = Vector3.one;
                //armyCardUI.Display(item);
                //Label label = Instantiate(GameData.Prefabs.Label, armyCardUI.transform).GetComponent<Label>();
                //label.image.color = new Color(0, 0, 0, 0);
                ////switch (listType)
                //{
                //    case PropertyType.damage:
                //        label.text.text = item.MaxDamage.ToString();
                //        break;
                //    case PropertyType.health:
                //        label.text.text = item.MaxHealth.ToString();
                //        break;
                //    case PropertyType.defense:
                //        label.text.text = item.MaxDefense.ToString();
                //        break;
                //    case PropertyType.moveRange:
                //        break;
                //    case PropertyType.attackRange:
                //        break;
                //    case PropertyType.critRate:
                //        break;
                //    case PropertyType.critBounes:
                //        break;
                //    case PropertyType.pop:
                //        break;
                //    default:
                //        break;
                //}
            }
        }

        #region FilterSelection

        public void FilterByArmyType(int armyType)
        {
            Army.Types param = (Army.Types)Enum.Parse(typeof(Army.Types), armyType.ToString());
            if (currentSortType == SortType.armyType && currentSortTypeParam.Equals(param))
            {
                currentDisplayingArmy = GetPlayerEventCards();
                currentSortType = SortType.none;
            }
            else
            {
                currentDisplayingArmy = EventCardFilters.ByArmyType(param);
                currentSortType = SortType.armyType;
                currentSortTypeParam = param;
            }
            DisplayEventCardList();
        }

        public void FilterByStandPostion(int standpostion)
        {
            BattleProperty.Position param = (BattleProperty.Position)standpostion;
            if (currentSortType == SortType.standPosition && currentSortTypeParam.Equals(param))
            {
                currentDisplayingArmy = GetPlayerEventCards();
                currentSortType = SortType.none;
                currentSortTypeParam = param;
            }
            else
            {
                currentDisplayingArmy = EventCardFilters.ByStandPosition(param);
                currentSortType = SortType.standPosition;
            }
            DisplayEventCardList();
        }

        #endregion

        public void DisplayEventCardList()
        {
            ClearArmy();
            List<EventCardItem> EventCardItems = EventCardArrangement?.Invoke(currentDisplayingArmy);
            EventCardItems = EventCardItems ?? GetPlayerEventCards();
            if (reverseArrangement)
            {
                EventCardItems.Reverse();
            }
            ShowEventCard(EventCardItems);
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
            SceneControl.GotoSceneImmediate(lastScene);
        }

        public void ReverseArrangement()
        {
            reverseArrangement = !reverseArrangement;
            DisplayEventCardList();
        }

    }

    public static class EventCardFilters
    {
        public static List<EventCardItem> ByArmyType(Army.Types armyType)
        {
            List<EventCardItem> EventCardItems = new List<EventCardItem>();

            //foreach (var item in EventCardList.GetPlayerEventCards())
            //{
            //    if (item.Type == armyType)
            //    {
            //        EventCardItems.Add(item);
            //    }
            //}

            return EventCardItems;
        }

        public static List<EventCardItem> ByStandPosition(BattleProperty.Position position)
        {
            List<EventCardItem> EventCardItems = new List<EventCardItem>();

            //foreach (var item in ArmyListUI.GetPlayerArmiesDisplayed())
            //{
            //    if (item.StandPosition == position)
            //    {
            //        EventCardItems.Add(item);
            //    }
            //}

            return EventCardItems;
        }
    }

    public static class EventCardArrangements
    {
        public static List<EventCardItem> ByLevel(List<EventCardItem> EventCardItems)
        {
            List<EventCardItem> organizedList = new List<EventCardItem>();
            for (int i = 0; i < EventCardItems.Count; i++)
            {
                EventCardItem EventCardItem = EventCardItems[i];
                if (organizedList.Count == 0)
                {
                    organizedList.Add(EventCardItem);
                    continue;
                }
                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].Level <= EventCardItem.Level)
                    {
                        organizedList.Insert(j, EventCardItem);
                        break;
                    }
                    else if (j == organizedList.Count - 1)
                    {
                        organizedList.Add(EventCardItem);
                        break;
                    }
                }
            }

            Debug.Log(EventCardItems.Count + ": " + organizedList.Count);
            return (organizedList);
        }

        public static List<EventCardItem> ByDamage(List<EventCardItem> EventCardItems)
        {
            List<EventCardItem> organizedList = new List<EventCardItem>();
            for (int i = 0; i < EventCardItems.Count; i++)
            {
                //    EventCardItem EventCardItem = EventCardItems[i];
                //    if (organizedList.Count == 0)
                //    {
                //        organizedList.Add(EventCardItem);
                //        continue;
                //    }
                //    for (int j = 0; j < organizedList.Count; j++)
                //    {
                //        if (organizedList[j].MaxDamage <= EventCardItem.MaxDamage)
                //        {
                //            organizedList.Insert(j, EventCardItem);
                //            break;
                //        }
                //        else if (j == organizedList.Count - 1)
                //        {
                //            organizedList.Add(EventCardItem);
                //            break;
                //        }
                //    }
            }

            Debug.Log(EventCardItems.Count + ": " + organizedList.Count);
            return (organizedList);
        }

        public static List<EventCardItem> ByHealth(List<EventCardItem> EventCardItems)
        {
            List<EventCardItem> organizedList = new List<EventCardItem>();
            for (int i = 0; i < EventCardItems.Count; i++)
            {
                //EventCardItem EventCardItem = EventCardItems[i];
                //if (organizedList.Count == 0)
                //{
                //    organizedList.Add(EventCardItem);
                //    continue;
                //}
                //for (int j = 0; j < organizedList.Count; j++)
                //{
                //    if (organizedList[j].MaxHealth <= EventCardItem.MaxHealth)
                //    {
                //        organizedList.Insert(j, EventCardItem);
                //        break;
                //    }
                //    else if (j == organizedList.Count - 1)
                //    {
                //        organizedList.Add(EventCardItem);
                //        break;
                //    }
                //}
            }

            Debug.Log(EventCardItems.Count + ": " + organizedList.Count);
            return (organizedList);
        }

        public static List<EventCardItem> ByDefense(List<EventCardItem> EventCardItems)
        {
            List<EventCardItem> organizedList = new List<EventCardItem>();
            for (int i = 0; i < EventCardItems.Count; i++)
            {
                //EventCardItem EventCardItem = EventCardItems[i];
                //if (organizedList.Count == 0)
                //{
                //    organizedList.Add(EventCardItem);
                //    continue;
                //}
                //for (int j = 0; j < organizedList.Count; j++)
                //{
                //    if (organizedList[j].MaxDefense <= EventCardItem.MaxDefense)
                //    {
                //        organizedList.Insert(j, EventCardItem);
                //        break;
                //    }
                //    else if (j == organizedList.Count - 1)
                //    {
                //        organizedList.Add(EventCardItem);
                //        break;
                //    }
                //}
            }

            Debug.Log(EventCardItems.Count + ": " + organizedList.Count);
            return (organizedList);
        }
    }
}