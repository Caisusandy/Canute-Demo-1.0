using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.UI
{
    [Obsolete("For old legion setting only")]
    public class LSLegionPanel : MonoBehaviour
    {
        public static LSLegionPanel instance;

        public List<LegionGroup> legionGroups;
        public LegionGroup selectingLegionGroup;

        public void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }

        public void Select(ArmyIcon armyIcon)
        {
            ArmyIcon legionPos = selectingLegionGroup.selectingArmy;

            if (!legionPos)
            {
                return;
            }

            if (legionPos.displayingArmy == armyIcon.displayingArmy)
            {
                return;
            }

            selectingLegionGroup.Legion.Replace(legionPos.displayingArmy, armyIcon.displayingArmy);
            legionPos.displayingArmy = armyIcon.displayingArmy;
        }

        public void Select(LeaderIcon leaderIcon)
        {
            LeaderIcon legionPos = selectingLegionGroup.leaderIcon;

            if (!legionPos)
            {
                Debug.Log("No selecting legion");
                return;
            }

            if (legionPos.displayingLeader == leaderIcon.displayingLeader)
            {
                Debug.Log("same leader");
                return;
            }

            //selectingLegionGroup.Legion.SetLeader(leaderIcon.displayingLeader);
            legionPos.displayingLeader = leaderIcon.displayingLeader;
        }

        public void EditLeader()
        {
            LSScroll.instance.LoadScroll(Item.Type.leader);
        }
    }
}