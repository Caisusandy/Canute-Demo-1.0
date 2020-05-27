using Canute.UI.Legion;
using System;
using System.Collections;
using System.Collections.Generic;
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

            moveRange.text = armyItem.ArmyProperty.MoveRange.ToString();
            attackRange.text = armyItem.ArmyProperty.AttackRange.ToString();
            critBounes.text = armyItem.ArmyProperty.CritBounes.ToString() + "%";
            critRate.text = armyItem.ArmyProperty.CritRate.ToString() + "%";

            career.sprite = GameData.SpriteLoader.Get(SpriteAtlases.careerIcon, (armyItem.HasLeader ? armyItem.Leader.Career : armyItem.Career).ToString());
            leaderName.text = armyItem.HasLeader ? armyItem.Leader.DisplayingName : "No Leader Assigned";
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

            career.sprite = null;
            leaderName.text = "";
        }

        public void ChangeArmy()
        {
            SceneControl.AddScene(MainScene.playerArmyList);
            ArmyListUI.legion = LSLegionDisplay.instance.Legion;
            ArmyListUI.CardSelection = Change;
            ArmyListUI.lastMainScene = MainScene.legionSetting;

            void Change(ArmyCardUI armyCardUI)
            {
                Debug.Log("Change Army");
                SceneControl.RemoveScene(MainScene.playerArmyList);
                var legion = LSLegionDisplay.instance.Legion;
                legion.Replace(SelectingArmy, armyCardUI.displayingArmy);
                LSLegionDisplay.instance.ReloadLegion();
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
                        Debug.Log(armyCardUI.transform.eulerAngles.y);
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
    }
}