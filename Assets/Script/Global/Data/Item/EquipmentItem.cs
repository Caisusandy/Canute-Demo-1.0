using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using static Canute.Equipment;

namespace Canute
{
    [Serializable]
    public class EquipmentItem : Item, IPrototypeCopy<Equipment>, IBattleBounesItem
    {
        public EquipmentItem(Equipment equipment)
        {
            this.protoName = equipment.Name;
        }

        public Equipment Prototype { get => GameData.Prototypes.GetEquipmentPrototype(protoName); private set => protoName = value?.Name; }
        public override Prototype Proto => Prototype;
        public override int Level => GetLevel(20, 1.1, Exp, 10);
        public override Type ItemType => Type.equipment;

        public EquipmentType EquipmentType => Prototype.Type;
        public List<Army.Types> EquipmentUsage => Prototype?.EquipmentUsage;
        public List<PropertyBonus> Bonus => Prototype?.Bonus;


        public List<PropertyBonus> GetequipmentProperties()
        {
            return Prototype.Bonus;
        }

        public int GetPropertyValueAdditive(PropertyType propertyType)
        {
            foreach (var item in Bonus)
            {
                if (item.Type == propertyType)
                {
                    if (item.BonusType == BonusType.additive)
                    {
                        return item.GetValue(Level);
                    }
                }
            }
            return 0;
        }

        public double GetPropertyValuePercentage(PropertyType propertyType)
        {
            foreach (var item in Bonus)
            {
                if (item.Type == propertyType)
                {
                    if (item.BonusType == BonusType.percentage)
                    {
                        return item.GetValue(Level);
                    }
                }
            }
            return 0;
        }
    }
}

namespace Canute.BattleSystem
{
    public interface IBattleBounesItem
    {
        List<PropertyBonus> Bonus { get; }
        int Level { get; }
    }
}