using System;
using System.Collections.Generic;
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
        [Tooltip("目标选择")] [SerializeField] private int targetCount;
        [Tooltip("攻击方式")] [SerializeField] private AttackType attackType;
        [Tooltip("技能    ")] [SerializeField] private HalfEffect skill;
        [Tooltip("附加    ")] [SerializeField] private ArgList addition;



        public double Defense { get => defense; set => defense = value; }
        public double CritRate { get => critRate; set => critRate = value; }
        public double CritBounes { get => critBounes; set => critBounes = value; }
        public int Pop { get => pop; set => pop = value; }
        public int AttackRange { get => attackRange; set => attackRange = value; }
        public int MoveRange { get => moveRange; set => moveRange = value; }
        public Position StandPosition { get => standPosition; set => standPosition = value; }
        public Position AttackPosition { get => attackPosition; set => attackPosition = value; }
        public AttackType Attack { get => attackType; set => attackType = value; }
        public int AttackArea { get => attackArea; set => attackArea = value; }
        public int TargetCount { get => targetCount; set => targetCount = value; }
        public HalfEffect Skill => skill;
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
            targetCount = 1;

            standPosition = Position.none;
            attackPosition = Position.none;
            attackType = AttackType.melee;

            skill = new HalfEffect();
            addition = new ArgList();
        }

        public BattleProperty(ArmyItem armyItem)
        {
            defense = armyItem.Defense;
            critRate = armyItem.CritRate;
            critBounes = armyItem.CritBounes;
            attackRange = armyItem.AttackRange;
            moveRange = armyItem.MoveRange;
            pop = armyItem.Pop;
            standPosition = armyItem.StandPosition;
            attackPosition = armyItem.AttackPosition;

            attackType = armyItem.AttackType;
            attackArea = armyItem.AttackArea;
            targetCount = armyItem.TargetCount;

            skill = armyItem.SkillPack;
            addition = armyItem.Addition;
        }

        public bool Equals(BattleProperty obj)
        {
            return CritRate == obj.CritRate &&
                CritBounes == obj.CritBounes &&
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
    }
}

