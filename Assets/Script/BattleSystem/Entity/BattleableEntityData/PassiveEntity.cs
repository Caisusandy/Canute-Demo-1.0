using System;
using UnityEngine;
using System.Linq;
using System.Collections.Generic;

namespace Canute.BattleSystem
{
    public delegate void DamageEvent(IBattleableEntity source, IPassiveEntity target, int value);
    public delegate void DefeatEvent(IBattleableEntity source, IPassiveEntity target);

    //[Serializable]
    //public abstract class PassiveEntityData : BattleableEntityData, IPassiveEntityData
    //{
    //    [SerializeField] protected int maxHealth;

    //    [SerializeField] protected int health;
    //    [SerializeField] protected int defense;
    //    [SerializeField] protected int armor;
    //    [SerializeField] protected bool canBeTargeted;

    //    public virtual int MaxHealth => maxHealth;
    //    public virtual int Defense => defense;
    //    public virtual float HealthPercent => ((float)health) / MaxHealth;
    //    public virtual int Health { get => health; set => health = value < 0 ? 0 : (value > MaxHealth ? MaxHealth : value); }
    //    public virtual int Armor { get => armor; set => armor = value < 0 ? 0 : value; }
    //    public virtual bool CanBeTargeted { get => canBeTargeted; set => canBeTargeted = value; }

    //    protected PassiveEntityData() : base() { }

    //    protected PassiveEntityData(Prototype prototype) : base(prototype) { }

    //    protected override void AddBounes(params IBattleBounesItem[] bouneses)
    //    {
    //        if (bouneses is null)
    //        {
    //            return;
    //        }

    //        foreach (var (item, property, type) in from item in bouneses
    //                                               from property in item.Bonus
    //                                               from PropertyType type in PropertyTypes.Types
    //                                               select (item, property, type))
    //        {
    //            switch (property.Type & type)
    //            {
    //                case PropertyType.moveRange:
    //                    properties.MoveRange = property.Bonus(properties.MoveRange, item.Level);
    //                    break;
    //                case PropertyType.health:
    //                    maxHealth = property.Bonus(maxHealth, item.Level);
    //                    break;
    //                case PropertyType.defense:
    //                    defense = property.Bonus(defense, item.Level);
    //                    break;
    //                default:
    //                    break;
    //            }
    //        }
    //    }
    //    protected override void RemoveBounes(params IBattleBounesItem[] bouneses)
    //    {
    //        if (bouneses is null)
    //        {
    //            return;
    //        }

    //        foreach (var (item, property, type) in from item in bouneses
    //                                               from property in item.Bonus
    //                                               from PropertyType type in PropertyTypes.Types
    //                                               select (item, property, type))
    //        {
    //            switch (property.Type & type)
    //            {
    //                case PropertyType.moveRange:
    //                    properties.MoveRange = property.RemoveBonus(properties.MoveRange, item.Level);
    //                    break;
    //                case PropertyType.health:
    //                    maxHealth = property.RemoveBonus(maxHealth, item.Level);
    //                    break;
    //                case PropertyType.defense:
    //                    defense = property.RemoveBonus(defense, item.Level);
    //                    break;
    //                default:
    //                    break;
    //            }
    //        }
    //    }
    //}

    /// <summary> 能被攻击的实体的数据的接口 </summary>
    public interface IPassiveEntityData : IBattleableEntityData, IStatusContainer, IOwnable
    {
        /// <summary> 最高生命值 </summary>
        int MaxHealth { get; }
        /// <summary>  </summary>
        float HealthPercent { get; }
    }

    public interface IAggressiveEntity : IBattleableEntity
    {
        new IAggressiveEntityData Data { get; }

        List<CellEntity> GetAttackArea();
        void GetAttackTarget(ref Effect effect);
        void Attack(params object[] vs);
    }

    public interface IPassiveEntity : IBattleableEntity, IDefeatable, ISkillable
    {
        new IPassiveEntityData Data { get; }

        void Hurt(params object[] vs);
    }

    public static class PassiveEntities
    {
        public static event DamageEvent DamageEvent;
        public static event DefeatEvent DefeatEvent;

        public static int FinalDamage(this IPassiveEntity passive, int damagePoint, IBattleableEntity source = null)
        {
            passive.Data.Health -= damagePoint;
            DamageEvent?.Invoke(source, passive, damagePoint);
            Debug.Log("Harm: " + damagePoint + ":" + passive.ToString());
            return damagePoint;
        }

        public static int DamageArmor(this IPassiveEntity passive, int damagePoint)
        {
            int dArmor = passive.Data.Armor;
            if (passive.Data.Armor != 0)
            {
                if (passive.Data.Armor >= damagePoint)
                {
                    passive.Data.Armor -= damagePoint;
                }
                else
                {
                    passive.Data.Armor = 0;
                }
            }
            dArmor -= passive.Data.Armor;
            Debug.Log("Harm Armor: " + dArmor + ":" + passive.ToString());
            return dArmor;
        }

        public static int GetDamageAfterDefensePoint(this IPassiveEntity passive, int damagePoint)
        {
            return Mathf.Max(1, damagePoint - passive.Data.Defense);
        }

        public static void Die(this IPassiveEntity passive, params object[] vs)
        {
            IDefeatable entity = passive as IDefeatable;
            if (entity is null)
            {
                return;
            }

            IBattleableEntity source;
            entity.KillEntity();

            try { source = vs[0] as IBattleableEntity; }
            catch { source = null; }
            DefeatEvent?.Invoke(source, passive);
        }


        public static void Damage(this IPassiveEntity passiveEntity, int damage, IBattleableEntity source = null)
        {
            int finalDamageIfCrit = damage;
            if (source is IAggressiveEntity)
            {
                IAggressiveEntity sourceEntity = source as IAggressiveEntity;
                finalDamageIfCrit = finalDamageIfCrit.Bonus(UnityEngine.Random.value < sourceEntity.Data.Properties.CritRate ? 0 : sourceEntity.Data.Properties.CritBonus, BonusType.percentage);

            }
            int flow = (int)(finalDamageIfCrit);// * Random.Range(0.95f, 1.05f));
            int armorHit = passiveEntity.DamageArmor(flow);

            flow -= armorHit;
            flow = passiveEntity.GetDamageAfterDefensePoint(flow);

            passiveEntity.FinalDamage(flow, source);
            passiveEntity.DisplayDamage(flow);

            if (armorHit != 0) passiveEntity.DisplayDamage(armorHit);
        }

        public static void DisplayDamage(this IPassiveEntity passiveEntity, int damage)
        {
            Debug.Log(damage);
            GameObject displayer = UnityEngine.Object.Instantiate(GameData.Prefabs.Get("armyDamageDisplayer"), passiveEntity.transform);
            displayer.GetComponent<ArmyDamageDisplayer>().damage = damage;
            //Debug.Log("Display Damage");
        }
    }
}