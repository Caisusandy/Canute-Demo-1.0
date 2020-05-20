using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public delegate void EventCardSelection(EventCardUI armyCardUI);

    public class EventCardUI : MonoBehaviour
    {
        public EventCardSelection selectEvent;

        public Image frame;
        public Image chart;
        public Image rarity;
        public Text cost;
        public Text cardName;
        public Text description;

        [HideInInspector] public EventCardItem displayingEventCard;

        public void Awake()
        {
            GetComponent<Canvas>().sortingLayerName = "UI";
        }

        public void Display(EventCardItem eventCardItem)
        {
            displayingEventCard = eventCardItem;

            cardName.text = eventCardItem.DisplayingName;
            cost.text = eventCardItem.Level.ToString();
        }

        public void Select()
        {
            selectEvent?.Invoke(this);
            EventCardList.CardSelection?.Invoke(this);
        }
    }
}