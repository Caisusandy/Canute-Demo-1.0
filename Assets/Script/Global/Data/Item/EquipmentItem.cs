using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using static Canute.Equipment;

namespace Canute
{
    [Serializable]
    public class EquipmentItem : Item, IPrototypeCopy<Equipment>, IBattleBonusItem
    {
        public EquipmentItem(Equipment equipment)
        {
            this.protoName = equipment.Name;
        }

        public Equipment Prototype { get => GameData.Prototypes.GetEquipmentPrototype(protoName); private set => protoName = value?.Name; }
        public override Prototype Proto => Prototype;
        public override int Level => GetLevel(20, 1.1, Exp, 10);
        public override Type ItemType => Type.equipment;
        public bool IsUsed => ArmyUseThis();


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

        public bool CanUseBy(Army army)
        {
            if (!army)
            {
                return false;
            }
            Debug.Log(Prototype.EquipmentUsage.Contains(army.Type));
            foreach (var item in Prototype.EquipmentUsage.ToArray())
            {
                Debug.Log(Name);
                Debug.Log(item);
            }
            Debug.Log((army.Type));
            return Prototype.EquipmentUsage.Contains(army.Type);
        }

        private bool ArmyUseThis()
        {
            foreach (var item in Game.PlayerData.Armies)
            {
                foreach (var equipment in item.Equipments)
                {
                    if (equipment == this) return true;
                }
            }
            return false;
        }

        public override int CompareTo(Item other)
        {
            int ret = base.CompareTo(other);
            if (ret == 0 && other is EquipmentItem)
                return IsUsed == (other as EquipmentItem).IsUsed ? 0 : (IsUsed ? -1 : 1);

            else return ret;
        }
    }
}

namespace Canute.BattleSystem
{
    public interface IBattleBonusItem
    {
        List<PropertyBonus> Bonus { get; }
        int Level { get; }
    }
}