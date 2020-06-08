using Canute.Languages;
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
            Army,
            Leader,
            Equipment,
            EventCard
        }

        [SerializeField] protected string protoName;
        [SerializeField] protected UUID uuid;
        [SerializeField] protected int exp;
        [SerializeField] protected int floatExp;


        public abstract int Level { get; }
        public abstract Prototype Proto { get; }
        public abstract Type ItemType { get; }
        public UUID UUID { get => uuid; set => uuid = value; }
        public int Exp => exp + floatExp;
        public string Name => protoName;
        public string DisplayingName => GetDisplayingName();
        public Rarity Rarity => GetRarity();
        /// <summary>  item's icon </summary>
        public Sprite Icon => GetIcon();
        /// <summary> item's portrait </summary>
        public Sprite Portrait => GetPortrait();
        protected bool HasPrototype => !(Proto is null);

        /// <summary> actually I don't know what is this </summary>
        [Temporary] public Sprite Sprite => GetSprite();



        protected Rarity GetRarity()
        {
            if (HasPrototype)
            {
                return Proto.Rarity;
            }
            else
            {
                return Rarity.Common;
            }

        }

        protected string GetDisplayingName()
        {
            if (HasPrototype)
            {
                return Proto.DisplayingName;
            }
            else
            {
                return ("Canute.BattleSystem." + ItemType.ToString().ToUpperInvariant() + "." + protoName).Lang();
            }
        }

        protected virtual Sprite GetIcon()
        {
            if (HasPrototype)
            {
                return Proto.Icon;
            }
            else
            {
                return GameData.SpriteLoader.Get(SpriteAtlases.armyIcon, protoName);
            }
        }

        protected virtual Sprite GetPortrait()
        {
            if (HasPrototype)
            {
                return Proto.Portrait;
            }
            else
            {
                return GameData.SpriteLoader.Get(SpriteAtlases.armyPortrait, protoName);
            }
        }


        [Temporary]
        private Sprite GetSprite()
        {
            if (HasPrototype)
            {
                return Proto.Sprite;
            }
            else
            {
                return GameData.SpriteLoader.Get(SpriteAtlases.armySprite, protoName);
            }
        }


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

