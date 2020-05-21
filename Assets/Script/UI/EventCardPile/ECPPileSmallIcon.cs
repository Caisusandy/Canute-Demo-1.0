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

        public void LoadIcon()
        {
            if (id == -1)
            {
                return;
            }

            Debug.Log(id);
            for (int i = 0; i < Game.PlayerData.EventCardPiles[id].EventCards.Count; i++)
            {
                if (i > 4)
                {
                    break;
                }
                Debug.Log(id + "," + i);
                EventCardItem item = Game.PlayerData.EventCardPiles[id].EventCards[i];
                UpdateIcon(i, item);
            }

            for (int i = Game.PlayerData.EventCardPiles[id].EventCards.Count; i < 5; i++)
            {
                Hide(i);
            }
        }

        /// <summary>
        /// update single icon
        /// </summary>
        /// <param name="id"></param>
        /// <param name="armyItem"></param>
        public void UpdateIcon(int id, EventCardItem armyItem)
        {
            cardIcons[id].SetActive(true);
            cardIcons[id].GetComponent<Image>().sprite = armyItem.Icon;
        }

        public void Hide(int id)
        {
            cardIcons[id].SetActive(false);
        }

        /// <summary>
        /// change the displaying pile
        /// </summary>
        /// <param name="id"></param>
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
