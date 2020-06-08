using System;
using System.Collections.Generic;

namespace Canute.BattleSystem
{
    [Serializable]
    public class EquipmentItem : Item, IPrototypeCopy<Equipment>, IBattleBounesItem
    {
        public Equipment Prototype { get => GameData.Prototypes.GetEquipmentPrototype(protoName); private set => protoName = value?.Name; }
        public override Prototype Proto => Prototype;
        public override int Level => GetLevel(20, 1.1, Exp, 10);
        public override Type ItemType => Type.Equipment;

        public Equipment.EquipmentType EquipmentType => Prototype.Type;
        public List<PropertyBounes> Bounes => Prototype.Bouneses;


        public List<PropertyBounes> GetequipmentProperties()
        {
            return Prototype.Bouneses;
        }

    }

    public interface IBattleBounesItem
    {
        List<PropertyBounes> Bounes { get; }
        int Level { get; }
    }
}