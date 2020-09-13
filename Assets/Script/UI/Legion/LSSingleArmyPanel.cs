using Canute.UI.Legion;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    /// <summary>
    /// Left panel of the legion setting
    /// </summary>
    public class LSSingleArmyPanel : MonoBehaviour
    {
        public static LSSingleArmyPanel instance;
        public static ArmyItem SelectingArmy => instance.selectingArmyCard.displayingArmy;

        public ArmyCardUI selectingArmyCard;
        public ArmyCardUI armyCardUI;

        public LSArmySkillCardUI armySkillCardUI;
        public LSArmyUpgradePanel upgradePanel;

        [Header("Text Info")]
        public Text damage;
        public Text defense;
        public Text health;

        public Text moveRangeKey;
        public Text moveRange;
        public Text attackRangeKey;
        public Text attackRange;
        public Text critRateKey;
        public Text critRate;
        public Text critBounesKey;
        public Text critBounes;
        public Text starKey;
        public Text star;

        public Text leaderName;

        [Header("Icon")]
        public Image career;
        public Image leaderIcon;

        [Header("Equipment")]
        public List<EquipmentUI> equipmentSlot;

        public ArmyTypeIcon armyTypeIcon;
        public AttackTypeIcon attackTypeIcon;
        public StandPostionIcon standPosition;
        public AttackPostionIcon attackPostion;

        private void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Display(ArmyItem armyItem)
        {
            ClearDisplay();
            if (!armyItem)
                return;

            transform.GetChild(0).gameObject.SetActive(true);
            Debug.Log(armyCardUI);

            armyCardUI.Display(armyItem);
            armySkillCardUI.Display(armyItem);

            armyTypeIcon.SetArmyItem(armyItem);
            attackTypeIcon.SetArmyItem(armyItem);
            standPosition.SetArmyItem(armyItem);
            attackPostion.SetArmyItem(armyItem);


            damage.text = "A: " + armyItem.DamageForDisplay.ToString();
            health.text = "H: " + armyItem.MaxHealthForDisplay.ToString();
            defense.text = "D: " + armyItem.DefenseForDisplay.ToString();

            if (armyItem.HasLeader)
            {
                leaderIcon.enabled = true;
                leaderName.text = armyItem.Leader.DisplayingName;
                leaderIcon.sprite = armyItem.Leader.Icon;
            }
            else
            {
                leaderName.text = "No Leader Assigned";
                leaderIcon.enabled = false;
            }

            Debug.Log(armyItem.Equipments);

            for (int i = 0; i < 3; i++)
            {
                if (armyItem.Equipments[i])
                {
                    equipmentSlot[i].Display(armyItem.Equipments[i]);
                    equipmentSlot[i].icon.gameObject.SetActive(true);
                }
                else
                {
                    equipmentSlot[i].icon.gameObject.SetActive(false);
                    equipmentSlot[i].Display(null);
                }
            }


            moveRange.text = armyItem.PropertiesAfterEquipment.MoveRange.ToString();
            attackRange.text = armyItem.PropertiesAfterEquipment.AttackRange.ToString();
            critBounes.text = armyItem.PropertiesAfterEquipment.CritBonus.ToString() + "%";
            critRate.text = armyItem.PropertiesAfterEquipment.CritRate.ToString() + "%";
            star.text = armyItem.Star.ToString();

            career.sprite = GameData.SpriteLoader.Get(SpriteAtlases.careerIcon, armyItem.Career.ToString());
        }

        public void ClearDisplay()
        {
            transform.GetChild(0).gameObject.SetActive(false);

            #region old

            //armyCardUI.Display(ArmyItem.Empty);
            //armyTypeIcon.SetArmyItem(ArmyItem.Empty);
            //attackTypeIcon.SetArmyItem(ArmyItem.Empty);
            //standPosition.SetArmyItem(ArmyItem.Empty);
            //attackPostion.SetArmyItem(ArmyItem.Empty);


            //damage.text = "";
            //health.text = "";
            //defense.text = "";

            //moveRange.text = "";
            //attackRange.text = "";
            //critBounes.text = "";
            //critRate.text = "";

            //equipmentSlot1.sprite = null;
            //equipmentSlot2.sprite = null;
            //equipmentSlot3.sprite = null;

            //leaderIcon.enabled = false;

            //career.sprite = null;
            //leaderName.text = "";
            #endregion
        }

        public void ChangeArmy()
        {
            ArmyListUI.OpenArmyList();
            ArmyListUI.legion = LSLegionDisplay.instance.Legion;
            ArmyListUI.SelectEvent += Change;
            ArmyListUI.lastMainScene = MainScene.legionSetting;

            void Change(ArmyItem item)
            {
                Debug.Log("Change Army");
                ArmyListUI.CloseArmyList();
                var legion = LSLegionDisplay.instance.Legion;
                legion.Replace(SelectingArmy, item);
                ArmyListUI.SelectEvent -= Change;
                PlayerFile.SaveCurrentData();
                LSLegionDisplay.instance.ReloadLegion();
            }
        }

        public void ChangeArmyLeader()
        {
            LeaderScroll.notShowingLeader = LSLegionDisplay.instance.Legion.Armies.Select((army) => army.Leader).Except(new List<LeaderItem> { SelectingArmy.Leader });
            LeaderScroll.OpenLeaderScroll();
            LeaderScroll.SelectEvent += Change;

            void Change(LeaderItem leaderItem)
            {
                Debug.Log("Change Leader");
                LeaderScroll.CloseLeaderScroll();
                SelectingArmy.Leader = leaderItem;
                Display(SelectingArmy);
                PlayerFile.SaveCurrentData();
            }
        }

        public void ChangeEquipment(int id)
        {
            EquipmentListUI.OpenEquipmentList();
            EquipmentListUI.currentListType = EquipmentListUI.ListType.strict;
            EquipmentListUI.currentArmy = selectingArmyCard.displayingArmy;
            EquipmentListUI.changingEquipment = selectingArmyCard.displayingArmy.Equipments[id];
            EquipmentListUI.SelectEvent += Change;
            Debug.Log(EquipmentListUI.changingEquipment);
            void Change(EquipmentItem item)
            {
                //Debug.Log(item);
                //Debug.Log(item.Name);
                //Debug.Log(item.EquipmentUsage);
                //Debug.Log(SelectingArmy);
                if (!item.CanUseBy(SelectingArmy.Prototype))
                {
                    Debug.Log("Army can't use");
                    return;
                }

                Debug.Log("Change Equipment");
                EquipmentListUI.CloseEquipmentList();
                EquipmentListUI.SelectEvent -= Change;

                SelectingArmy.Equipments[id] = selectingArmyCard.displayingArmy.Equipments[id] == item ? null : item;
                PlayerFile.SaveCurrentData();
                Display(SelectingArmy);
            }
        }

        public void LeftArmy()
        {

            Debug.Log("Left Army");
            LSLegionDisplay.instance.Legion.Left(SelectingArmy);
            LSLegionDisplay.instance.ReloadLegion();
            PlayerFile.SaveCurrentData();

        }

        public void Filp()
        {
            Vector3 d = new Vector3(0, 5f, 0);
            if (armyCardUI.transform.eulerAngles.y < 90)
            {
                StartCoroutine(Flip());
            }
            else
            {
                StartCoroutine(Reverse());
            }

            IEnumerator Flip()
            {
                while (true)
                {
                    if (armyCardUI.transform.eulerAngles.y < 90)
                    {
                        armyCardUI.transform.eulerAngles += d;
                    }
                    else
                    {
                        armyCardUI.gameObject.SetActive(false);
                        armySkillCardUI.gameObject.SetActive(true);
                        Debug.Log(armySkillCardUI.transform.eulerAngles.y);
                        if (Math.Abs(armySkillCardUI.transform.eulerAngles.y) > 0.1)
                        {
                            armySkillCardUI.transform.eulerAngles -= d;
                        }
                        else
                        {
                            armySkillCardUI.transform.eulerAngles = Vector3.zero;
                            yield break;
                        }
                    }
                    yield return new WaitForFixedUpdate();
                    Debug.Log(armyCardUI.transform.eulerAngles.y);
                }
            }

            IEnumerator Reverse()
            {
                while (true)
                {
                    if (armySkillCardUI.transform.eulerAngles.y < 90)
                    {
                        armySkillCardUI.transform.eulerAngles += d;
                    }
                    else
                    {
                        armySkillCardUI.gameObject.SetActive(false);
                        armyCardUI.gameObject.SetActive(true);
                        //Debug.Log(armyCardUI.transform.eulerAngles.y);
                        if (Math.Abs(armyCardUI.transform.eulerAngles.y) > 0.1)
                        {
                            armyCardUI.transform.eulerAngles -= d;
                        }
                        else
                        {
                            armyCardUI.transform.eulerAngles = Vector3.zero;
                            yield break;
                        }
                    }
                    yield return new WaitForFixedUpdate();
                }
            }
        }

        public void OpenUpgradePanel()
        {
            if (SelectingArmy)
            {
                upgradePanel.gameObject.SetActive(true);
            }
        }
    }
}