using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.Shops
{
    public static class Build
    {
        public static Prize BuildPrize(uint aethium, uint mantleAlloy, uint manpower, uint federgram)
        {
            float aethiumRarityBounesParam = UnityEngine.Random.value;
            float rarityRandomParam = UnityEngine.Random.value;


            float aethiumPrefer = aethium / 5f * 0.8f;
            float equipmentPrefer = mantleAlloy / 20f * 0.8f;
            float armyPrefer = manpower / 1000f * 0.8f;
            float leaderPrefer = federgram / 1000f * 0.8f;


            float rarityPos = (equipmentPrefer + armyPrefer + leaderPrefer + rarityRandomParam * 9) / 12;
            rarityPos += (1 - rarityPos) * (aethiumPrefer + 0.2f) * (0.2f + aethiumRarityBounesParam / 4);
            float prefer = Mathf.Max(equipmentPrefer, armyPrefer, leaderPrefer);


            ItemWeight itemWeight = ItemWeight.Get(new ItemWeight[3] { new ItemWeight((int)(10000 * armyPrefer), "Army"), new ItemWeight((int)(10000 * leaderPrefer), "Leader"), new ItemWeight((int)(10000 * equipmentPrefer), "Equipment") }, UnityEngine.Random.value);



            Rarity rarity;
            if (rarityPos < 0.43)
                rarity = Rarity.Common;
            else if (rarityPos < 0.65)
                rarity = Rarity.Rare;
            else if (rarityPos < 0.8)
                rarity = Rarity.Epic;
            else
                rarity = Rarity.Legendary;
            switch (itemWeight.itemName)
            {
                case "Army":
                    if (armyPrefer != prefer && rarity != Rarity.Common)
                    {
                        rarity -= 1;
                    }
                    return GetArmy(rarity);
                case "Equipment":
                    if (equipmentPrefer != prefer && rarity != Rarity.Common)
                    {
                        rarity -= 1;
                    }
                    return GetEquipement(rarity);
                case "Leader":
                    if (leaderPrefer != prefer && rarity != Rarity.Common)
                    {
                        rarity -= 1;
                    }
                    return GetLeader(rarity);
                default:
                    break;
            }


            //Debug.Log("=========================================================");
            //Debug.Log("Build Thing: " + itemWeight.itemName + ", Rarity: " + rarity);
            //Debug.Log("Rarity Posiiton: " + rarityPos + ",    AE: " + aethiumPrefer + ";    MA: " + armyPrefer + ";    MP: " + equipmentPrefer + ";    FG: " + leaderPrefer + "；  Rarity Param:" + rarityRandomParam + "  Aethium Bounes Param: " + aethiumRarityBounesParam);

            return new Prize(Prize.Type.none, "", rarity);
        }

        public static Prize GetArmy(Rarity rarity)
        {
            List<ItemWeight> armies = new List<ItemWeight>();
            foreach (var army in GameData.Shop.Army)
            {
                Army item = GameData.Prototypes.GetArmyPrototype(army.itemName);
                if (item.Rarity == rarity)
                {
                    armies.Add(army);
                }
            }
            ItemWeight itemWeight = ItemWeight.Get(armies.ToArray(), UnityEngine.Random.value);
            return new Prize(Prize.Type.army, itemWeight.itemName);
        }

        public static Prize GetEquipement(Rarity rarity)
        {
            List<ItemWeight> items = new List<ItemWeight>();
            foreach (var i in GameData.Shop.Equipment)
            {
                Army item = GameData.Prototypes.GetArmyPrototype(i.itemName);
                if (item.Rarity == rarity)
                {
                    items.Add(i);
                }
            }
            ItemWeight itemWeight = ItemWeight.Get(items.ToArray(), UnityEngine.Random.value);
            return new Prize(Prize.Type.equipement, itemWeight.itemName);
        }

        public static Prize GetLeader(Rarity rarity)
        {
            List<ItemWeight> items = new List<ItemWeight>();
            foreach (var i in GameData.Shop.Leader)
            {
                Army item = GameData.Prototypes.GetArmyPrototype(i.itemName);
                if (item.Rarity == rarity)
                {
                    items.Add(i);
                }
            }
            ItemWeight itemWeight = ItemWeight.Get(items.ToArray(), UnityEngine.Random.value);
            return new Prize(Prize.Type.leader, itemWeight.itemName);
        }
    }

    public struct Prize
    {
        public enum Type
        {
            none,
            army,
            leader,
            equipement
        }

        public Type type;
        public string name;
        public Rarity rarity;

        public Prize(Type type, string name, Rarity rarity)
        {
            this.type = type;
            this.name = name;
            this.rarity = rarity;
        }

        public Prize(Type type, string name) : this()
        {
            this.type = type;
            this.name = name;
        }
    }

    public class ItemWeightList : ScriptableObject
    {
    }

    [Serializable]
    public struct ItemWeight
    {
        public string itemName;
        public int weight;

        public ItemWeight(int weight, string itemName)
        {
            this.weight = weight;
            this.itemName = itemName;
        }

        public static ItemWeight Get(ItemWeight[] itemWeights, float position)
        {
            if (WeightOf(itemWeights) == 0)
            {
                return default;
            }
            int cur = 0;
            int param = (int)(WeightOf(itemWeights) * position);
            foreach (var item in itemWeights)
            {
                if (param - cur < item.weight)
                {
                    return item;
                }
                cur += item.weight;
            }

            throw new ArgumentOutOfRangeException("The parameter is too large to get a Weight Of, the parameter should be between 0 and " + WeightOf(itemWeights));
        }

        public static int WeightOf(ItemWeight[] itemWeights)
        {
            int cur = 0;
            foreach (var item in itemWeights)
            {
                cur += item.weight;
            }
            return cur;
        }

        public static ItemWeight[] Shuffle(ItemWeight[] itemWeights)
        {
            List<ItemWeight> ans = itemWeights.ToList();
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
    }
}
