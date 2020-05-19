using Canute.BattleSystem;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.UI.EventCardPile
{

    //public class ECPScroll : MonoBehaviour
    //{
    //    public static ECPScroll instance;

    //    public GameObject armyIconPrefab;
    //    public GameObject leaderIconPrefab;
    //    public GameObject scroll;
    //    public Item.Type currentType;

    //    private void Awake()
    //    {
    //        instance = this;
    //    }

    //    // Start is called before the first frame update
    //    private void Start()
    //    {

    //    }

    //    // Update is called once per frame
    //    private void Update()
    //    {

    //    }

    //    public void Create(ArmyItem item)
    //    {
    //        GameObject icon = Instantiate(armyIconPrefab, scroll.transform);
    //        ArmyIcon armyIcon = icon.GetComponent<ArmyIcon>();
    //        armyIcon.displayingArmy = item;
    //    }

    //    public void Create(LeaderItem item)
    //    {
    //        GameObject icon = Instantiate(leaderIconPrefab, scroll.transform);
    //        LeaderIcon leaderIcon = icon.GetComponent<LeaderIcon>();
    //        leaderIcon.displayingLeader = item;
    //    }

    //    public void LoadScroll(Item.Type type)
    //    {
    //        if (currentType != type)
    //        {
    //            ClearScroll();
    //        }
    //        else
    //        {
    //            return;
    //        }

    //        switch (type)
    //        {
    //            case Item.Type.army:

    //                foreach (ArmyItem item in Game.PlayerData.Armies)
    //                {
    //                    Create(item);
    //                }
    //                break;
    //            case Item.Type.leader:
    //                foreach (LeaderItem item in Game.PlayerData.Leaders)
    //                {
    //                    Create(item);
    //                }
    //                break;
    //            default:
    //                break;
    //        }
    //    }

    //    public void ClearScroll()
    //    {
    //        foreach (Transform item in scroll.transform)
    //        {
    //            Destroy(item.gameObject);
    //        }
    //    }
    //}

    public class ECPPileDisplay : MonoBehaviour
    {
        public static ECPPileDisplay instance;
        public List<GameObject> eventCards;
        public List<ECPPileSmallIcon> cardIcons;

        public int legionId;
        public int lastId { get; set; }

        public Canute.EventCardPile EventCardPile => Game.PlayerData.EventCardPiles[legionId];
        public ECPPileSmallIcon PileSmallIcon => cardIcons[legionId];

        public void Awake()
        {
            instance = this;
            ECPPileSmallIcon.SelectEvent += LoadPile;
        }

        public void OnDestroy()
        {
            instance = null;
        }

        // Start is called before the first frame update
        void Start()
        {
            LoadPile(0);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SelectArmy(int id)
        {
            ECPSingleCardPanel.instance.Display(EventCardPile.EventCards[id]);
            ECPSingleCardPanel.instance.selectingEventCard = eventCards[id].GetComponent<EventCardUI>();
        }

        public void ReloadPile()
        {
            LoadPile(legionId);
        }

        public void LoadPile(int id)
        {
            var pile = Game.PlayerData.EventCardPiles[id];
            legionId = id;

            foreach (var item in pile.EventCards)
            {

            }

            SelectArmy(0);
        }
    }
}
