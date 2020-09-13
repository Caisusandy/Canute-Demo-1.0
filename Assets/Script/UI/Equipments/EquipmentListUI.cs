using System;
using System.Collections.Generic;
using UnityEngine;
using Canute.BattleSystem;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;

namespace Canute.UI
{
    public delegate List<EquipmentItem> EquipmentArrangement(List<EquipmentItem> equipmentItems);


    public class EquipmentListUI : MonoBehaviour
    {
        /// <summary>
        /// mark for either list shows all equipment or only the free one that is not used by any army
        /// </summary>
        [Flags]
        public enum ListType
        {
            strict,
            equiped,
            notLimited,
            equipedAndNotLimited,
        }

        public static EquipmentListUI instance;
        public static EquipmentSelection SelectEvent;
        public static EquipmentArrangement EquipmentArrangement;

        public static ListType currentListType = ListType.strict;
        public static List<Army.Types> listLimitUsage = new List<Army.Types>();
        public static PropertyType arrangementType = PropertyType.none;
        public static bool reverseArrangement;
        public static List<EquipmentItem> currentDisplayingEquipment;
        public static ArmyItem currentArmy;
        public static EquipmentItem changingEquipment;


        public Toggle showUsedEquipment;
        public Toggle showNotAllowedEquipment;
        public GameObject armyTypeSelectionBar;
        public GameObject equipmentPrefab;
        public GameObject firstEquipmentPrefab;
        public GameObject scroll;
        public List<EquipmentUI> equipments;


        // Use this for initialization
        public void Awake()
        {
            showUsedEquipment.isOn = currentListType.HasFlag(ListType.equiped);
            showNotAllowedEquipment.isOn = currentListType.HasFlag(ListType.notLimited);
            EquipmentArrangement = EquipmentArrangements.ByLevel;
            currentDisplayingEquipment = GetPlayerEquipmentDisplayed();
            instance = this;
        }

        public void OnDestroy()
        {
            SelectEvent = null;
            instance = null;
        }

        void Start()
        {
            if (currentArmy) armyTypeSelectionBar.SetActive(false);
            DisplayEquipmentList();
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
            DisplayEquipmentList();
        }

        public void ArrangeByDamage()
        {
            arrangementType = PropertyType.damage;
            EquipmentArrangement = EquipmentArrangements.ByDamage;
            DisplayEquipmentList();
        }

        public void ArrangeByHealth()
        {
            arrangementType = PropertyType.health;
            EquipmentArrangement = EquipmentArrangements.ByHealth;
            DisplayEquipmentList();
        }

        public void ArrangeByDefense()
        {
            arrangementType = PropertyType.defense;
            EquipmentArrangement = EquipmentArrangements.ByDefense;
            DisplayEquipmentList();
        }


        public void ArrangeByMoveRange()
        {
            arrangementType = PropertyType.moveRange;
            EquipmentArrangement = EquipmentArrangements.ByMoveRange;
            DisplayEquipmentList();
        }

        public void ArrangeByAttackRange()
        {
            arrangementType = PropertyType.attackRange;
            EquipmentArrangement = EquipmentArrangements.ByAttackRange;
            DisplayEquipmentList();
        }

        public void ArrangeByCritRate()
        {
            arrangementType = PropertyType.critRate;
            EquipmentArrangement = EquipmentArrangements.ByCritRate;
            DisplayEquipmentList();
        }

        public void ArrangeByCritBonus()
        {
            arrangementType = PropertyType.critBonus;
            EquipmentArrangement = EquipmentArrangements.ByCritBonus;
            DisplayEquipmentList();
        }

        public void ArrangeByPop()
        {
            arrangementType = PropertyType.pop;
            EquipmentArrangement = EquipmentArrangements.ByPopulation;
            DisplayEquipmentList();
        }

        #endregion

        public void ShowEquipment(List<EquipmentItem> equipmentItems)
        {
            Debug.Log(currentListType);
            if (equipmentItems == null)
            {
                return;
            }

            if (changingEquipment)
            {
                CreateFisrt(changingEquipment);
            }

            foreach (var item in equipmentItems)
            {
                EquipmentUI equipmentUI = CreateEquipmentUI(item);

                if (!equipmentUI.displayingEquipment.CanUseBy(currentArmy?.Prototype) && currentArmy)
                {
                    equipmentUI.GetComponent<Button>().interactable = false;
                }

                if (equipmentUI.displayingEquipment.IsUsed && currentArmy)
                {
                    equipmentUI.GetComponent<Button>().interactable = false;
                }


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
                    case PropertyType.critBonus:
                        break;
                    case PropertyType.pop:
                        break;
                    default:
                        break;
                }
            }
        }

        private EquipmentUI CreateEquipmentUI(EquipmentItem item)
        {
            var equipmentUI = Instantiate(equipmentPrefab, scroll.transform).GetComponent<EquipmentUI>();
            equipmentUI.Display(item);
            equipmentUI.transform.localScale = Vector3.one;

            equipments.Add(equipmentUI);

            Label label = Instantiate(GameData.Prefabs.Get("label"), equipmentUI.transform).GetComponent<Label>();
            label.image.color = new Color(0, 0, 0, 0);

            return equipmentUI;
        }
        private EquipmentUI CreateFisrt(EquipmentItem item)
        {
            var equipmentUI = Instantiate(firstEquipmentPrefab, scroll.transform).GetComponent<EquipmentUI>();
            equipmentUI.Display(item);
            equipmentUI.transform.localScale = Vector3.one;

            equipments.Add(equipmentUI);

            Label label = Instantiate(GameData.Prefabs.Get("label"), equipmentUI.transform).GetComponent<Label>();
            label.image.color = new Color(0, 0, 0, 0);

            return equipmentUI;
        }

        public void FilterByArmyType(int armyType)
        {
            if (currentListType.HasFlag(ListType.notLimited))
            {
                currentDisplayingEquipment = GetPlayerEquipmentDisplayed();
                currentListType -= ListType.notLimited;
            }
            else
            {
                Army.Types param = (Army.Types)Enum.Parse(typeof(Army.Types), armyType.ToString());
                currentListType |= ListType.notLimited;
                if (!listLimitUsage.Contains(param)) listLimitUsage.Add(param);
                else listLimitUsage.Remove(param);

                currentDisplayingEquipment = GetPlayerEquipmentDisplayed();
            }
            DisplayEquipmentList();
        }

        public void ShowArmyUsedEquipment(bool show)
        {
            if (show)
                currentListType |= ListType.equiped;
            else
                currentListType &= ~ListType.equiped;

            DisplayEquipmentList();
        }

        public void ShowArmyNotAllowedEquipment(bool show)
        {
            if (show)
                currentListType |= ListType.notLimited;
            else
                currentListType &= ~ListType.notLimited;

            DisplayEquipmentList();
        }

        public static List<EquipmentItem> GetPlayerEquipmentDisplayed()
        {
            List<EquipmentItem> equipments = Game.PlayerData.Equipments;
            equipments.Sort();

            Debug.Log(equipments.Count);
            Debug.Log(currentArmy);


            if (changingEquipment)
            {
                equipments.Remove(changingEquipment);
            }
            if (currentArmy)
            {
                equipments.RemoveAll((e) => currentArmy.Equipments.Contains(e));
            }
            if (!currentListType.HasFlag(ListType.equiped))
            {
                equipments = RemoveEquiped(equipments);
            }
            if (!currentListType.HasFlag(ListType.notLimited))
            {
                equipments = RemoveUnmatched(equipments);
            }

            return equipments;

        }

        public void DisplayEquipmentList()
        {
            ClearList();
            List<EquipmentItem> equipmentItems = EquipmentArrangement?.Invoke(GetPlayerEquipmentDisplayed());
            if (reverseArrangement)
            {
                equipmentItems.Reverse();
            }
            ShowEquipment(equipmentItems);
        }

        public void ClearList()
        {
            foreach (var item in equipments)
            {
                Destroy(item.gameObject);
            }
            equipments.Clear();
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
            DisplayEquipmentList();
        }

        /// <summary>
        /// Get all the free equipment
        /// </summary>
        /// <param name="equipments"></param>
        /// <returns></returns>
        public static List<EquipmentItem> RemoveEquiped(List<EquipmentItem> equipments)
        {
            return equipments.Where((e) => !e.IsUsed).ToList();
        }

        /// <summary>
        /// Get all valid equipment for the limiting army type
        /// </summary>
        /// <param name="equipments"></param>
        /// <returns></returns>
        public static List<EquipmentItem> RemoveUnmatched(List<EquipmentItem> equipments)
        {
            var listLimitUsage = EquipmentListUI.listLimitUsage.ShallowClone();

            if (currentArmy)
            {
                listLimitUsage.Add(currentArmy.Type);
            }
            if (listLimitUsage.Count == 0)
            {
                return equipments;
            }

            for (int i = equipments.Count - 1; i >= 0; i--)
            {
                EquipmentItem item = equipments[i];
                bool remove = true;
                foreach (var usage in item.EquipmentUsage)
                {
                    if (listLimitUsage.Contains(usage)) { remove = false; }
                }
                if (remove) { equipments.Remove(item); }
            }

            return equipments;
        }

        public static void Initialize()
        {
            SelectEvent = null;
            EquipmentArrangement = null;
            currentListType = ListType.strict;
            listLimitUsage = new List<Army.Types>();
            arrangementType = PropertyType.none;
            reverseArrangement = false;
            currentDisplayingEquipment = new List<EquipmentItem>();
            currentArmy = null;
            changingEquipment = null;

        }

        public static void OpenEquipmentList()
        {
            Initialize();
            SceneControl.AddScene(MainScene.playerEquipmentList);
        }

        public static void CloseEquipmentList()
        {
            SceneControl.RemoveScene(MainScene.playerEquipmentList);
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
                    if (organizedList[j].GetPropertyValuePercentage(PropertyType.defense) <= equipmentItem.GetPropertyValuePercentage(PropertyType.defense))
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

        public static List<EquipmentItem> ByMoveRange(List<EquipmentItem> items)
        {
            List<EquipmentItem> organizedList = new List<EquipmentItem>();
            for (int i = 0; i < items.Count; i++)
            {
                EquipmentItem equipmentItem = items[i];
                if (organizedList.Count == 0)
                {
                    organizedList.Add(equipmentItem);
                    continue;
                }
                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].GetPropertyValueAdditive(PropertyType.moveRange) <= equipmentItem.GetPropertyValueAdditive(PropertyType.moveRange))
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
                    if (organizedList[j].GetPropertyValuePercentage(PropertyType.moveRange) <= equipmentItem.GetPropertyValuePercentage(PropertyType.moveRange))
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

            Debug.Log(items.Count + ": " + organizedList.Count);
            return (organizedList);
        }

        public static List<EquipmentItem> ByAttackRange(List<EquipmentItem> items)
        {
            List<EquipmentItem> organizedList = new List<EquipmentItem>();
            for (int i = 0; i < items.Count; i++)
            {
                EquipmentItem equipmentItem = items[i];
                if (organizedList.Count == 0)
                {
                    organizedList.Add(equipmentItem);
                    continue;
                }
                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].GetPropertyValueAdditive(PropertyType.attackRange) <= equipmentItem.GetPropertyValueAdditive(PropertyType.attackRange))
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
                    if (organizedList[j].GetPropertyValuePercentage(PropertyType.attackRange) <= equipmentItem.GetPropertyValuePercentage(PropertyType.attackRange))
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

            Debug.Log(items.Count + ": " + organizedList.Count);
            return (organizedList);
        }

        public static List<EquipmentItem> ByCritRate(List<EquipmentItem> items)
        {
            List<EquipmentItem> organizedList = new List<EquipmentItem>();
            for (int i = 0; i < items.Count; i++)
            {
                EquipmentItem equipmentItem = items[i];
                if (organizedList.Count == 0)
                {
                    organizedList.Add(equipmentItem);
                    continue;
                }
                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].GetPropertyValueAdditive(PropertyType.critRate) <= equipmentItem.GetPropertyValueAdditive(PropertyType.critRate))

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
                    if (organizedList[j].GetPropertyValuePercentage(PropertyType.critRate) <= equipmentItem.GetPropertyValuePercentage(PropertyType.critRate))
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

            Debug.Log(items.Count + ": " + organizedList.Count);
            return (organizedList);
        }

        public static List<EquipmentItem> ByCritBonus(List<EquipmentItem> items)
        {
            List<EquipmentItem> organizedList = new List<EquipmentItem>();
            for (int i = 0; i < items.Count; i++)
            {
                EquipmentItem equipmentItem = items[i];
                if (organizedList.Count == 0)
                {
                    organizedList.Add(equipmentItem);
                    continue;
                }
                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].GetPropertyValueAdditive(PropertyType.critBonus) <= equipmentItem.GetPropertyValueAdditive(PropertyType.critBonus))
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
                    if (organizedList[j].GetPropertyValuePercentage(PropertyType.critBonus) <= equipmentItem.GetPropertyValuePercentage(PropertyType.critBonus))
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

            Debug.Log(items.Count + ": " + organizedList.Count);
            return (organizedList);
        }

        public static List<EquipmentItem> ByPopulation(List<EquipmentItem> items)
        {
            List<EquipmentItem> organizedList = new List<EquipmentItem>();
            for (int i = 0; i < items.Count; i++)
            {
                EquipmentItem equipmentItem = items[i];
                if (organizedList.Count == 0)
                {
                    organizedList.Add(equipmentItem);
                    continue;
                }
                for (int j = 0; j < organizedList.Count; j++)
                {
                    if (organizedList[j].GetPropertyValueAdditive(PropertyType.pop) <= equipmentItem.GetPropertyValueAdditive(PropertyType.pop))
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
                    if (organizedList[j].GetPropertyValuePercentage(PropertyType.pop) <= equipmentItem.GetPropertyValuePercentage(PropertyType.pop))
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

            Debug.Log(items.Count + ": " + organizedList.Count);
            return (organizedList);
        }

    }
}
