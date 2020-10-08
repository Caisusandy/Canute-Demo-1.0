using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public struct BattleProperty
    {

        [Flags]
        public enum AttackType
        {
            none,
            melee = 1,
            projectile = 2,

            single = 4,
            singleMelee,
            singleProjectile,

            area = 8,
            areaMelee,
            areaProjectile,

            splash = 16,
            splashMelee,
            splashProjectile,

        }

        [Flags]
        public enum Position
        {
            none = 0,
            land = 1,
            air = 2,
            all = 3,
            water = 4,
        }

        [Header("Properties")]
        [Tooltip("暴击概率")] [SerializeField] private double defense;
        [Tooltip("暴击概率")] [SerializeField] private double critRate;
        [Tooltip("暴击加成")] [SerializeField] private double critBounes;
        [Tooltip("军队人口")] [SerializeField] private int pop;
        [Header("Map related")]
        [Tooltip("攻击范围")] [SerializeField] private int attackRange;
        [Tooltip("移动范围")] [SerializeField] private int moveRange;
        [Tooltip("军队位置")] [SerializeField] private Position standPosition;
        [Tooltip("目标位置")] [SerializeField] private Position attackPosition;
        [Header("Target Related")]
        [Tooltip("群伤范围")] [SerializeField] private int attackArea;
        [Tooltip("攻击次数")] [SerializeField] private int damageExtraCount;
        [Tooltip("目标选择")] [SerializeField] private int targetCount;
        [Tooltip("攻击方式")] [SerializeField] private AttackType attackType;
        [Tooltip("技能    ")] [SerializeField] private HalfSkillEffect skill;
        [Tooltip("附加    ")] [SerializeField] private ArgList addition;



        public double Defense { get => defense; set => defense = value; }
        public double CritRate { get => critRate; set => critRate = value; }
        public double CritBonus { get => critBounes; set => critBounes = value; }
        public int Pop { get => pop; set => pop = value; }
        public int AttackRange { get => attackRange; set => attackRange = value; }
        public int MoveRange { get => moveRange; set => moveRange = value; }
        public Position StandPosition { get => standPosition; set => standPosition = value; }
        public Position AttackPosition { get => attackPosition; set => attackPosition = value; }
        public AttackType Attack { get => attackType; set => attackType = value; }
        public int DamageExtraCount { get => damageExtraCount; set => damageExtraCount = value; }
        public int AttackArea { get => attackArea; set => attackArea = value; }
        public int TargetCount { get => targetCount; set => targetCount = value; }
        public HalfSkillEffect SkillPack => skill;
        public ArgList Addition { get => addition; set => addition = value; }

        public BattleProperty(Army army)
        {
            defense = 0;
            critRate = 20;
            critBounes = 50;
            attackRange = 3;
            moveRange = 4;
            pop = 1;
            attackArea = 1;
            damageExtraCount = 1;
            targetCount = 1;

            standPosition = Position.none;
            attackPosition = Position.none;
            attackType = AttackType.melee;

            skill = new HalfSkillEffect();
            addition = new ArgList();
        }

        public BattleProperty(ArmyItem armyItem)
        {
            defense = armyItem.BaseProperty.Defense;
            critRate = armyItem.BaseProperty.CritRate;
            critBounes = armyItem.BaseProperty.CritBonus;
            attackRange = armyItem.BaseProperty.AttackRange;
            moveRange = armyItem.BaseProperty.MoveRange;
            pop = armyItem.BaseProperty.Pop;
            standPosition = armyItem.BaseProperty.StandPosition;
            attackPosition = armyItem.BaseProperty.AttackPosition;

            attackType = armyItem.BaseProperty.Attack;
            attackArea = armyItem.BaseProperty.AttackArea;
            targetCount = armyItem.BaseProperty.TargetCount;
            damageExtraCount = armyItem.BaseProperty.DamageExtraCount;

            skill = armyItem.BaseProperty.SkillPack;
            addition = armyItem.BaseProperty.Addition;
        }

        /// <summary>
        /// use for display info only
        /// </summary>
        /// <param name="armyItem"></param>
        /// <param name="equipment"></param>
        public BattleProperty(ArmyItem armyItem, ArmyItemEquipmentSlot equipment)
        {
            defense = armyItem.BaseProperty.Defense.Bonus(armyItem.LevelBonus);
            critRate = armyItem.BaseProperty.CritRate;
            critBounes = armyItem.BaseProperty.CritBonus;
            attackRange = armyItem.BaseProperty.AttackRange;
            moveRange = armyItem.BaseProperty.MoveRange;
            pop = armyItem.BaseProperty.Pop;
            standPosition = armyItem.BaseProperty.StandPosition;
            attackPosition = armyItem.BaseProperty.AttackPosition;

            attackType = armyItem.BaseProperty.Attack;
            attackArea = armyItem.BaseProperty.AttackArea;
            targetCount = armyItem.BaseProperty.TargetCount;
            damageExtraCount = armyItem.BaseProperty.DamageExtraCount;

            skill = armyItem.BaseProperty.SkillPack;
            addition = armyItem.BaseProperty.Addition;

            AddBonus(equipment.ToArray());
        }

        public bool Equals(BattleProperty obj)
        {
            return CritRate == obj.CritRate &&
                CritBonus == obj.CritBonus &&
                AttackRange == obj.AttackRange &&
                MoveRange == obj.MoveRange &&
                Pop == obj.Pop &&
                StandPosition == obj.StandPosition &&
                AttackPosition == obj.AttackPosition &&
                Attack == obj.Attack &&
                Attack == obj.Attack;
        }
        public override bool Equals(object obj)
        {
            if (!(obj is BattleProperty))
            {
                return false;
            }
            return Equals((BattleProperty)obj);
        }

        public static bool operator ==(BattleProperty left, BattleProperty right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BattleProperty left, BattleProperty right)
        {
            return !(left == right);
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public void AddBonus(params IBattleBonusItem[] bonuses)
        {
            if (bonuses is null) return;

            var propertyBonuses = new List<PropertyBonus>();
            foreach (var (item, property) in from item in bonuses from property in item?.Bonus select (item, property))
            {
                propertyBonuses.Add(property.ConvertToLevel1(item.Level));
            }

            AddBonus(propertyBonuses.ToArray());
        }

        public void AddBonus(params PropertyBonus[] propertyBonuses)
        {
            Array.Sort(propertyBonuses);
            foreach (var (property, type) in from property in propertyBonuses from PropertyType type in PropertyTypes.Types select (property, type))
            {
                if ((property.Type & type) == PropertyType.none) continue;
                switch (property.Type & type)
                {
                    case PropertyType.defense:
                        Defense = property.Bonus(Defense);
                        break;
                    case PropertyType.moveRange:
                        MoveRange = property.Bonus(MoveRange);
                        break;
                    case PropertyType.attackRange:
                        AttackRange = property.Bonus(AttackRange);
                        break;
                    case PropertyType.critRate:
                        CritRate = property.Bonus(CritRate);
                        break;
                    case PropertyType.critBonus:
                        CritBonus = property.Bonus(CritBonus);
                        break;
                    default:
                        break;
                }
            }
        }

        public void RemoveBonus(params IBattleBonusItem[] bonuses)
        {
            if (bonuses is null) return;

            var propertyBonuses = new List<PropertyBonus>();
            foreach (var (item, property) in from item in bonuses from property in item?.Bonus select (item, property))
            {
                propertyBonuses.Add(property.ConvertToLevel1(item.Level));
            }

            RemoveBonus(propertyBonuses.ToArray());
        }

        public void RemoveBonus(params PropertyBonus[] propertyBonuses)
        {
            Array.Sort(propertyBonuses);
            foreach (var (property, type) in from property in propertyBonuses from PropertyType type in PropertyTypes.Types select (property, type))
            {
                if ((property.Type & type) == PropertyType.none) continue;
                switch (property.Type & type)
                {
                    case PropertyType.defense:
                        Defense = property.RemoveBonus(Defense);
                        break;
                    case PropertyType.moveRange:
                        MoveRange = property.RemoveBonus(MoveRange);
                        break;
                    case PropertyType.attackRange:
                        AttackRange = property.RemoveBonus(AttackRange);
                        break;
                    case PropertyType.critRate:
                        CritRate = property.RemoveBonus(CritRate);
                        break;
                    case PropertyType.critBonus:
                        CritBonus = property.RemoveBonus(CritBonus);
                        break;
                    default:
                        break;
                }
                Debug.Log(property);
            }
        }
    }


    public interface IBattlePropertyContainer
    {
        BattleProperty Properties { get; }
    }

    public interface IArmy : IBattlePropertyContainer
    {
        Army.Types Type { get; }
    }

}

