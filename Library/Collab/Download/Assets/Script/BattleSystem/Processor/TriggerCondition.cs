using System;
using System.Collections.Generic;
using UnityEngine;
namespace Canute.BattleSystem
{
    /// <summary>
    /// Trigger's parameter
    /// </summary>
    [Serializable]
    public class TriggerCondition : Args, INameable, ICloneable, IEquatable<TriggerCondition>
    {
        public enum Conditions
        {
            turnBegin,
            turnEnd,
            move,
            beforeAttack,
            attack,
            defense,
            afterDefence
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

        public bool HasKey(string key)
        {
            try
            {
                string value = this[key];
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public object Clone()
        {
            return MemberwiseClone() as TriggerCondition;
        }

        public override string ToString()
        {
            return "Condition: " + condition.ToString() + ", Expected: " + expectValue;
        }

        public bool Equals(TriggerCondition other)
        {
            Debug.Log(other);
            if (other is null)
            {
                return false;
            }
            if (Count == other.Count)
            {
                return false;
            }
            if (condition != other.condition || group != other.group || expectValue != other.expectValue)
            {
                return false;
            }
            foreach (KeyValuePair<string, string> item in this)
            {
                if (!other.HasKey(item.Key))
                {
                    return false;
                }
                else if (item.Value != other[item.Key])
                {
                    return false;
                }
            }
            return true;
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as TriggerCondition);
        }

        public static TriggerCondition OnMove => new TriggerCondition(Conditions.move, true, ConditionGroups.or);
        public static TriggerCondition OnAttack => new TriggerCondition(Conditions.attack, true, ConditionGroups.or);
        public static TriggerCondition OnBeforeAttack => new TriggerCondition(Conditions.beforeAttack, true, ConditionGroups.or);
        public static TriggerCondition OnDefense => new TriggerCondition(Conditions.defense, true, ConditionGroups.or);
        public static TriggerCondition OnDefenseEnd => new TriggerCondition(Conditions.afterDefence, true, ConditionGroups.or);

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static TriggerCondition Parse(Arg arg)
        {
            return (TriggerCondition)arg;
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

        public static bool IsTriggerCondition(Arg arg)
        {
            TriggerCondition args = (TriggerCondition)arg;
            if (args is null)
            {
                return false;
            }
            return true;

        }

        public static TriggerConditions GetTriggerCondition(Effect effect)
        {
            List<TriggerCondition> args = new List<TriggerCondition>();

            foreach (var item in effect.Args)
            {
                if (IsTriggerCondition(item))
                {
                    args.Add(Parse(item));
                }
            }
            return new TriggerConditions(args);
        }
    }

    [Serializable]
    public class TriggerConditions : DataList<TriggerCondition>, ICloneable, IEquatable<TriggerConditions>
    {
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
            return ConditionChecker.Check(this, currentEffect);
        }

        public bool CanTrigger()
        {
            return ConditionChecker.Check(this);
        }

        public object Clone()
        {
            return new TriggerConditions(((IEnumerable<TriggerCondition>)this).Clone());
        }

        public TriggerConditions() { }

        public TriggerConditions(IEnumerable<TriggerCondition> keyValuePairs) : base(keyValuePairs) { }

        public TriggerConditions(TriggerCondition keyValuePairs) : base(new List<TriggerCondition> { keyValuePairs }) { }

        public TriggerConditions(params TriggerCondition[] keyValuePairs) : this((IEnumerable<TriggerCondition>)keyValuePairs) { }

        public override string ToString()
        {
            string ret = "";
            foreach (var item in this)
            {
                ret += item.ToString() + "\n";
            }
            return ret;
        }
    }

    public static class ConditionChecker
    {

        public static bool Check(this TriggerConditions conditions, Effect effect = null)
        {
            bool andGroup = true;
            bool orGroup = true;
            foreach (TriggerCondition item in conditions)
            {
                bool result = item.Check(effect) == item.ExpectValue;

                switch (item.Group)
                {
                    case TriggerCondition.ConditionGroups.and:
                        andGroup = andGroup && (item.Check() == item.ExpectValue);
                        break;
                    case TriggerCondition.ConditionGroups.or:
                        orGroup = orGroup || (item.Check() == item.ExpectValue);
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
        public static bool Check(this TriggerCondition condition, Effect effect = null)
        {
            switch (condition.Condition)
            {
                //These conditions will be require a specific check
                case TriggerCondition.Conditions.turnBegin:
                    return Game.CurrentBattle.Round.CurrentStat == Round.Stat.turnBegin;
                case TriggerCondition.Conditions.turnEnd:
                    return Game.CurrentBattle.Round.CurrentStat == Round.Stat.turnEnd;

                //These conditions will be automatically recognize when the time is right
                case TriggerCondition.Conditions.beforeAttack:
                case TriggerCondition.Conditions.attack:
                case TriggerCondition.Conditions.defense:
                case TriggerCondition.Conditions.afterDefence:
                    return effect?.Type == Effect.Types.attack;
                case TriggerCondition.Conditions.move:
                    return effect?.Type == Effect.Types.move;
                default:
                    break;
            }

            return false;
        }


    }
}