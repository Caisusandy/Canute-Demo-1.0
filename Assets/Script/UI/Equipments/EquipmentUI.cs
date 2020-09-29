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
        public Image icon;
        public Text itemName;
        public Text level;
        public Text info;
        [HideInInspector] public EquipmentItem displayingEquipment;

        public void Display(EquipmentItem item)
        {
            displayingEquipment = item;
            if (displayingEquipment)
            {
                if (itemName) itemName.text = item.DisplayingName;
                if (level) level.text = item.Level.ToString();
                if (icon) icon.sprite = item.Icon;
                if (rarity) rarity.sprite = item.GetRaritySprite();
                if (info) info.text = displayingEquipment.Bonus.ToArray().Lang();
            }
            else
            {
                if (itemName) itemName.text = "";
                if (level) level.text = "";
                if (icon) icon.sprite = null;
                if (rarity) rarity.sprite = GameData.SpriteLoader.Get(SpriteAtlases.rarity, Rarity.none.ToString());
                if (info) info.text = "";
            }
        }

        public void Select()
        {
            selectEvent?.Invoke(displayingEquipment);
            EquipmentListUI.SelectEvent?.Invoke(displayingEquipment);
        }
    }
}