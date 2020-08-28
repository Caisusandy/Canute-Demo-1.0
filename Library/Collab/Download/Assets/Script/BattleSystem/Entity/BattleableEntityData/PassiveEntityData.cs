using System;
using UnityEngine;
using System.Linq;

namespace Canute.BattleSystem
{
    [Serializable]
    public abstract class PassiveEntityData : BattleableEntityData, IPassiveEntityData
    {
        [SerializeField] protected int maxHealth;

        [SerializeField] protected int health;
        [SerializeField] protected int defense;
        [SerializeField] protected int armor;
        [SerializeField] protected bool canBeTargeted;

        public virtual int MaxHealth => maxHealth;
        public virtual int Defense => defense;
        public virtual float HealthPercent => ((float)health) / MaxHealth;
        public virtual int Health { get => health; set => health = value < 0 ? 0 : (value > MaxHealth ? MaxHealth : value); }
        public virtual int Armor { get => armor; set => armor = value < 0 ? 0 : value; }
        public virtual bool CanBeTargeted { get => canBeTargeted; set => canBeTargeted = value; }

        protected PassiveEntityData() : base() { }

        protected PassiveEntityData(Prototype prototype) : base(prototype) { }

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
                switch (property.Type & type)
                {
                    case PropertyType.moveRange:
                        properties.MoveRange = property.Bounes(properties.MoveRange, item.Level);
                        break;
                    case PropertyType.health:
                        maxHealth = property.Bounes(maxHealth, item.Level);
                        break;
                    case PropertyType.defense:
                        defense = property.Bounes(defense, item.Level);
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
                    case PropertyType.moveRange:
                        properties.MoveRange = property.RemoveBounes(properties.MoveRange, item.Level);
                        break;
                    case PropertyType.health:
                        maxHealth = property.RemoveBounes(maxHealth, item.Level);
                        break;
                    case PropertyType.defense:
                        defense = property.RemoveBounes(defense, item.Level);
                        break;
                    default:
                        break;
                }
            }
        }
    }


    /// <summary> 能被攻击的实体的数据的接口 </summary>
    public interface IPassiveEntityData : IBattleableEntityData, IStatusContainer, IOwnable
    {
        /// <summary> 生命值 </summary>
        int Health { get; set; }
        /// <summary> 最高生命值 </summary>
        int MaxHealth { get; }
        /// <summary> 防御值 </summary>
        int Defense { get; }
        /// <summary> 护甲值 </summary>
        int Armor { get; set; }
        /// <summary>  </summary>
        float HealthPercent { get; }
    }

    public static class PassiveEntitiesData
    {
        public static int Damage(this IPassiveEntityData passiveEntity, int damagePoint)
        {
            passiveEntity.Health -= damagePoint;

            Debug.Log("Harm: " + damagePoint + ":" + passiveEntity.ToString());
            return damagePoint;
        }

        public static int DamageArmor(this IPassiveEntityData passiveEntity, int damagePoint)
        {
            int dArmor = passiveEntity.Armor;
            if (passiveEntity.Armor != 0)
            {
                if (passiveEntity.Armor >= damagePoint)
                {
                    passiveEntity.Armor -= damagePoint;
                }
                else
                {
                    passiveEntity.Armor = 0;
                }
            }
            dArmor -= passiveEntity.Armor;
            Debug.Log("Harm Armor: " + dArmor + ":" + passiveEntity.ToString());
            return dArmor;
        }

        public static int GetDamageAfterDefensePoint(this IPassiveEntityData passiveEntity, int damagePoint)
        {
            return Mathf.Max(1, damagePoint - passiveEntity.Defense);
        }


        public static void RealDamage(this IPassiveEntityData passiveEntity, int damagePoint)
        {
            passiveEntity.Health -= damagePoint;
        }
    }
}