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

        public void Display(EventCardItem item)
        {
            displayingEventCard = item;
            if (displayingEventCard)
            {
                if (cardName) cardName.text = item.DisplayingName;
                if (cost) cost.text = item.Level.ToString();
                //if (frame) frame.sprite = GameData.SpriteLoader.Get(SpriteAtlases.rarity, item.Rarity.ToString());
                if (rarity) rarity.sprite = GameData.SpriteLoader.Get(SpriteAtlases.rarity, item.Rarity.ToString());
                if (chart) chart.sprite = item.Prototype.Sprite;
                if (description) description.text = item.Effect.Info();
            }

        }

        public void Select()
        {
            selectEvent?.Invoke(this);
            EventCardList.CardSelection?.Invoke(this);
        }
    }
}