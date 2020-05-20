using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.EventCardPile
{
    public delegate void PileSelectEvent(int id);
    public class ECPPileSmallIcon : MonoBehaviour
    {
        public int id;
        public static PileSelectEvent SelectEvent;

        public List<GameObject> cardIcons = new List<GameObject>();

        public void Start()
        {
            foreach (Transform item in transform.Find("eventCardIcon"))
            {
                cardIcons.Add(item.gameObject);
            }

            LoadIcon();
        }

        private void LoadIcon()
        {
            if (id == -1)
            {
                return;
            }

            for (int i = 0; i < Game.PlayerData.EventCardPiles[id].EventCards.Count; i++)
            {
                EventCardItem item = Game.PlayerData.EventCardPiles[id].EventCards[i];
                Change(i, item);
            }
        }

        public void Change(int id, EventCardItem armyItem)
        {
            cardIcons[id].GetComponent<Image>().sprite = armyItem.Icon;
        }

        public void ChangePile(int id)
        {
            this.id = id;
            LoadIcon();
        }

        public void SelectPile()
        {
            Debug.Log("Select Pile " + id);
            SelectEvent?.Invoke(id);
        }
    }
}
