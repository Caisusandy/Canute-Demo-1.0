using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

namespace Canute.Shops
{
    [Serializable]
    public struct WeightItem : INameable, ICountable, IWeightable
    {
        public string itemName;
        public int weight;

        public string Name => itemName;
        public int Count { get => weight; set => weight = value; }

        public WeightItem(int weight, string itemName)
        {
            this.weight = weight;
            this.itemName = itemName;
        }

        public static T Get<T>(float position, params T[] itemWeights) where T : IWeightable
        {
            int totalWeight = WeightOf(itemWeights);
            if (totalWeight == 0)
            {
                return default;
            }
            if (position > 1 && position < 0)
            {
                position %= 1;
            }

            float param = totalWeight * position;
            //Debug.Log(totalWeight + ", " + param);

            for (int i = 0; i < itemWeights.Length; i++)
            {
                T item = itemWeights[i];
                param -= item.Count;
                if (param <= 0)
                {
                    //Debug.Log(i + ", " + item.Count + ", " + (param + item.Count) + ", " + totalWeight);
                    return item;
                }
            }

            throw new ArgumentOutOfRangeException("The parameter is too large to get a Weight Of, the parameter should be between 0 and " + totalWeight + ", but parameter lead to " + param);
        }

        public static int WeightOf<T>(params T[] itemWeights) where T : IWeightable
        {
            int cur = 0;
            foreach (var item in itemWeights)
            {
                cur += item.Count;
            }
            //Debug.Log(cur);
            return cur;
        }

        public static IWeightable[] Shuffle(IWeightable[] itemWeights)
        {
            List<IWeightable> ans = itemWeights.ToList();
            for (int i = 0; i < 5 * itemWeights.Length; i++)
            {
                byte[] bytes = new byte[4];
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider()) { rng.GetBytes(bytes); }
                System.Random random = new System.Random(BitConverter.ToInt32(bytes, 0));
                int n1 = random.Next(0, itemWeights.Length);
                int n2 = random.Next(0, itemWeights.Length);
                var c = itemWeights[n1];
                ans.RemoveAt(n1);
                ans.Insert(n2, c);
            }
            Debug.Log(WeightOf(itemWeights));
            Debug.Log(WeightOf(ans.ToArray()));
            return ans.ToArray();
        }

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(WeightItem left, WeightItem right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WeightItem left, WeightItem right)
        {
            return !(left == right);
        }
    }
    public interface ICountable
    {
        int Count { get; }
    }
    public interface IWeightable : ICountable, INameable
    {
    }
}
