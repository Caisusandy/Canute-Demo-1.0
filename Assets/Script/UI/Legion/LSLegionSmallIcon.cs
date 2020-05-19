using Canute.BattleSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public delegate void LegionSelectEvent(int id);

    public class LSLegionSmallIcon : MonoBehaviour
    {
        public int id;
        public static LegionSelectEvent SelectEvent;

        public List<GameObject> armyIcons = new List<GameObject>();

        public void Start()
        {
            foreach (Transform item in transform.Find("armyIcon"))
            {
                armyIcons.Add(item.gameObject);
            }

            LoadIcon();
        }

        private void LoadIcon()
        {
            if (id == -1)
            {
                return;
            }

            for (int i = 0; i < Game.PlayerData.Legions[id].Armies.Count; i++)
            {
                ArmyItem item = Game.PlayerData.Legions[id].Armies[i];
                Change(i, item);
            }
        }

        public void Change(int id, ArmyItem armyItem)
        {
            armyIcons[id].GetComponent<Image>().sprite = armyItem.Icon;
        }

        public void ChangeLegion(int id)
        {
            this.id = id;
            LoadIcon();
        }

        public void SelectLegion()
        {
            SelectEvent?.Invoke(id);
        }
    }
}
