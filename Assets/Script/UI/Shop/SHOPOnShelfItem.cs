using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Canute.UI.SupplyTeam;
using Canute.Shops;
using Canute.LanguageSystem;
using System;

namespace Canute.UI.Shop
{
    public class SHOPOnShelfItem : MonoBehaviour
    {
        public Button button => GetComponent<Button>();
        public Text name;
        public Text cost;
        public Image icon;
        public Image rarity;
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
                InfoWindow.CreateInfoWindow(info);
            }
            else
            {
                string info = "Canute.Shop.NoMoney".Lang();
                InfoWindow.CreateInfoWindow(info);
            }
        }
    }
}