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

        public Text leaderName;

        [Header("Icon")]
        public Image career;
        public Image leaderIcon;

        public Image equipmentSlot1;
        public Image equipmentSlot2;
        public Image equipmentSlot3;

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
            Debug.Log(armyCardUI);
            armyCardUI.Display(armyItem);
            armySkillCardUI.Display(armyItem);

            armyTypeIcon.SetArmyItem(armyItem);
            attackTypeIcon.SetArmyItem(armyItem);
            standPosition.SetArmyItem(armyItem);
            attackPostion.SetArmyItem(armyItem);


            damage.text = "A: " + armyItem.MaxDamage.ToString();
            health.text = "H: " + armyItem.MaxHealth.ToString();
            defense.text = "D: " + armyItem.Defense.ToString();

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

            if (armyItem.Equipments[0])
                equipmentSlot1.sprite = armyItem.Equipments[0].Icon;
            if (armyItem.Equipments[1])
                equipmentSlot2.sprite = armyItem.Equipments[1].Icon;
            if (armyItem.Equipments[2])
                equipmentSlot3.sprite = armyItem.Equipments[2].Icon;


            moveRange.text = armyItem.Properties.MoveRange.ToString();
            attackRange.text = armyItem.Properties.AttackRange.ToString();
            critBounes.text = armyItem.Properties.CritBonus.ToString() + "%";
            critRate.text = armyItem.Properties.CritRate.ToString() + "%";

            career.sprite = GameData.SpriteLoader.Get(SpriteAtlases.careerIcon, armyItem.Career.ToString());
        }

        public void ClearDisplay()
        {
            armyCardUI.Display(ArmyItem.Empty);
            armyTypeIcon.SetArmyItem(ArmyItem.Empty);
            attackTypeIcon.SetArmyItem(ArmyItem.Empty);
            standPosition.SetArmyItem(ArmyItem.Empty);
            attackPostion.SetArmyItem(ArmyItem.Empty);


            damage.text = "";
            health.text = "";
            defense.text = "";

            moveRange.text = "";
            attackRange.text = "";
            critBounes.text = "";
            critRate.text = "";

            equipmentSlot1.sprite = null;
            equipmentSlot2.sprite = null;
            equipmentSlot3.sprite = null;

            leaderIcon.enabled = false;

            career.sprite = null;
            leaderName.text = "";
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
                LSLegionDisplay.instance.ReloadLegion();
                ArmyListUI.SelectEvent -= Change;
                PlayerFile.SaveCurrentData();
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
            EquipmentListUI.currentListType = EquipmentListUI.ListType.free;
            EquipmentListUI.OpenEquipmentList();
            EquipmentListUI.SelectEvent += Change;

            void Change(EquipmentItem item)
            {
                Debug.Log(item);
                Debug.Log(item.Name);
                Debug.Log(item.EquipmentUsage);
                Debug.Log(SelectingArmy);
                if (!item.EquipmentUsage.Contains(SelectingArmy.Type))
                {
                    Debug.Log("Change Equipment (not)");
                }

                Debug.Log("Change Equipment");
                EquipmentListUI.CloseEquipmentList();
                LSLegionDisplay.instance.ReloadLegion();
                EquipmentListUI.SelectEvent -= Change;

                SelectingArmy.Equipments[id] = item;
                PlayerFile.SaveCurrentData();
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