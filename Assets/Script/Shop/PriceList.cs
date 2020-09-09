using Canute.Module;
using System;
using UnityEngine;

namespace Canute.Shops
{
    [Serializable]
    public class PriceList : DataList<PricePair>
    {
        public int TotalWeight => GetSum();

        protected int GetSum()
        {
            int i = 0;
            foreach (PricePair item in this)
            {
                i += item.Weight;
            }
            return i;
        }

        public PricePair InWeightOf(int weight)
        {
            foreach (PricePair item in this)
            {
                if (weight < item.Weight)
                {
                    return item;
                }
                else
                {
                    weight -= item.Weight;
                }
            }
            return null;
        }

        public PricePair RandomOut()
        {
            int random = UnityEngine.Random.Range(0, TotalWeight);
            PricePair pricePair = InWeightOf(random);
            Debug.Log(pricePair.Name);
            return pricePair;
        }
    }

    [Serializable]
    public class PricePair : INameable
    {
        [SerializeField] protected Prize prize;
        [SerializeField] protected int weight;
        [SerializeField] protected Currency[] price;

        public string Name => Prize.Name;
        public int Weight { get => weight; set => weight = value; }
        public Prize Prize { get => prize; set => prize = value; }
        public Currency[] Price { get => price; set => price = value; }
        public Item.Type ItemType { get => Prize.PrizeType; set => Prize.PrizeType = value; }

        public bool Buy()
        {
            if (Game.PlayerData.Spent(price))
            {
                prize.Fulfill();
                return true;
            }

            return false;
        }
    }
}
