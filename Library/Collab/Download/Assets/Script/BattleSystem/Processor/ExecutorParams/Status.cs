using System;
using UnityEngine;

namespace Canute.BattleSystem
{
    /// <summary> 状态 </summary>
    [Serializable]
    public class Status : IUUIDLabeled, INameable, ISimilar<Status>, ICloneable
    {
        public enum StatType
        {
            turnBase,
            countBase,
            dualBase,
            perminant,
            resonance,
        }

        /*
         * 对于所有的stat：
         * 1. 应该是瞬时的效果
         * 2. target 的Statlist应该有这个status
         */

        [SerializeField] private Effect effect;
        [SerializeField] private TriggerConditions triggerConditions;
        [SerializeField] private int turnCount;
        [SerializeField] private int statCount;
        [SerializeField] private StatType type;
        [SerializeField] private bool showToPlayer;

        /// <summary> effect in the status </summary>
        public Effect Effect { get => effect; set => effect = value; }
        /// <summary> Trigger Conditions of the status </summary>
        public TriggerConditions TriggerConditions { get => triggerConditions; set => triggerConditions = value; }
        /// <summary> turn count of the status </summary>
        public int TurnCount { get => turnCount; set => turnCount = value; }
        /// <summary> count of the status </summary>
        public int StatCount { get => statCount; set => statCount = value; }
        /// <summary> status count type of the status </summary>
        public StatType Type { get => type; set => type = value; }
        /// <summary> show status to player in the status </summary>
        public bool ShowToPlayer { get => showToPlayer; set => showToPlayer = value; }


        /// <summary> is status a resonance </summary>
        public bool IsResonance => type == StatType.resonance;
        /// <summary> is status permanent </summary>
        public bool IsPermanentStatus => type == StatType.perminant;
        /// <summary> is status base on turn </summary>
        public bool IsBaseOnTurn => type == StatType.turnBase;
        /// <summary> is status base on count </summary>
        public bool IsBaseOnCount => type == StatType.countBase;
        /// <summary> is status base on turn & count </summary>
        public bool IsDualBase => type == StatType.dualBase;
        /// <summary> is status run out </summary>
        public bool NoMore => !HasMore();
        public UUID UUID { get => Effect.UUID; set => Effect.UUID = value; }
        /// <summary>
        /// Name of the status
        /// </summary>
        public string Name => Effect.Name;

        /// <summary>
        /// Check two status similarity
        /// <para>status is similar when:</para>
        /// <para>1. statuses is base on the same count system</para>
        /// <para>2. statuses' effect is similar to each other</para>
        /// <para>3. statuses' have same trigger condition</para>
        /// </summary>
        /// <param name="other">other status</param>
        /// <returns></returns>
        public bool SimilarTo(Status other)
        {
            bool sameCount = ((IsBaseOnCount == other.IsBaseOnCount == true) || (IsBaseOnTurn == other.IsBaseOnTurn == true) || (IsDualBase == other.IsDualBase == true) || (IsResonance == other.IsResonance == true)) && !IsPermanentStatus && !other.IsPermanentStatus;
            return sameCount && effect.SimilarTo(other.effect) && triggerConditions.Equals(other.TriggerConditions);
        }

        public Status(Effect e, bool showToPlayer = true)
        {
            this.effect = e;
            this.effect[Effect.isStatus] = "true";
            this.showToPlayer = showToPlayer;
        }

        public Status(Effect e, int tc, int sc, StatType st, bool showToPlayer = true) : this(e, showToPlayer)
        {
            turnCount = tc;
            statCount = sc;
            type = st;
        }

        public Status(Effect e, int tc, int sc, StatType st, TriggerConditions tr, bool showToPlayer = true) : this(e, tc, sc, st, showToPlayer)
        {
            triggerConditions = tr;
        }

        public Status(Effect e, int tc, int sc, StatType st, TriggerCondition tr, bool showToPlayer = true) : this(e, tc, sc, st, showToPlayer)
        {
            triggerConditions = new TriggerConditions(tr);
        }

        public Status(Effect e, int tc, int sc, StatType st, bool showToPlayer = true, params TriggerCondition[] tr) : this(e, tc, sc, st, showToPlayer)
        {
            triggerConditions = new TriggerConditions(tr);
        }


        public static implicit operator Effect(Status status)
        {
            return status?.Effect;
        }

        private bool HasMore()
        {
            switch (type)
            {
                case StatType.turnBase:
                    return TurnCount > 0;
                case StatType.countBase:
                    return StatCount > 0;
                case StatType.dualBase:
                    return StatCount > 0 && TurnCount > 0;
                case StatType.perminant:
                case StatType.resonance:
                    return true;
                default: return false;
            }
        }

        /// <summary>
        /// Merge two status together
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Whether merge is successful or not</returns>
        public bool Merge(Status other)
        {
            if (Type != other.Type)
            {
                return false;
            }
            if (IsBaseOnTurn)
            {
                TurnCount += other.TurnCount;
                this.StatusMerge(other);
                return true;
            }
            if (IsBaseOnCount)
            {
                StatCount += other.StatCount;
                this.StatusMerge(other);
                return true;
            }
            if (IsDualBase)
            {
                StatCount += other.StatCount;
                TurnCount += other.TurnCount;
                this.StatusMerge(other);
                return true;
            }
            if (IsResonance)
            {
                this.StatusMerge(other);
                return true;
            }


            return false;
        }

        public Status Clone()
        {
            return new Status(Effect.Clone(), TurnCount, StatCount, Type, TriggerConditions.Clone() as TriggerConditions);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public override string ToString()
        {
            return "Type: " + Effect?.Type.ToString() +
                "\nStatus Count: " + StatCount +
                "\nTurn Count: " + TurnCount +
                "\nType: " + type +
                "\n============================\n" +
                TriggerConditions?.ToString() +
                "============================\n" +
                Effect?.ToString();
        }

    }

    [Serializable]
    public struct HalfStatus
    {
        [SerializeField] private HalfEffect effect;
        [SerializeField] private int turnCount;
        [SerializeField] private int statCount;
        [SerializeField] private Status.StatType statType;
        [SerializeField] private TriggerConditions triggerConditions;
        [SerializeField] private bool showToPlayer;

        public HalfStatus(HalfEffect effect, int turnCount, int statCount, Status.StatType statType, TriggerConditions triggerConditions, bool showToPlayer)
        {
            this.effect = effect;
            this.turnCount = turnCount;
            this.statCount = statCount;
            this.statType = statType;
            this.triggerConditions = triggerConditions;
            this.showToPlayer = showToPlayer;
        }

        public Status ToStatus()
        {
            Status status = new Status(effect, turnCount, statCount, statType, triggerConditions);
            status.ShowToPlayer = showToPlayer;
            return status;
        }

        public static implicit operator Status(HalfStatus status)
        {
            return status.ToStatus();
        }
        public static implicit operator HalfStatus(Status status)
        {
            return new HalfStatus(status.Effect, status.TurnCount, status.StatCount, status.Type, status.TriggerConditions, status.ShowToPlayer);
        }

        public override string ToString()
        {
            return ToStatus().ToString();
        }


    }

    [Serializable]
    public struct HalfResonance
    {
        [SerializeField] private HalfEffect effect;
        [SerializeField] private TriggerConditions triggerConditions;
        [SerializeField] private bool showToPlayer;

        private HalfResonance(HalfEffect effect, TriggerConditions triggerConditions, bool showToPlayer)
        {
            this.effect = effect;
            this.triggerConditions = triggerConditions;
            this.showToPlayer = showToPlayer;
        }

        public Resonance ToResonance()
        {
            Resonance resonance = new Resonance(effect, triggerConditions);
            resonance.ShowToPlayer = showToPlayer;
            return resonance;
        }

        public static implicit operator Resonance(HalfResonance status)
        {
            return status.ToResonance();
        }

        public override string ToString()
        {
            return ToResonance().ToString();
        }
    }

    /*
     * Status Giver : a SAE status
     * StatusRemover: RemoveStatus (Effect)
     */
}