using Canute.Languages;
using Canute.Shops;
using Canute.StorySystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public class ConditionalPrize : Prize
    {
        [SerializeField] private List<string> leaderRequirement;
        [SerializeField] private string armyRequirement;
        [SerializeField] private TimeInterval time;

        public ConditionalPrize() { }
        public ConditionalPrize(string name, int count, Item.Type type, List<string> leaderRequirement, string armyRequirement, TimeInterval time)
        {
            this.name = name;
            this.count = count;
            this.type = type;
            this.leaderRequirement = leaderRequirement;
            this.armyRequirement = armyRequirement;
            this.time = time;
        }


        public new ConditionalPrize Clone()
        {
            return new ConditionalPrize(name, count, type, leaderRequirement.Clone(), armyRequirement, time);
        }


        public List<string> LeaderRequirement { get => leaderRequirement; set => leaderRequirement = value; }
        public string ArmyRequirement { get => armyRequirement; set => armyRequirement = value; }
        public TimeInterval Time { get => time; set => time = value; }

    }
    [Serializable]
    public class Prize : IWeightable, ICloneable
    {
        [SerializeField] protected string name;
        [SerializeField] protected int count;
        [SerializeField] protected Item.Type type;

        public string Name => name;
        public int Count { get => count; }
        public Item.Type PrizeType { get => type; set => type = value; }
        public string DisplayingName => GetDisplayingName();


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
                    StoryDisplayer.Load(Story.Get(name));
                    //
                    break;
                case Item.Type.army:
                    Army army = GameData.Prototypes.GetArmyPrototype(Name);
                    if (!army)
                    {
                        return false;
                    }
                    Game.PlayerData.AddArmyItem(new ArmyItem(army));
                    break;
                case Item.Type.leader:
                    Leader leader = GameData.Prototypes.GetLeaderPrototype(Name);
                    if (!leader)
                    {
                        return false;
                    }
                    if (Game.PlayerData.IsLeaderUnlocked(leader.Name))
                    {
                        return false;
                    }
                    Game.PlayerData.AddLeaderItem(new LeaderItem(leader));
                    break;
                case Item.Type.equipment:
                    Equipment equipement = GameData.Prototypes.GetEquipmentPrototype(Name);
                    if (!equipement)
                    {
                        return false;
                    }
                    Game.PlayerData.AddEquipment(new EquipmentItem(equipement));
                    break;
                case Item.Type.currency:
                    Currency.Type type;
                    if (!Enum.TryParse(Name, out type))
                    {
                        return false;
                    }
                    Game.PlayerData.Earned(type, Count);
                    break;
                default:
                    break;
            }
            Debug.Log("Prize fulfilled");
            PlayerFile.SaveCurrentData();
            return true;
        }

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
                    return ((Enum)Enum.Parse(typeof(Currency), Name)).Lang();
                case Item.Type.exp:
                    return "exp";
                default:
                    break;
            }
            return "";
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