﻿using Canute.BattleSystem;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.Legion
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
                for (int i = 0; i < 6; i++)
                {
                    UpdateIcon(i, null);
                }
                return;
            }

            for (int i = 0; i < Game.PlayerData.Legions[id].Armies.Count; i++)
            {
                ArmyItem item = Game.PlayerData.Legions[id].Armies[i];
                UpdateIcon(i, item);
            }
        }

        public void UpdateIcon(int id, ArmyItem armyItem)
        {
            if (armyItem)
            {
                armyIcons[id].SetActive(true);
                armyIcons[id].GetComponent<Image>().sprite = armyItem.Icon;
            }
            else
            {
                armyIcons[id].SetActive(false);
            }
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