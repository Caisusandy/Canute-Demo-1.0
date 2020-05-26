﻿using System;
using System.Collections.Generic;
using UnityEngine;
namespace Canute.BattleSystem
{
    /// <summary>
    /// Trigger's parameter
    /// This is only a single unit of trigger condition
    /// </summary>
    [Serializable]
    public class TriggerCondition : Args, INameable, ICloneable, IEquatable<TriggerCondition>
    {
        public enum Conditions
        {
            /// <summary> When turn begin </summary> 
            turnBegin,
            /// <summary> When turn ended </summary>
            turnEnd,
            /// <summary> When play card </summary>
            playCard,
            /// <summary> When moved </summary>
            move,
            /// <summary> attack(When attack just begin) </summary>
            beforeAttack,
            /// <summary> attack(When tried changes attack's value) </summary>
            attack,
            /// <summary> defense(When tried changes attack's value) </summary>
            defense,
            /// <summary> defense(When attack ended) </summary>
            afterDefence,
            /// <summary> when entity arrive a cell </summary>
            entityArrive,
            /// <summary> when entity leave a cell </summary>
            entityLeft,
        }

        public enum ConditionGroups
        {
            or,
            and
        }


        [SerializeField] private Conditions condition;
        [SerializeField] private bool expectValue;
        [SerializeField] private ConditionGroups group;

        public Conditions Condition { get => condition; set => condition = value; }
        public ConditionGroups Group { get => group; set => group = value; }
        public bool ExpectValue { get => expectValue; set => expectValue = value; }
        public string Name => Condition.ToString();

        public TriggerCondition(Conditions cd, bool ex, ConditionGroups lg)
        {
            condition = cd;
            expectValue = ex;
            group = lg;
        }

        public TriggerCondition(Conditions cd, bool ex, ConditionGroups lg, KeyValuePairs<string, string> param) : this(cd, ex, lg)
        {
            foreach (KeyValuePair<string, string> item in param)
            {
                Add(item.Key, item.Value);
            }
        }

        public object Clone()
        {
            return MemberwiseClone() as TriggerCondition;
        }

        public bool Equals(TriggerCondition other)
        {
            Debug.Log(other);
            if (other is null)
            {
                return false;
            }
            if (Count != other.Count)
            {
                return false;
            }
            if (condition != other.condition || group != other.group || expectValue != other.expectValue)
            {
                return false;
            }
            foreach (var item in this)
            {
                if (!other.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }
        public override string ToString()
        {
            return "Condition: " + condition.ToString() + ", Expected: " + expectValue;
        }
        public override bool Equals(object obj)
        {
            return Equals(obj as TriggerCondition);
        }
        public override int GetHashCode()
        {
            return ((int)condition) * 100 + ((int)group) * 10 + expectValue.GetHashCode();
        }
        /// <summary> When moved </summary>
        public static TriggerCondition OnMove => new TriggerCondition(Conditions.move, true, ConditionGroups.or);
        /// <summary> attack(When tried changes attack's value) </summary>
        public static TriggerCondition OnAttack => new TriggerCondition(Conditions.attack, true, ConditionGroups.or);
        /// <summary> attack(When attack just begin) </summary>  
        public static TriggerCondition OnBeforeAttack => new TriggerCondition(Conditions.beforeAttack, true, ConditionGroups.or);
        /// <summary> defense(When tried changes attack's value) </summary>
        public static TriggerCondition OnDefense => new TriggerCondition(Conditions.defense, true, ConditionGroups.or);
        /// <summary> defense(When attack ended) </summary>
        public static TriggerCondition OnDefenseEnd => new TriggerCondition(Conditions.afterDefence, true, ConditionGroups.or);
        /// <summary> When play card </summary>
        public static TriggerCondition OnPlayCard => new TriggerCondition(Conditions.playCard, true, ConditionGroups.or);
        /// <summary> when entity arrive a cell </summary>
        public static TriggerCondition OnEnterCell => new TriggerCondition(Conditions.entityArrive, true, ConditionGroups.or);
        /// <summary> when entity leave a cell </summary>
        public static TriggerCondition OnExitCell => new TriggerCondition(Conditions.entityLeft, true, ConditionGroups.or);
        /// <summary> when turn begin </summary>
        public static TriggerCondition OnTurnBegin => new TriggerCondition(Conditions.turnBegin, true, ConditionGroups.or);
        /// <summary> when turn end </summary>
        public static TriggerCondition OnTurnEnd => new TriggerCondition(Conditions.turnEnd, true, ConditionGroups.or);
        public static TriggerCondition Parse(Arg arg)
        {
            return (TriggerCondition)arg;
        }
        public static bool IsTriggerCondition(Arg arg)
        {
            TriggerCondition args = (TriggerCondition)arg;
            if (args is null)
            {
                return false;
            }
            return true;

        }
        public static bool operator !=(TriggerCondition a, TriggerCondition b)
        {
            return !(a == b);
        }
        public static bool operator ==(TriggerCondition a, TriggerCondition b)
        {
            return a?.Equals(b) is true;
        }

        public static explicit operator TriggerCondition(Arg arg)
        {
            TriggerCondition condition = null;
            try
            {
                switch (arg.Key)
                {
                    case "OnPlayCard":
                        condition = OnPlayCard;
                        break;
                    case "onMove":
                        condition = OnMove;
                        break;
                    case "onAttack":
                        condition = OnAttack;
                        break;
                    case "onBeforeAttack":
                        condition = OnBeforeAttack;
                        break;
                    case "onDefense":
                        condition = OnDefense;
                        break;
                    case "onDefenseEnd":
                        condition = OnDefenseEnd;
                        break;
                    default:
                        throw new Exception();
                }

                condition.expectValue = string.IsNullOrEmpty(arg.Value) ? true : bool.Parse(arg.Value);
                return condition;
            }
            catch
            {
                if (condition != null)
                {
                    Debug.LogWarning("Trigger Condition Converted Failed " + arg);
                }
                return null;
            }
        }
    }

    /// <summary>
    /// The Conditions for status to trigger
    /// </summary>
    [Serializable]
    public class TriggerConditions : DataList<TriggerCondition>, ICloneable, IEquatable<TriggerConditions>
    {
        public TriggerConditions() { }

        public TriggerConditions(IEnumerable<TriggerCondition> keyValuePairs) : base(keyValuePairs) { }

        public TriggerConditions(TriggerCondition keyValuePairs) : base(new List<TriggerCondition> { keyValuePairs }) { }

        public TriggerConditions(params TriggerCondition[] keyValuePairs) : this((IEnumerable<TriggerCondition>)keyValuePairs) { }

        public bool HasCondition(TriggerCondition.Conditions condition)
        {
            foreach (TriggerCondition item in this)
            {
                if (item.Condition == condition)
                {
                    return true;
                }
            }
            return false;
        }

        public bool CanTrigger(Effect currentEffect)
        {
            return ConditionChecker.IsValid(this, currentEffect);
        }

        public bool CanTrigger()
        {
            return ConditionChecker.IsValid(this);
        }

        public TriggerConditions Clone()
        {
            return new TriggerConditions(((IEnumerable<TriggerCondition>)this).Clone());
        }

        object ICloneable.Clone()
        {
            return Clone() as object;
        }

        public bool Equals(TriggerConditions other)
        {
            if (other.Count != Count)
            {
                return false;
            }

            foreach (TriggerCondition item in this)
            {
                if (!other.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TriggerConditions);
        }

        public override string ToString()
        {
            string ret = "";
            foreach (var item in this)
            {
                ret += item.ToString() + "\n";
            }
            return ret;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static TriggerConditions GetTriggerCondition(Effect effect)
        {
            List<TriggerCondition> args = new List<TriggerCondition>();

            foreach (var item in effect.Args)
            {
                if (TriggerCondition.IsTriggerCondition(item))
                {
                    args.Add(TriggerCondition.Parse(item));
                }
            }
            return new TriggerConditions(args);
        }

    }

    public static class ConditionChecker
    {
        public static bool IsValid(this TriggerConditions conditions, Effect effect = null)
        {
            bool andGroup = true;
            bool orGroup = true;
            foreach (TriggerCondition item in conditions)
            {
                bool result = item.IsValid(effect) == item.ExpectValue;

                switch (item.Group)
                {
                    case TriggerCondition.ConditionGroups.and:
                        andGroup = andGroup && (item.IsValid() == item.ExpectValue);
                        break;
                    case TriggerCondition.ConditionGroups.or:
                        orGroup = orGroup || (item.IsValid() == item.ExpectValue);
                        break;
                }
            }
            return orGroup && andGroup;
        }

        /// <summary>
        /// Check every condition pack
        /// </summary>
        /// <param name="condition"> condition pack </param>
        /// <returns></returns>
        public static bool IsValid(this TriggerCondition condition, Effect refEffect = null)
        {
            switch (condition.Condition)
            {
                //These conditions will be require a specific check
                case TriggerCondition.Conditions.turnBegin:
                    return IsTurnBegin();
                case TriggerCondition.Conditions.turnEnd:
                    return IsTurnEnd();
                case TriggerCondition.Conditions.playCard:
                    return IsPlayingCard(condition);
                case TriggerCondition.Conditions.entityArrive:
                    return IsEntityArriving(refEffect);
                case TriggerCondition.Conditions.entityLeft:
                    return IsEntityLeaving(refEffect);
                //These conditions will be automatically recognize when the time is right
                case TriggerCondition.Conditions.beforeAttack:
                case TriggerCondition.Conditions.attack:
                case TriggerCondition.Conditions.defense:
                case TriggerCondition.Conditions.afterDefence:
                    return IsAttacking(refEffect);
                case TriggerCondition.Conditions.move:
                    return IsMoving(refEffect);
                default:
                    break;
            }

            return false;
        }

        private static bool IsMoving(Effect refEffect)
        {
            return refEffect?.Type == Effect.Types.move;
        }

        private static bool IsAttacking(Effect refEffect)
        {
            return refEffect?.Type == Effect.Types.attack;
        }

        private static bool IsEntityLeaving(Effect refEffect)
        {
            if (refEffect?.Type == Effect.Types.move)
            {
                ArmyEntity hasArmyStandOn = (refEffect.Target as OnMapEntity)?.OnCellOf.HasArmyStandOn;
                return !hasArmyStandOn;
            }
            return false;
        }

        private static bool IsEntityArriving(Effect refEffect)
        {
            if (refEffect?.Type == Effect.Types.move)
            {
                ArmyEntity hasArmyStandOn = (refEffect.Target as OnMapEntity)?.OnCellOf.HasArmyStandOn;
                return hasArmyStandOn;
            }
            return false;
        }

        private static bool IsPlayingCard(TriggerCondition condition)
        {
            string sCardType = condition.TryGet("cardType");
            string sType = condition.TryGet("effect");
            Card.Types cardType = default;
            Effect.Types effectType = default;
            if (!(sCardType is null))
                cardType = (Card.Types)Enum.Parse(typeof(Card.Types), sCardType);
            if (!(sType is null))
                effectType = (Effect.Types)Enum.Parse(typeof(Effect.Types), sType);

            if (!(sCardType is null) && !(sType is null))
                return Card.LastCard.Type == cardType && Card.LastCard.Effect.Type == effectType;
            else if (!(sCardType is null))
                return Card.LastCard.Type == cardType;
            else if (!(sType is null))
                return Card.LastCard.Effect.Type == effectType;
            else
                return Card.LastCard != null;
        }

        private static bool IsTurnEnd()
        {
            return Game.CurrentBattle.Round.CurrentStat == Round.Stat.turnEnd;
        }

        private static bool IsTurnBegin()
        {
            return Game.CurrentBattle.Round.CurrentStat == Round.Stat.turnBegin;
        }
    }
}