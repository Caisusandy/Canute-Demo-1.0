using Canute.BattleSystem;
using UnityEngine;

namespace Canute.UI
{
    public class LSScroll : MonoBehaviour
    {
        public static LSScroll instance;

        public GameObject armyIconPrefab;
        public GameObject leaderIconPrefab;
        public GameObject scroll;
        public Item.Type currentType;

        private void Awake()
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

        public void Create(ArmyItem item)
        {
            GameObject icon = Instantiate(armyIconPrefab, scroll.transform);
            ArmyIcon armyIcon = icon.GetComponent<ArmyIcon>();
            armyIcon.displayingArmy = item;
        }

        public void Create(LeaderItem item)
        {
            GameObject icon = Instantiate(leaderIconPrefab, scroll.transform);
            LeaderIcon leaderIcon = icon.GetComponent<LeaderIcon>();
            leaderIcon.displayingLeader = item;
        }

        public void LoadScroll(Item.Type type)
        {
            if (currentType != type)
            {
                ClearScroll();
            }
            else
            {
                return;
            }

            switch (type)
            {
                case Item.Type.army:

                    foreach (ArmyItem item in Game.PlayerData.Armies)
                    {
                        Create(item);
                    }
                    break;
                case Item.Type.leader:
                    foreach (LeaderItem item in Game.PlayerData.Leaders)
                    {
                        Create(item);
                    }
                    break;
                default:
                    break;
            }
        }

        public void ClearScroll()
        {
            foreach (Transform item in scroll.transform)
            {
                Destroy(item.gameObject);
            }
        }
    }

}