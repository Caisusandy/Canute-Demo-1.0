using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.EventCardPile
{
    /// <summary>
    /// all-card scroll
    /// </summary>
    public class ECPCardScroll : MonoBehaviour
    {
        public static ECPCardScroll instance;

        public GameObject eventCardPrefab;
        public Transform cardsParent;
        public GridLayoutGroup gridLayoutGroup;


        public List<GameObject> cards = new List<GameObject>();

        public void Start()
        {
            instance = this;
            Load();
        }

        public void Load()
        {
            IEnumerable<EventCardItem> eventCards = Game.PlayerData.EventCards.Except(ECPPileDisplay.instance.EventCardPile);

            foreach (var card in eventCards)
            {
                var gameObject = InstantiateCardEventCardUI(card);
                gameObject.AddComponent<ECPAddCard>();
                cards.Add(gameObject);
            }
            gridLayoutGroup.enabled = true;
        }

        public void Organize()
        {
            gridLayoutGroup.enabled = true;
            gridLayoutGroup.enabled = false;
        }

        public void Unload()
        {
            foreach (var item in cards)
            {
                Destroy(item);
            }
            cards.Clear();
            gridLayoutGroup.enabled = true;
        }

        public void Reload()
        {
            Unload();
            Load();
        }

        public GameObject InstantiateCardEventCardUI(EventCardItem item)
        {
            var cardUIObject = Instantiate(eventCardPrefab, cardsParent);
            var UI = cardUIObject.GetComponent<EventCardUI>();
            UI.Display(item);
            return cardUIObject;
        }
    }

}
