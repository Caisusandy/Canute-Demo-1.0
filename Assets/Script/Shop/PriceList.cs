using Canute.Module;
using System;
using UnityEngine;

namespace Canute.Shops
{
    //[Serializable]
    //public class PlayerPriceList : DataList<PlayerPricePair>
    //{

    //}

    //[Serializable]
    //public class PlayerPricePair : PricePair, INameable, IWeightable
    //{
    //    [SerializeField] protected bool available;

    //}

    [Serializable]
    [Obsolete]
    public class PriceList : DataList<PricePair>
    {
        [Obsolete]
        public int TotalWeight => WeightItem.WeightOf(ToArray());
        public PricePair InWeightOf(float weight) => WeightItem.Get(weight, ToArray());
        [Obsolete]
        public PricePair RandomOut()
        {
            PricePair pricePair = InWeightOf(UnityEngine.Random.value);
            Debug.Log(pricePair.Name);
            return pricePair;
        }
    }

    [Serializable]
    public class PricePair : INameable, IWeightable
    {
        [SerializeField] protected int weight;
        [SerializeField] protected Prize prize;
        [SerializeField] protected Currency[] price;

        public string Name => Prize.Name;
        [Obsolete]
        public int Count { get => weight; set => weight = value; }
        public Prize Prize { get => prize; set => prize = value; }
        [Obsolete]
        public Currency[] Price { get => price; set => price = value; }
        [Obsolete]
        public Item.Type ItemType { get => Prize.PrizeType; set => Prize.PrizeType = value; }

        [Obsolete]
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
