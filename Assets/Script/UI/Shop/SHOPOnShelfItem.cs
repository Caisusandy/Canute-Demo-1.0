using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Canute.UI.Exploration;
using Canute.Shops;
using Canute.Languages;
using System;

namespace Canute.UI.Shop
{
    public class SHOPOnShelfItem : MonoBehaviour
    {
        public Text Name;
        public PricePair pricePair;

        public void Display(PricePair pricePair)
        {
            this.pricePair = pricePair;
            Name.text = pricePair.Prize.DisplayingName;
            foreach (var item in pricePair.Price)
            {
                Name.text += "\n" + item.ToString();
            }
        }

        public void TryBuy()
        {
            ConfirmWindow.CreateConfirmWindow(Buy, "Buy " + pricePair.Prize.DisplayingName + " ?");
        }

        public void Buy()
        {
            bool a = pricePair.Buy();
        }
    }
}