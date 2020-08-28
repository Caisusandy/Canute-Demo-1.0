using System;
using System.Collections.Generic;
using UnityEngine;
using Canute.BattleSystem;
using UnityEngine.SceneManagement;
using static Canute.Equipment;

namespace Canute.UI
{
    public delegate List<EquipmentItem> EquipmentArrangement(List<EquipmentItem> equipmentItems);


    public class EquipmentListUI : MonoBehaviour
    {
        [Flags]
        public enum ListType
        {
            all,
            free,
            limited,
        }

        public static EquipmentListUI instance;
        public static EquipmentSelection SelectEvent;
        public static EquipmentArrangement EquipmentArrangement;

        public static ListType currentListType = ListType.all;
        public static Army.Types listLimitUsage = Army.Types.none;
        public static PropertyType arrangementType = PropertyType.none;
        public static bool reverseArrangement;
        public static object currentSortTypeParam;
        public static List<EquipmentItem> currentDisplayingEquipment;

        public static List<EquipmentItem> GetPlayerArmiesDisplayed()
        {
            List<EquipmentItem> equipments = Game.PlayerData.Equipments;

            if (currentListType == ListType.all)
            {
                equipments = Game.PlayerData.Equipments;
            }
            if (currentListType == ListType.all)
            {
                equipments = GetFreeEquipment(equipments);
            }
            if (currentListType == ListType.all)
            {
                equipments = GetLimitedEquipment(equipments);
            }
            return equipments;

        }

        public GameObject equipmentPrefab;
        public GameObject scroll;

        public List<EquipmentUI> equipments;

        // Use this for initialization
        public void Awake()
        {
            EquipmentArrangement = EquipmentArrangements.ByLevel;
            currentDisplayingEquipment = GetPlayerArmiesDisplayed();
            instance = this;
        }

        public void OnDestroy()
        {
            SelectEvent = null;
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
            arrangementType = PropertyType.none;
            EquipmentArrangement = EquipmentArrangements.ByLevel;
            DisplayArmyList();
        }

        public void ArrangeByDamage()
        {
            arrangementType = PropertyType.damage;
            EquipmentArrangement = EquipmentArrangements.ByDamage;
            DisplayArmyList();
        }

        public void ArrangeByHealth()
        {
            arrangementType = PropertyType.health;
            EquipmentArrangement = EquipmentArrangements.ByHealth;
            DisplayArmyList();
        }

        public void ArrangeByDefense()
        {
            arrangementType = PropertyType.defense;
            EquipmentArrangement = EquipmentArrangements.ByDefense;
            DisplayArmyList();
        }

        #endregion

        public void ShowEquipment(List<EquipmentItem> equipmentItems)
        {
            foreach (var item in equipmentItems)
            {
                GameObject gameObject = Instantiate(equipmentPrefab, scroll.transform);
                EquipmentUI equipmentUI = gameObject.GetComponent<EquipmentUI>();
                equipments.Add(equipmentUI);
                equipmentUI.transform.localScale = Vector3.one;
                equipmentUI.Display(item);
                Label label = Instantiate(GameData.Prefabs.Get("label"), equipmentUI.transform).GetComponent<Label>();
                label.image.color = new Color(0, 0, 0, 0);
                switch (arrangementType)
                {
                    case PropertyType.damage:
                        //label.text.text = item.MaxDamage.ToString();
                        break;
                    case PropertyType.health:
                        // label.text.text = item.MaxHealth.ToString();
                        break;
                    case PropertyType.defense:
                        //label.text.text = item.Defense.ToString();
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


        public void FilterByArmyType(int armyType)
        {
            if (currentListType.HasFlag(ListType.limited))
            {
                currentDisplayingEquipment = GetPlayerArmiesDisplayed();
                currentListType -= ListType.limited;
            }
            else
            {
                Army.Types param = (Army.Types)Enum.Parse(typeof(Army.Types), armyType.ToString());
                currentListType |= ListType.limited;
                listLimitUsage = param;
                currentDisplayingEquipment = GetPlayerArmiesDisplayed();
            }
            DisplayArmyList();
        }


        public void DisplayArmyList()
        {
            ClearArmy();
            List<EquipmentItem> equipmentItems = EquipmentArrangement?.Invoke(currentDisplayingEquipment);
            equipmentItems = equipmentItems ?? GetPlayerArmiesDisplayed();
            if (reverseArrangement)
            {
                equipmentItems.Reverse();
            }
            ShowEquipment(equipmentItems);
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
            if (SceneManager.sceneCount > 1)
            {
                CloseEquipmentList();
            }
            else
            {
                SceneControl.GotoSceneImmediate(MainScene.mainHall);
            }
        }

        public void ReverseArrangement()
        {
            reverseArrangement = !reverseArrangement;
            DisplayArmyList();
        }

        public static void OpenEquipmentList()
        {
            SceneControl.AddScene(MainScene.playerEquipmentList);
        }

        public static void CloseEquipmentList()
        {
            SceneControl.RemoveScene(MainScene.playerEquipmentList);
        }

        public void Close()
        {
            SceneControl.RemoveScene(MainScene.playerEquipmentList);
        }

        public static List<EquipmentItem> GetFreeEquipment(List<EquipmentItem> equipments)
        {
            foreach (var item in Game.PlayerData.Armies)
            {
                foreach (var equipment in item.Equipments)
                {
                    equipments.Remove(equipment);
                }
            }

            return equipments;
        }

        public static List<EquipmentItem> GetLimitedEquipment(List<EquipmentItem> equipments)
        {
            if (listLimitUsage == Army.Types.none)
            {
                return equipments;
            }

            foreach (var item in equipments)
            {
                if (item.EquipmentUsage.Contains(listLimitUsage))
                {
                    equipments.Remove(item);
                }
            }

            return equipments;
        }

    }


    public static class EquipmentArrangements
    {
        public static List<EquipmentItem> ByLevel(List<EquipmentItem> equipmentItems)
        {
            List<EquipmentItem> organizedList = new List<EquipmentItem>();
            for (int i = 0; i < equipmentItems.Count; i++)
            {
                EquipmentItem equipmentItem = equipmentItems[i];
                if (organizedList.Count == 0)
                {
                    organizedList.Add(equipmentItem);
                    continue;
                }
                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].Level <= equipmentItem.Level)
                    {
                        organizedList.Insert(j, equipmentItem);
                        break;
                    }
                    else if (j == organizedList.Count - 1)
                    {
                        organizedList.Add(equipmentItem);
                        break;
                    }
                }
            }

            Debug.Log(equipmentItems.Count + ": " + organizedList.Count);
            return (organizedList);
        }

        public static List<EquipmentItem> ByDamage(List<EquipmentItem> equipmentItems)
        {
            List<EquipmentItem> organizedList = new List<EquipmentItem>();
            for (int i = 0; i < equipmentItems.Count; i++)
            {
                EquipmentItem equipmentItem = equipmentItems[i];
                if (organizedList.Count == 0)
                {
                    organizedList.Add(equipmentItem);
                    continue;
                }

                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].GetPropertyValueAdditive(PropertyType.damage) <= equipmentItem.GetPropertyValueAdditive(PropertyType.damage))
                    {
                        organizedList.Insert(j, equipmentItem);
                        break;
                    }
                    else if (j == organizedList.Count - 1)
                    {
                        organizedList.Add(equipmentItem);
                        break;
                    }
                }

                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].GetPropertyValuePercentage(PropertyType.damage) <= equipmentItem.GetPropertyValuePercentage(PropertyType.damage))
                    {
                        organizedList.Insert(j, equipmentItem);
                        break;
                    }
                    else if (j == organizedList.Count - 1)
                    {
                        organizedList.Add(equipmentItem);
                        break;
                    }
                }
            }

            Debug.Log(equipmentItems.Count + ": " + organizedList.Count);
            return (organizedList);
        }

        public static List<EquipmentItem> ByHealth(List<EquipmentItem> equipmentItems)
        {
            List<EquipmentItem> organizedList = new List<EquipmentItem>();
            for (int i = 0; i < equipmentItems.Count; i++)
            {
                EquipmentItem equipmentItem = equipmentItems[i];
                if (organizedList.Count == 0)
                {
                    organizedList.Add(equipmentItem);
                    continue;
                }
                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].GetPropertyValueAdditive(PropertyType.health) <= equipmentItem.GetPropertyValueAdditive(PropertyType.health))
                    {
                        organizedList.Insert(j, equipmentItem);
                        break;
                    }
                    else if (j == organizedList.Count - 1)
                    {
                        organizedList.Add(equipmentItem);
                        break;
                    }
                }
                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].GetPropertyValuePercentage(PropertyType.health) <= equipmentItem.GetPropertyValuePercentage(PropertyType.health))
                    {
                        organizedList.Insert(j, equipmentItem);
                        break;
                    }
                    else if (j == organizedList.Count - 1)
                    {
                        organizedList.Add(equipmentItem);
                        break;
                    }
                }
            }

            Debug.Log(equipmentItems.Count + ": " + organizedList.Count);
            return (organizedList);
        }

        public static List<EquipmentItem> ByDefense(List<EquipmentItem> equipmentItems)
        {
            List<EquipmentItem> organizedList = new List<EquipmentItem>();
            for (int i = 0; i < equipmentItems.Count; i++)
            {
                EquipmentItem equipmentItem = equipmentItems[i];
                if (organizedList.Count == 0)
                {
                    organizedList.Add(equipmentItem);
                    continue;
                }
                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].GetPropertyValueAdditive(PropertyType.defense) <= equipmentItem.GetPropertyValueAdditive(PropertyType.defense))
                    {
                        organizedList.Insert(j, equipmentItem);
                        break;
                    }
                    else if (j == organizedList.Count - 1)
                    {
                        organizedList.Add(equipmentItem);
                        break;
                    }
                }
                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].GetPropertyValueAdditive(PropertyType.defense) <= equipmentItem.GetPropertyValueAdditive(PropertyType.defense))
                    {
                        organizedList.Insert(j, equipmentItem);
                        break;
                    }
                    else if (j == organizedList.Count - 1)
                    {
                        organizedList.Add(equipmentItem);
                        break;
                    }
                }
            }

            Debug.Log(equipmentItems.Count + ": " + organizedList.Count);
            return (organizedList);
        }


    }
}
