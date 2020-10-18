using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Canute.UI.SupplyTeam;
using Canute.Shops;
using Canute.LanguageSystem;
using System;
using Canute.Module;
using Canute.BattleSystem;

namespace Canute.UI.Shop
{
    public class SHOPOnShelfItem : MonoBehaviour
    {
        public Button button => GetComponent<Button>();
        public new Text name;
        public Text cost;
        public Image icon;
        public Image rarity;
        public Label label;
        public PricePair pricePair;

        public void Display(PricePair pricePair)
        {
            this.pricePair = pricePair;
            button.interactable = true;
            name.text = pricePair.Prize.DisplayingName;
            icon.sprite = pricePair.Prize.Icon;
            rarity.sprite = pricePair.Prize.GetRaritySprite();
            cost.text = pricePair.Price.Lang();
        }

        public void Disable()
        {
            button.interactable = false;
        }

        public void TryBuy()
        {
            string infoBase = "Canute.Shop.BuyItem".Lang();
            string info = infoBase.Replace("@itemName", pricePair.Prize.DisplayingName).Replace("@cost", pricePair.Price.Lang());
            ConfirmWindow.CreateConfirmWindow(Buy, info);
        }

        public void Buy()
        {
            bool a = pricePair.Buy();

            if (a)
            {
                Game.PlayerData.ShopInfo.Remove(pricePair);
                SHOPShelf.instance.Refresh();
                string infoBase = "Canute.Shop.GetItem".Lang();
                string info = infoBase.Replace("@itemName", pricePair.Prize.DisplayingName);
                InfoWindow.Create(info);
            }
            else
            {
                string info = "Canute.Shop.NoMoney".Lang();
                InfoWindow.Create(info);
            }
        }


        public void OnMouseOver() { DisplayInfo(); }

        public void OnMouseDown() { DisplayInfo(); }

        public void OnMouseExit() { HideInfo(); }

        public void OnMouseUp() { HideInfo(); }

        public virtual void DisplayInfo()
        {
            if (!label)
            {
                label = Label.GetLabel(transform);
                label.transform.localPosition = new Vector3(30, 30, 0);
            }
            label.gameObject.SetActive(true);

            string info = "";
            switch (pricePair.ItemType)
            {
                case Item.Type.army:
                    Army army = GameData.Prototypes.GetArmyPrototype(pricePair.Prize.Name);
                    info = pricePair.Prize.DisplayingName + "\nA: " + (int)army.Damage + "\nH: " + (int)army.Health + "\nD: " + (int)army.Properties[0].Defense;
                    break;
                case Item.Type.leader:
                    break;
                case Item.Type.equipment:
                    Equipment equipment = GameData.Prototypes.GetEquipmentPrototype(pricePair.Prize.Name);
                    info = equipment.DisplayingName + "\n" + equipment.Bonus.ToArray().Lang();
                    break;
                case Item.Type.eventCard:
                    EventCard eventCard = GameData.Prototypes.GetEventCardPrototype(pricePair.Prize.Name);
                    info = eventCard.DisplayingName + "\n" + eventCard.Effect.Info();
                    break;
                default:
                    break;
            }

            label.text.text = info;
            label.text.alignment = TextAnchor.UpperLeft;
        }

        public virtual void HideInfo()
        {
            label.gameObject.SetActive(false);
        }
    }
}