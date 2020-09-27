using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{

    [Serializable]
    public abstract class BattleEntityData : OnMapEntityData, IBattleEntityData, IPassiveEntityData, IAggressiveEntityData, IBattleableEntityData, ICareerLabled, IBattlePropertyContainer
    {
        public enum AutonomousType
        {
            none,
            idle,
            attack,
            patrol,
        }

        [Header("Leader")]
        [SerializeField] protected BattleLeader localLeader;
        [Header("Autonomous")]
        [SerializeField] protected AutonomousType autonomousType;
        [Header("Battle Entity Properties")]
        [SerializeField] protected int anger;
        [SerializeField] protected Career career;
        [SerializeField] protected BattleProperty properties;

        [Tooltip("军队最大血量")]
        [SerializeField] protected int maxHealth;
        [Tooltip("军队当前血量")]
        [SerializeField] protected int health;
        [Tooltip("护甲值")]
        [SerializeField] protected int armor;
        [Tooltip("攻击值")]
        [SerializeField] protected int damage;

        public Career Career => !IsNullOrEmpty(localLeader) ? (LocalLeader.Career != Career.none ? LocalLeader.Career : career) : career;
        public virtual BattleProperty RawProperties { get => properties; set => properties = value; }
        public virtual BattleProperty Properties => EffectExecute.GetProperty(this);
        public virtual BattleProperty.Position StandPosition => Properties.StandPosition;


        public virtual BattleLeader LocalLeader { get => localLeader; set => localLeader = value; }
        public virtual BattleLeader ViceCommander => Owner?.ViceCommander;
        public virtual HalfSkillEffect SkillPack { get => Properties.SkillPack; }
        public virtual AutonomousType Autonomous { get => autonomousType; set => autonomousType = value; }
        public new OnMapEntity Entity => BattleSystem.Entity.Get(uuid) as OnMapEntity;
        public virtual int Anger { get => anger; set => anger = value < 0 ? 0 : value >= 100 ? 100 : value; }

        public virtual int MaxHealth { get => maxHealth; set => maxHealth = value; }
        public virtual int Defense => (int)Properties.Defense;
        public virtual int RawDamage { get => damage; set => damage = value; }
        public virtual float HealthPercent => ((float)health) / MaxHealth;
        public virtual int Health { get => health; set => health = value < 0 ? 0 : (value > MaxHealth ? MaxHealth : value); }
        public virtual int Armor { get => armor; set => armor = value < 0 ? 0 : value; }

        public virtual int Damage => this.GetDamage();

        public virtual double CritRate => Properties.CritRate;
        public virtual BattleProperty.AttackType AttackType => Properties.Attack;
        public virtual BattleProperty.Position AttackPosition => Properties.AttackPosition;


        protected BattleEntityData() : base() { autonomousType = AutonomousType.none; }

        protected BattleEntityData(Prototype prototype) : base(prototype) { autonomousType = AutonomousType.none; }

        public virtual void PerformSkill()
        {
            Effect effect = SkillPack;
            Entity entity = Entity;

            IBattleableEntity battleable = entity as IBattleableEntity;
            if (battleable is null)
            {
                Debug.LogError("an entity with an imposible identity tried to perform skill " + ToString());
                return;
            }

            effect.Source = entity;
            effect.Target = entity;
            effect.Execute(true);
            Anger = 0;
        }

        public List<CellEntity> GetMoveArea() => MapEntity.CurrentMap.GetMoveArea(MapEntity.CurrentMap[Coordinate], Properties.MoveRange, this);

        public virtual void CheckPotentialAction(params object[] vs)
        {
            if (Health <= 0)
            {
                Debug.Log("Died");
                (Entity as IPassiveEntity).Die(vs);
            }
            else if (Anger == 100)
            {
                Debug.Log("anger is ready for skill");
                PerformSkill();
            }
        }

        protected virtual void AddBonus(params IBattleBonusItem[] bonuses)
        {
            if (bonuses is null) return;

            var propertyBonuses = new List<PropertyBonus>();
            foreach (var (item, property) in from item in bonuses from property in item?.Bonus select (item, property))
            {
                propertyBonuses.Add(property.ConvertToLevel1(item.Level));
            }

            propertyBonuses.Sort();
            foreach (var (propertyBonus, type) in from property in propertyBonuses from PropertyType type in PropertyTypes.Types select (property, type))
            {
                if ((propertyBonus.Type & type) == PropertyType.none) continue;
                //Debug.Log(property);
                //Debug.Log(item.Bounes.Count);
                switch (propertyBonus.Type & type)
                {
                    case PropertyType.damage:
                        damage = propertyBonus.Bonus(damage);
                        break;
                    case PropertyType.health:
                        maxHealth = propertyBonus.Bonus(maxHealth);
                        break;
                    case PropertyType.defense:
                    case PropertyType.moveRange:
                    case PropertyType.attackRange:
                    case PropertyType.critRate:
                    case PropertyType.critBonus:
                        properties.AddBonus(propertyBonus);
                        break;
                }


            }
        }

        protected virtual void RemoveBonus(params IBattleBonusItem[] bonuses)
        {
            if (bonuses is null) return;

            var propertyBonuses = new List<PropertyBonus>();
            foreach (var (item, property) in from item in bonuses from property in item?.Bonus select (item, property))
            {
                propertyBonuses.Add(property.ConvertToLevel1(item.Level));
            }

            propertyBonuses.Sort();
            foreach (var (propertyBonus, type) in from property in propertyBonuses from PropertyType type in PropertyTypes.Types select (property, type))
            {
                if ((propertyBonus.Type & type) == PropertyType.none) continue;
                switch (propertyBonus.Type & type)
                {
                    case PropertyType.damage:
                        damage = propertyBonus.RemoveBonus(damage);
                        break;
                    case PropertyType.health:
                        maxHealth = propertyBonus.RemoveBonus(maxHealth);
                        break;
                    case PropertyType.defense:
                    case PropertyType.moveRange:
                    case PropertyType.attackRange:
                    case PropertyType.critRate:
                    case PropertyType.critBonus:
                        properties.RemoveBonus(propertyBonus);
                        break;
                }
                Debug.Log(propertyBonus);
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