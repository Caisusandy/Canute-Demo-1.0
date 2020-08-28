using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.EventCardPile
{
    /// <summary>
    /// event card pile
    /// </summary>
    public class ECPPileDisplay : MonoBehaviour
    {
        public static ECPPileDisplay instance;

        public GameObject eventCardPrefab;
        private int pileID;
        public Transform cardsParent;
        public Text cardCount;
        public List<GameObject> cardsInPile;
        public List<ECPPileSmallIcon> cardIcons;

        public int PileID { get => pileID; set => pileID = value; }
        public int lastId { get; set; }

        public Canute.EventCardPile EventCardPile => Game.PlayerData.EventCardPiles[PileID];


        public ECPPileSmallIcon PileSmallIcon => cardIcons[PileID];

        public void Awake()
        {
            instance = this;
            ECPPileSmallIcon.SelectEvent = SelectPile;
        }

        public void Start()
        {
            LoadPile(pileID);
        }

        public GameObject InstantiateCardEventCardUI(EventCardItem item)
        {
            var cardUIObject = Instantiate(eventCardPrefab, cardsParent);
            var UI = cardUIObject.GetComponent<EventCardUI>();
            UI.gameObject.AddComponent<ECPRemoveCard>();
            UI.Display(item);
            return cardUIObject;
        }

        public void LoadPile(int id)
        {
            Debug.Log("Load Pile " + id);
            PileID = id;
            foreach (var item in Game.PlayerData.EventCardPiles[id].EventCards)
            {
                var gameObject = InstantiateCardEventCardUI(item);
                cardsInPile.Add(gameObject);
            }
            LoadCardCount();
        }

        public void UnloadPile()
        {
            foreach (var item in cardsInPile)
            {
                Destroy(item);
            }
            cardsInPile.Clear();
        }


        public void Reload()
        {
            UnloadPile();
            LoadPile(pileID);
            PileSmallIcon.LoadIcon();
        }

        public void SelectPile(int id)
        {
            UnloadPile();
            LoadPile(id);
            ECPCardScroll.instance.Reload();
        }

        public void LoadCardCount()
        {
            cardCount.text = EventCardPile.CardCount + "/24";
        }
    }
}
