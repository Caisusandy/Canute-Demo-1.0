﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{

    [Serializable]
    public abstract class BattleEntityData : BattleableEntityData, IBattleEntityData, IPassiveEntityData, IAggressiveEntityData
    {
        [Tooltip("军队最大血量")] [SerializeField] protected int maxHealth;
        [Tooltip("军队当前血量")] [SerializeField] protected int health;
        [Tooltip("护甲值")] [SerializeField] protected int armor;
        [Tooltip("攻击值")] [SerializeField] protected int damage;

        public virtual int MaxHealth => maxHealth;
        public virtual int Defense => (int)Properties.Defense;
        public virtual int RawDamage { get => damage; set => damage = value; }
        public virtual float HealthPercent => ((float)health) / MaxHealth;
        public virtual int Health { get => health; set => health = value < 0 ? 0 : (value > MaxHealth ? MaxHealth : value); }
        public virtual int Armor { get => armor; set => armor = value < 0 ? 0 : (value > MaxHealth ? MaxHealth : value); }

        public virtual int Damage => this.GetDamage();

        public virtual double CritRate => Properties.CritRate;
        public virtual BattleProperty.AttackType AttackType => Properties.Attack;
        public virtual BattleProperty.Position AttackPosition => Properties.AttackPosition;


        protected BattleEntityData() : base() { }

        protected BattleEntityData(Prototype prototype) : base(prototype) { }

        public override void CheckPotentialAction(params object[] vs)
        {
            if (Health <= 0)
            {
                Debug.Log("Died");
                Die();
            }
            else if (Anger == 100)
            {
                Debug.Log("Skill");
                PerformSkill();
            }
        }

        public virtual void Die()
        {
            IDefeatable entity = Entity as IDefeatable;
            entity.KillEntity();
        }

        protected override void AddBounes(params IBattleBounesItem[] bouneses)
        {
            if (bouneses is null)
            {
                return;
            }

            foreach (var (item, property, type) in from item in bouneses
                                                   from property in item.Bounes
                                                   from PropertyType type in PropertyTypes.Types
                                                   select (item, property, type))
            {
                //Debug.Log(property);
                //Debug.Log(item.Bounes.Count);
                switch (property.Type & type)
                {
                    case PropertyType.damage:
                        damage = property.Bounes(damage, item.Level);
                        break;
                    case PropertyType.health:
                        maxHealth = property.Bounes(maxHealth, item.Level);
                        break;
                    case PropertyType.defense:
                        properties.Defense = property.Bounes(properties.Defense, item.Level);
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
                        properties.CritBonus = property.Bounes(properties.CritBonus, item.Level);
                        break;
                    default:
                        break;
                }


            }
        }

        protected override void RemoveBounes(params IBattleBounesItem[] bouneses)
        {
            if (bouneses is null)
            {
                return;
            }

            foreach (var (item, property, type) in from item in bouneses
                                                   from property in item.Bounes
                                                   from PropertyType type in PropertyTypes.Types
                                                   select (item, property, type))
            {
                switch (property.Type & type)
                {
                    case PropertyType.damage:
                        damage = property.RemoveBounes(damage, item.Level);
                        break;
                    case PropertyType.health:
                        maxHealth = property.RemoveBounes(maxHealth, item.Level);
                        break;
                    case PropertyType.defense:
                        properties.Defense = property.Bounes(properties.Defense, item.Level);
                        break;
                    case PropertyType.attackRange:
                        properties.AttackRange = property.RemoveBounes(properties.AttackRange, item.Level);
                        break;
                    case PropertyType.critRate:
                        properties.CritRate = property.RemoveBounes(properties.CritRate, item.Level);
                        break;
                    case PropertyType.critBounes:
                        properties.CritBonus = property.RemoveBounes(properties.CritBonus, item.Level);
                        break;
                    default:
                        break;
                }
                Debug.Log(property);
            }
        }

        public List<CellEntity> GetAttackArea() => MapEntity.CurrentMap.GetAttackArea(MapEntity.CurrentMap[Coordinate], Properties.AttackRange, this);
    }

    /// <summary> 完备的上战场的接口 </summary>
    public interface IBattleEntityData : IAggressiveEntityData, IPassiveEntityData
    {

    }



    public static class EntityDatas
    {
        public static int Bonus(this int @base, int rate, BonusType type = BonusType.percentage)
        {
            switch (type)
            {
                case BonusType.percentage:
                    return (int)(@base * (1 + rate / 100d));
                case BonusType.additive:
                    return (int)(@base + rate);
                default:
                    goto case BonusType.additive;
            }
        }

        public static int Bonus(this int @base, double rate, BonusType type = BonusType.percentage)
        {
            switch (type)
            {
                case BonusType.percentage:
                    return (int)(@base * (1 + rate / 100d));
                case BonusType.additive:
                    return (int)(@base + rate);
                default:
                    goto case BonusType.additive;
            }
        }

        public static int Bonus(this double @base, int rate, BonusType type = BonusType.percentage)
        {
            switch (type)
            {
                case BonusType.percentage:
                    return (int)(@base * (1 + rate / 100d));
                case BonusType.additive:
                    return (int)(@base + rate);
                default:
                    goto case BonusType.additive;
            }
        }

        public static int Bonus(this double @base, double rate, BonusType type = BonusType.percentage)
        {
            switch (type)
            {
                case BonusType.percentage:
                    return (int)(@base * (1 + rate / 100d));
                case BonusType.additive:
                    return (int)(@base + rate);
                default:
                    goto case BonusType.additive;
            }
        }

        public static int RemoveBonus(this int @base, int rate, BonusType type = BonusType.percentage)
        {
            switch (type)
            {
                case BonusType.percentage:
                    return (int)(@base / (1 + rate / 100d));
                case BonusType.additive:
                    return (int)(@base - rate);
                default:
                    goto case BonusType.additive;
            }
        }

        public static int RemoveBonus(this int @base, double rate, BonusType type = BonusType.percentage)
        {
            switch (type)
            {
                case BonusType.percentage:
                    return (int)(@base / (1 + rate / 100d));
                case BonusType.additive:
                    return (int)(@base - rate);
                default:
                    goto case BonusType.additive;
            }
        }

        public static int RemoveBonus(this double @base, int rate, BonusType type = BonusType.percentage)
        {
            switch (type)
            {
                case BonusType.percentage:
                    return (int)(@base / (1 + rate / 100d));
                case BonusType.additive:
                    return (int)(@base - rate);
                default:
                    goto case BonusType.additive;
            }
        }

        public static int RemoveBonus(this double @base, double rate, BonusType type = BonusType.percentage)
        {
            Debug.Log(@base + "-" + rate);
            switch (type)
            {
                case BonusType.percentage:
                    return (int)(@base / (1 + rate / 100d));
                case BonusType.additive:
                    return (int)(@base - rate);
                default:
                    goto case BonusType.additive;
            }
        }
    }
}