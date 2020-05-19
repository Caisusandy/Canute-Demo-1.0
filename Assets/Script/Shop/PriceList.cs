using System;
using System.Collections.Generic;
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
            return InWeightOf(random);
        }
    }

    [Serializable]
    public class PricePair : INameable
    {
        [SerializeField] protected string name;
        [SerializeField] protected int weight;
        [SerializeField] protected Currency price;

        public string Name => name;
        public int Weight { get => weight; set => weight = value; }
        public Currency Price { get => price; set => price = value; }
    }
}
