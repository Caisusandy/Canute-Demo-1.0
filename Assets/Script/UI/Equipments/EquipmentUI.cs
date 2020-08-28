using System;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public delegate void EquipmentSelection(EquipmentItem equipmentItem);
    public class EquipmentUI : MonoBehaviour
    {
        public static EquipmentSelection selectEvent;
        public Image rarity;
        public Image Icon;
        public Text level;
        public Text itemName;
        [HideInInspector] public EquipmentItem displayingEquipment;

        public void Display(EquipmentItem item)
        {
            itemName.text = item.DisplayingName;
            level.text = item.Level.ToString();
            displayingEquipment = item;
        }

        public void Select()
        {
            selectEvent?.Invoke(displayingEquipment);
            EquipmentListUI.SelectEvent?.Invoke(displayingEquipment);
        }
    }
}