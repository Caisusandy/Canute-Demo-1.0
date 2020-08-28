using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{
    //[Serializable]
    //public abstract class AggressiveEntityData : BattleableEntityData, IAggressiveEntityData
    //{
    //    [SerializeField] protected int damage;

    //    public virtual int RawDamage { get => damage; set => damage = value; }
    //    public virtual BattleProperty.AttackType AttackType => Properties.Attack;
    //    public virtual BattleProperty.Position AttackPosition => Properties.AttackPosition;
    //    public virtual int Damage => this.GetDamage();
    //    public virtual double CritRate => Properties.CritRate;

    //    protected override void AddBounes(params IBattleBounesItem[] bouneses)
    //    {
    //        if (bouneses is null)
    //        {
    //            return;
    //        }

    //        foreach ((IBattleBounesItem item, PropertyBonus property, PropertyType type) in from item in bouneses
    //                                                                                         from property in item.Bonus
    //                                                                                         from type in PropertyTypes.Types
    //                                                                                         select (item, property, type))
    //        {
    //            Debug.Log(property.Type);
    //            switch (property.Type & type)
    //            {
    //                case PropertyType.damage:
    //                    damage = property.Bonus(damage, item.Level);
    //                    break;
    //                case PropertyType.moveRange:
    //                    properties.MoveRange = property.Bonus(properties.MoveRange, item.Level);
    //                    break;
    //                case PropertyType.attackRange:
    //                    properties.AttackRange = property.Bonus(properties.AttackRange, item.Level);
    //                    break;
    //                case PropertyType.critRate:
    //                    properties.CritRate = property.Bonus(properties.CritRate, item.Level);
    //                    break;
    //                case PropertyType.critBounes:
    //                    properties.CritBonus = property.Bonus(properties.CritBonus, item.Level);
    //                    break;
    //                default:
    //                    break;
    //            }
    //        }
    //    }

    //    public List<CellEntity> GetAttackArea() => MapEntity.CurrentMap.GetAttackArea(MapEntity.CurrentMap[Coordinate], Properties.AttackRange, this);

    //    protected AggressiveEntityData() : base() { }

    //    protected AggressiveEntityData(Prototype prototype) : base(prototype) { }
    //}

    /// <summary> 能攻击的实体的数据的接口 </summary>
    public interface IAggressiveEntityData : IBattleableEntityData, IStatusContainer, IOwnable
    {
        /// <summary> 军队基础的伤害点数 </summary>
        int RawDamage { get; set; }
        /// <summary> 获取一次军队可造成的伤害点数（不计算Status的加成） </summary>
        int Damage { get; }
        /// <summary> 可以攻击的位置 </summary>
        BattleProperty.Position AttackPosition { get; }
        /// <summary> 攻击方式 </summary>
        BattleProperty.AttackType AttackType { get; }
        /// <summary> Critical Damage Rate </summary>
        double CritRate { get; }

        /// <summary> 攻击方式（选择目标的方式）</summary>
        //ArmyProperty.TargetTypes TargetType { get; }
        /// <summary> 攻击范围内所有格子 </summary>
        List<CellEntity> GetAttackArea();
        /// <summary> 执行大招 </summary>
        void PerformSkill();
    }

    public interface IAggressiveEntity : IBattleableEntity
    {
        /// <summary> Entity Data </summary>
        new IAggressiveEntityData Data { get; }
        /// <summary> 重写攻击目标 </summary> <param name="effect"> 攻击效果参数 </param>
        void GetAttackTarget(ref Effect effect);
        /// <summary> 攻击范围内所有格子 </summary>
        List<CellEntity> GetAttackArea();
        /// <summary> 攻击 </summary>
        void Attack(params object[] vs);

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
            bool isCritical = UnityEngine.Random.Range(0, 100) <= agressiveEntity.Properties.CritRate;// * agressiveEntity.Owner.Morale;

            if (isCritical)
            {
                damage = damage.Bonus(agressiveEntity.Properties.CritBonus);
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