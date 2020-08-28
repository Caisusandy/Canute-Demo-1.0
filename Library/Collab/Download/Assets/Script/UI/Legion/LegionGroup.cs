using Canute.BattleSystem;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.UI
{
    public class LegionGroup : MonoBehaviour
    {
        public int index;
        public List<ArmyIcon> ArmyIcons;
        public ArmyIcon selectingArmy;
        public LeaderIcon leaderIcon;
        public GameObject armies;

        public static List<LegionGroup> legionGroups = new List<LegionGroup>();
        public Legion Legion => Game.PlayerData.legions[index];

        private void Awake()
        {
            legionGroups.Add(this);
        }

        public void Start()
        {
            if (index != 0)
            {
                armies.SetActive(false);
            }
        }

        public void Update()
        {
            OnLoad();
        }


        public void OnLoad()
        {
            if (armies.activeSelf)
            {
                leaderIcon.displayingLeader = Legion.Leader;
            }

            for (int i = 0; i < Legion.Armies.Count; i++)
            {
                ArmyItem armyItem = Legion.Armies[i];
                ArmyIcons[i].displayingArmy = armyItem;

                //image.sprite = armyItem.Prototype.Icon;
                //image.transform.Find("T_ArmyLevel").GetComponent<Text>().text = "Lv." + armyItem.Level;
            }
            for (int i = Legion.Armies.Count; i < 5; i++)
            {
                ArmyIcons[i].displayingArmy = null;
            }

        }

        public void Select(int index)
        {
            if (selectingArmy != ArmyIcons[index])
            {
                Debug.Log("First Select");
                selectingArmy = ArmyIcons[index];
                LSScroll.instance.LoadScroll(Item.Type.army);
            }
            else
            {
                Debug.Log("Second Select");
                OpenArmyInfo(Legion.Armies[index]);
                /***/
                selectingArmy = null;
            }

        }

        public void OpenArmyInfo(ArmyItem armyItem)
        {

        }

        public void Open()
        {
            LSLegionPanel.instance.selectingLegionGroup = this;

            foreach (LegionGroup item in legionGroups)
            {
                item.armies.SetActive(false);
            }
            armies.SetActive(true);
        }

        public void Next()
        {
            index += 1;
            if (index == 3)
            {
                index = 0;
            }
        }
        public void Last()
        {
            index -= 1;
            if (index == -1)
            {
                index = 2;
            }
        }
    }
}