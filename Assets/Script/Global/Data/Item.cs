﻿using System;
using UnityEditor;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public abstract class Item : INameable, IUUIDLabeled, IPrototypeCopy, IRarityLabled, IComparable<Item>
    {
        public enum Type
        {
            nA = -1,
            none,
            commonItem,
            army,
            leader,
            equipment,
            eventCard,
            story,
            currency,
            exp,
            letter
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
        /// <summary> item's icon </summary>
        public Sprite Icon => GetIcon();
        protected bool HasPrototype => !(Proto is null);

        /// <summary> actually I don't know what is this </summary>
        [Temporary] public Sprite Sprite => GetSprite();



        protected virtual Rarity GetRarity()
        {
            return HasPrototype ? Proto.Rarity : Rarity.common;
        }

        protected virtual string GetDisplayingName()
        {
            return HasPrototype
                ? Proto.DisplayingName
                : ("Canute.BattleSystem." + ItemType.ToString().ToUpperInvariant() + "." + protoName).Lang();
        }

        protected virtual Sprite GetIcon()
        {
            return Proto.Icon;
        }



        [Temporary]
        protected virtual Sprite GetSprite()
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

            if (item.protoName == "Empty")
            {
                return false;
            }

            return true;
        }

        public static bool operator ==(Item a, Item b)
        {
            if (!a && !b)
            {
                return true;
            }
            else if (a && b)
            {
                return a.Equals(b);
            }
            else return false;
        }
        public static bool operator !=(Item a, Item b)
        {
            return !(a == b);
        }

        protected Item()
        {
            this.NewUUID();
        }

        public static int GetLevel(int @base, double multiple, int currentExp, int maxLevel = 60)
        {
            int curLevelExp = @base;
            for (int i = 1; i < maxLevel; i++)
            {
                if (currentExp > curLevelExp)
                {
                    curLevelExp = (int)((1d - Math.Pow(multiple, i + 1)) / (1d - multiple) * @base);
                }
                else return i;
            }
            return maxLevel;
        }

        public int GetTotalExpInThisLevel(int @base, double multiple, int maxLevel = 60)
        {
            int curLevel = (int)(@base * multiple);
            for (int i = 1; i < maxLevel - 1; i++)
            {
                if (Exp < curLevel)
                {
                    Debug.Log(Exp);
                    Debug.Log(curLevel);
                    return (int)((1d - Math.Pow(multiple, i - 1)) / (1d - multiple) * @base);
                }
                else
                {
                    curLevel = (int)((1d - Math.Pow(multiple, i + 1)) / (1d - multiple) * @base);
                }
            }
            return 0;
        }




        //protected override Sprite GetIcon()
        //{
        //    return HasPrototype
        //        ? Proto.Icon
        //        : GameData.SpriteLoader.Get(SpriteAtlases.armyIcon, protoName).Exist() ?? GameData.SpriteLoader.Get(SpriteAtlases.armyTypeIcon, Type.ToString());
        //}

        //protected override Sprite GetPortrait()
        //{
        //    return HasPrototype ? Proto.Portrait : GameData.SpriteLoader.Get(SpriteAtlases.armyPortrait, protoName);
        //}



        public virtual void AddFloatExp(int floatExp)
        {
            this.floatExp += floatExp;
        }


        public virtual void AddExp(int exp)
        {
            this.exp += exp;
        }

        public virtual int CompareTo(Item other)
        {
            return Rarity - other.Rarity;
        }
    }

}

