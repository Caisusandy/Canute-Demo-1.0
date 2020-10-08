using Canute.BattleSystem;
using Canute.Shops;
using Canute.StorySystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public class Prize : IWeightable, ICloneable, IRarityLabled
    {
        [SerializeField] protected string name;
        [SerializeField] protected int count;
        [SerializeField] protected int parameter;
        [SerializeField] protected Item.Type type;

        public string Name => name;
        public int Count { get => count; }
        public int Parameter { get => parameter; }
        public Item.Type PrizeType { get => type; set => type = value; }
        public string DisplayingName => GetDisplayingName();
        public Sprite Icon => GetIcon();
        public Rarity Rarity => GetRarity();

        public Prize(string name, int count, Item.Type type)
        {
            this.type = type;
            this.name = name;
            this.count = count;
        }
        public Prize()
        {
        }





        public bool Fulfill()
        {
            Debug.LogWarning("try fulfill prize " + name);
            switch (PrizeType)
            {
                case Item.Type.none:
                    Debug.LogWarning("there is something wrong with the prize");
                    break;
                case Item.Type.story:
                    //StoryDisplayer.Load(Story.Get(name));
                    Game.PlayerData.AddCollectionStory(name);
                    //
                    break;
                case Item.Type.letter:
                    //LetterDisplayer.Load(Letter.Get(name));
                    Game.PlayerData.AddCollectionLetter(name);
                    break;
                case Item.Type.army:
                    Army army = GameData.Prototypes.GetArmyPrototype(Name);
                    if (!army) return false;
                    Game.PlayerData.AddArmyItem(new ArmyItem(army));
                    break;
                case Item.Type.leader:
                    Leader leader = GameData.Prototypes.GetLeaderPrototype(Name);
                    if (!leader) return false;
                    if (Game.PlayerData.IsLeaderUnlocked(leader.Name)) return false;
                    Game.PlayerData.AddLeaderItem(new LeaderItem(leader));
                    break;
                case Item.Type.equipment:
                    Equipment equipement = GameData.Prototypes.GetEquipmentPrototype(Name);
                    if (!equipement) return false;
                    Game.PlayerData.AddEquipmentItem(new EquipmentItem(equipement));
                    break;
                case Item.Type.eventCard:
                    EventCard eventCard = GameData.Prototypes.GetEventCardPrototype(Name);
                    if (!eventCard) return false;
                    Game.PlayerData.AddEventCardItem(new EventCardItem(eventCard));
                    break;
                case Item.Type.currency:
                    Currency.Type type;
                    if (!Enum.TryParse(Name, out type))
                    {
                        return false;
                    }
                    Game.PlayerData.Earned(type, Parameter);
                    break;
                default:
                    break;
            }
            Debug.Log("Prize fulfilled");
            PlayerFile.SaveCurrentData();
            return true;
        }

        /// <summary>
        /// Give items exp
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items"></param>
        /// <returns></returns>
        public bool Fulfill<T>(List<T> items) where T : Item
        {
            switch (PrizeType)
            {
                case Item.Type.exp:
                    foreach (var item in items)
                    {
                        if (!item)
                        {
                            continue;
                        }
                        if (Name == "floatExp")
                        {
                            item.AddFloatExp(Count);
                        }
                        else item.AddExp(Count);
                    }
                    break;
                default:
                    return Fulfill();
            }
            return true;
        }

        public bool Fulfill(List<Item> items) => Fulfill<Item>(items);

        /// <summary>
        /// Get displaying name of different item
        /// </summary>
        /// <returns></returns>
        private string GetDisplayingName()
        {
            switch (PrizeType)
            {
                case Item.Type.none:
                    break;
                case Item.Type.army:
                    return GameData.Prototypes.GetArmyPrototype(Name).DisplayingName;
                case Item.Type.leader:
                    return GameData.Prototypes.GetLeaderPrototype(Name).DisplayingName;
                case Item.Type.equipment:
                    return GameData.Prototypes.GetEquipmentPrototype(Name).DisplayingName;
                case Item.Type.eventCard:
                    return GameData.Prototypes.GetEventCardPrototype(Name).DisplayingName;
                case Item.Type.story:
                    break;
                case Item.Type.currency:
                    Debug.Log(Name);
                    return ((Enum)Enum.Parse(typeof(Currency.Type), Name)).Lang();
                case Item.Type.exp:
                    return "exp";
                default:
                    break;
            }
            return "";
        }

        private Sprite GetIcon()
        {
            switch (PrizeType)
            {
                case Item.Type.none:
                    break;
                case Item.Type.army:
                    return GameData.Prototypes.GetArmyPrototype(Name).Icon;
                case Item.Type.leader:
                    return GameData.Prototypes.GetLeaderPrototype(Name).Icon;
                case Item.Type.equipment:
                    return GameData.Prototypes.GetEquipmentPrototype(Name).Icon;
                case Item.Type.eventCard:
                    return GameData.Prototypes.GetEventCardPrototype(Name).Icon;
                case Item.Type.currency:
                    Debug.Log(Name);
                    return GameData.SpriteLoader.Get(SpriteAtlases.currency, Name);
                default:
                    break;
            }
            return null;
        }

        private Rarity GetRarity()
        {
            switch (PrizeType)
            {
                case Item.Type.none:
                    break;
                case Item.Type.army:
                    return GameData.Prototypes.GetArmyPrototype(Name).Rarity;
                case Item.Type.leader:
                    return GameData.Prototypes.GetLeaderPrototype(Name).Rarity;
                case Item.Type.equipment:
                    return GameData.Prototypes.GetEquipmentPrototype(Name).Rarity;
                case Item.Type.eventCard:
                    return GameData.Prototypes.GetEventCardPrototype(Name).Rarity;
                case Item.Type.currency:
                    Debug.Log(Name);
                    return Rarity.epic;
                default:
                    break;
            }
            return Rarity.none;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Prize Clone()
        {
            return new Prize(name, count, type);
        }
    }

    [Serializable]
    public class StoryPrize : ConditionalPrize, IWeightable
    {

    }

    [Serializable]
    public class ItemPrize : ConditionalPrize, IRarityLabled, IWeightable
    {
        public Prototype Prototype => GetPrototype();

        private Prototype GetPrototype()
        {
            switch (PrizeType)
            {
                case Item.Type.army:
                    return GameData.Prototypes.GetArmyPrototype(Name);
                case Item.Type.leader:
                    return GameData.Prototypes.GetLeaderPrototype(Name);
                case Item.Type.equipment:
                    return GameData.Prototypes.GetEquipmentPrototype(Name);
            }
            return GameData.Prototypes.GetPrototype(Name);
        }

        public Rarity Rarity => Prototype.Rarity;
    }
}