using System;
using UnityEditor;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public abstract class Item : INameable, IUUIDLabeled, IPrototypeCopy
    {
        public enum Type
        {
            nA = -1,
            none,
            army,
            leader,
            equipment,
            eventCard
        }

        [SerializeField] protected string protoName;
        [SerializeField] protected UUID uuid;
        [SerializeField] protected int exp;
        [SerializeField] protected int floatExp;

        public UUID UUID { get => uuid; set => uuid = value; }
        public string Name => protoName;
        public int Exp => exp + floatExp;

        public string DisplayingName => Proto.DisplayingName;
        public Rarity Rarity => Proto.Rarity;
        public Sprite Icon => Proto.Icon;
        public Sprite Portrait => Proto.Portrait;
        public Sprite Sprite => Proto.Sprite;

        public abstract int Level { get; }
        public abstract Prototype Proto { get; }


        public static implicit operator bool(Item item)
        {
            if (item is null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(item.protoName))
            {
                return false;
            }

            return true;
        }

        protected Item()
        {
            this.NewUUID();
        }

        public static int GetLevel(int @base, double multiple, int currentExp, int maxLevel = 60)
        {
            double curLevel = @base;
            for (int i = 0; i < maxLevel; i++)
            {
                if (currentExp > curLevel)
                {
                    curLevel = (1d - Math.Pow(multiple, i + 1)) / (1d - multiple) * @base;
                }
                else
                {
                    return i + 1;
                }
            }
            return maxLevel;
        }
    }

}

