using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public abstract class AggressiveEntityData : BattleableEntityData, IAggressiveEntityData
    {
        [SerializeField] protected int damage;

        public virtual int RawDamage => damage;
        public virtual BattleProperty.AttackType AttackType => Properties.Attack;
        public virtual BattleProperty.Position AttackPosition => Properties.AttackPosition;
        public virtual int Damage => this.GetDamage();

        protected override void AddBounes(params IBattleBounesItem[] bouneses)
        {
            if (bouneses is null)
            {
                return;
            }

            foreach ((IBattleBounesItem item, PropertyBounes property, PropertyType type) in from item in bouneses
                                                                                             from property in item.Bounes
                                                                                             from type in PropertyTypes.Types
                                                                                             select (item, property, type))
            {
                Debug.Log(property.Type);
                switch (property.Type & type)
                {
                    case PropertyType.damage:
                        damage = property.Bounes(damage, item.Level);
                        break;
                    case PropertyType.moveRange:
                        properties.MoveRange = property.Bounes(properties.MoveRange, item.Level);
                        break;
                    case PropertyType.attackRange:
                        properties.AttackRange = property.Bounes(properties.AttackRange, item.Level);
                        break;
                    case PropertyType.critRate:
                        properties.CritRate = property.Bounes(properties.CritRate, item.Level);
                        break;
                    case PropertyType.critBounes:
                        properties.CritBounes = property.Bounes(properties.CritBounes, item.Level);
                        break;
                    default:
                        break;
                }
            }
        }

        public List<CellEntity> GetAttackArea() => MapEntity.CurrentMap.GetAttackArea(MapEntity.CurrentMap[Coordinate], Properties.AttackRange, this);

        protected AggressiveEntityData() : base() { }

        protected AggressiveEntityData(Prototype prototype) : base(prototype) { }
    }

    /// <summary> 能攻击的实体的数据的接口 </summary>
    public interface IAggressiveEntityData : IBattleableEntityData, IStatusContainer, IOwnable
    {
        /// <summary> 军队基础的伤害点数 </summary>
        int RawDamage { get; }
        /// <summary> 获取一次军队可造成的伤害点数（不计算Status的加成） </summary>
        int Damage { get; }

        /// <summary> 攻击方式（选择目标的方式）</summary>
        //ArmyProperty.TargetTypes TargetType { get; }

        List<CellEntity> GetAttackArea();

        void PerformSkill();
    }

    public static class AgressiveEntitiesData
    {
        /// <summary>
        /// Get Raw Damage of this Army (without any buff, but may with crit)
        /// </summary>
        /// <returns>Raw Damage</returns>
        public static int GetDamage(this IAggressiveEntityData agressiveEntity)
        {
            int damage = agressiveEntity.RawDamage;
            bool isCritical = UnityEngine.Random.Range(0, 100) <= agressiveEntity.Properties.CritRate * agressiveEntity.Owner.Morale;

            if (isCritical)
            {
                damage = damage.Bonus(agressiveEntity.Properties.CritBounes);
            }

            return damage;
        }

        public static bool CanAttack(this IAggressiveEntityData data, IPassiveEntityData target)
        {
            return data.Properties.AttackPosition.IsTypeOf(target.StandPosition);
        }
    }

    //public struct Damage
    //{
    //    int value;
    //    bool isCritical;

    //    public int Value { get => value; set => this.value = value; }
    //    public bool IsCritical { get => isCritical; set => isCritical = value; }

    //    public Damage(int value, bool isCritical)
    //    {
    //        this.value = value;
    //        this.isCritical = isCritical;
    //    }

    //    public static implicit operator int(Damage damage)
    //    {
    //        return damage.value;
    //    }


    //    public static bool operator ==(Damage left, Damage right)
    //    {
    //        return left.Equals(right);
    //    }

    //    public static bool operator !=(Damage left, Damage right)
    //    {
    //        return !(left == right);
    //    }
    //}
}