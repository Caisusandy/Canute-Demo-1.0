using Canute.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{
    /// <summary>
    /// 效果
    /// </summary>
    [Serializable]
    public partial class Effect : IUUIDLabeled, INameable, ISimilar<Effect>, ICloneable
    {
        [SerializeField] private UUID uuid;
        [SerializeField] private string specialName;

        [SerializeField] private Types type;
        [SerializeField] private int count;
        [SerializeField] private int parameter;
        [SerializeField] private UUID sourceEntity;
        [SerializeField] private List<UUID> targetEntities = new List<UUID>();

        [SerializeField] private Args args = new Args();

        public UUID UUID { get => uuid; set => uuid = value; }
        public string Name => string.IsNullOrEmpty(specialName) ? type.ToString() : specialName;

        public Entity Source { get => Entity.Get(sourceEntity); set => sourceEntity = value?.UUID == null ? UUID.Empty : value.UUID; }
        public Entity Target { get => GetUniTarget(); set => SetUniTarget(value); }
        public List<Entity> Targets { get => Entity.Get(targetEntities); set => SetTargets(value); }

        public Types Type { get => type; set => type = value; }
        public int Count { get => count; set => count = value; }
        public int Parameter { get => parameter; set => parameter = value; }
        public string this[string index]
        {
            get { try { return args[index]; } catch { return null; } }
            set { SetParam(index, value); }
        }


        public IEnumerable<Entity> AllEntities => new List<Entity>(Targets).Union(new List<Entity> { Source });
        public bool IsCompleteEffect => type != Types.none && targetEntities != null && sourceEntity != UUID.Empty && count != 0;
        public Sprite Icon => GameData.SpriteLoader.GetIcon(name: Name);
        public Args Args { get => args; set => args = value; }


        public Effect()
        {
            this.NewUUID();
            args = new Args();
        }

        public Effect(Types i, int c, int p = 0) : this()
        {
            type = i;
            count = c;
            parameter = p;
        }
        public Effect(PropertyType i, BounesType bt, int c, int p = 0) : this()
        {
            type = Types.propertyBounes;
            count = c;
            parameter = p;
            this[propertyBounes] = i.ToString();
            this[bounesType] = bt.ToString();
        }

        public Effect(Types i, int c, int p = 0, params Arg[] args) : this(i, c, p)
        {
            this.args = new Args(args);
        }

        public Effect(PropertyType i, BounesType bt, int c, int p = 0, params Arg[] args) : this(i, bt, c, p)
        {
            this.args.AddRange(args);
        }

        public Effect(Types i, Entity s, List<Entity> te, int c, int p) : this(i, c, p)
        {
            Source = s;
            Targets = te;
        }

        public Effect(Types i, Entity s, Entity te, int c, int p = 0) : this(i, c, p)
        {
            Source = s;
            Target = te;
        }

        public Effect(Types i, Entity s, Entity te, int c, int p, params Arg[] vs) : this(i, s, te, c, p)
        {
            args = new Args();
            foreach (var item in vs)
            {
                args.Add(item);
            }
        }

        public Effect(Types i, Entity s, List<Entity> te, int c, int p, params Arg[] vs) : this(i, s, te, c, p)
        {
            args = new Args(vs);
        }

        public Effect(Types i, Entity s, List<Entity> te, int c, int p, Args vs) : this(i, s, te, c, p)
        {
            args = new Args(vs);
        }



        private void SetTargets(List<Entity> value)
        {
            targetEntities = new List<UUID>();
            if (value is null)
            {
                return;
            }
            foreach (Entity item in value)
            {
                targetEntities.Add(item.UUID);
            }
        }

        private void SetUniTarget(Entity value)
        {
            if (value is null)
            {
                return;
            }

            Debug.Log("set unitarget");
            targetEntities = new List<UUID> { value.UUID };
        }

        private Entity GetUniTarget()
        {
            if (targetEntities is null)
            {
                return null;
            }
            else if (targetEntities.Count == 1)
            {
                return Entity.Get(targetEntities[0]);
            }
            return null;
        }


        public void RemoveTarget(Entity entity)
        {
            targetEntities.Remove(entity.UUID);
        }

        public void SetSpecialName(string name)
        {
            specialName = name;
        }

        public void SetParam(string key, string value)
        {
            if (args.ContainsKey(key))
            {
                if (value is null)
                {
                    args.Remove(key);
                    return;
                }
                else
                {
                    args[key] = value;
                }
            }
            else
            {
                args.Add(key, value);
            }
        }

        public int GetParam(string key)
        {
            return int.Parse(string.IsNullOrEmpty(this[key]) ? "0" : this[key]);
        }

        public T GetParam<T>(string key) where T : Enum
        {
            try
            {
                return (T)Enum.Parse(typeof(T), this[key]);
            }
            catch
            {

                try
                {
                    return (T)(object)int.Parse(this[key]);
                }
                catch (Exception e)
                {
                    throw e;
                }
            }
        }

        public CellEntity GetCellParam()
        {
            return Game.CurrentBattle.MapEntity.GetCell(GetParam("x"), GetParam("y"));
        }

        public void SetCellParam(CellEntity cellEntity)
        {
            this["x"] = cellEntity.x.ToString();
            this["y"] = cellEntity.y.ToString();
            Debug.Log(GetCellParam());
        }

        /// <summary>
        /// Effect 的相似条件：
        /// 1. 同一个 type
        /// 2. parameter 一致
        /// 3. args 一致
        /// 4. targets 一致
        /// 5. source 一致
        /// </summary>
        /// <param name="effect"></param>
        /// <returns></returns>
        public bool SimilarTo(Effect effect)
        {
            if (targetEntities.Count != effect.targetEntities.Count)
            {
                return false;
            }
            foreach (var item in targetEntities)
            {
                if (!effect.targetEntities.Contains(item))
                {
                    return false;
                }
            }
            foreach (var item in args)
            {
                if (!effect.args.Contains(item))
                {
                    return false;
                }
            }

            return type == effect.type && parameter == effect.parameter && sourceEntity == effect.sourceEntity;
        }

        public Status ToStatus()
        {
            Effect e = new Effect(type, Source.Owner.Entity, Targets, Count, Parameter, Args.ToArray());
            e.SetSpecialName(specialName);
            Debug.Log(e.specialName);
            e[isStatus] = "true";
            e[statusAddingEffect] = null;
            TriggerConditions cd = TriggerCondition.GetTriggerCondition(this);
            int sc = GetParam("sc");
            int tc = GetParam("tc");
            return new Status(e, tc, sc, cd);
        }

        public Status ToStatus(params TriggerCondition[] args)
        {
            Effect e = new Effect(type, Source.Owner.Entity, Targets, Count, Parameter, Args.ToArray());
            e.specialName = specialName;
            e[isStatus] = "true";
            e[statusAddingEffect] = null;
            TriggerConditions cd = TriggerCondition.GetTriggerCondition(this);
            cd.AddRange(args);
            int sc = GetParam("sc");
            int tc = GetParam("tc");
            return new Status(e, tc, sc, cd);
        }

        public Effect Clone()
        {
            Effect effect = new Effect(type, Source, Targets, count, parameter, args);
            effect.SetSpecialName(specialName);
            return effect;
        }

        public override string ToString()
        {
            string GetTargetName()
            {
                string ans = "";
                if (Targets is null)
                {
                    return ans;
                }
                if (Targets.Count == 0)
                {
                    return ans;
                }
                foreach (Entity item in Targets)
                {
                    ans += "\n  -";
                    ans += item?.Name;
                    ans += ",";
                    ans += item?.UUID;
                    ans += ";\n";
                }
                return ans;
            }

            string GetArgs()
            {
                string ans = "";
                if (args is null)
                {
                    return ans;
                }
                if (args.Count == 0)
                {
                    return ans;
                }
                foreach (var item in args)
                {
                    ans += "\n  -";
                    ans += item.Key;
                    ans += ": ";
                    ans += item.Value;
                    ans += ";";
                }
                return ans;
            }
            return "Type:" + type.ToString() + "\n"
                + "Count:" + count + "\n"
                + "Parameter:" + parameter + "\n"
                + "Source name:" + Source?.name + ", " + sourceEntity + "\n"
                + "Target names:" + GetTargetName() + "\n"
                + "Args:" + GetArgs();
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }


    /// <summary> 状态 </summary>
    [Serializable]
    public class Status : IUUIDLabeled, INameable, ISimilar<Status>, ICloneable
    {
        /*
         * 对于所有的stat：
         * 1.应该是瞬时的效果
         */

        [SerializeField] private Effect effect;
        [SerializeField] private TriggerConditions triggerConditions;
        [SerializeField] private int turnCount;
        [SerializeField] private int statCount;


        public Effect Effect { get => effect; set => effect = value; }
        public TriggerConditions TriggerConditions { get => triggerConditions; set => triggerConditions = value; }
        public int TurnCount { get => turnCount; set => turnCount = value; }
        public int StatCount { get => statCount; set => statCount = value; }

        public bool IsResonance => this is Resonance;
        public bool IsTag => triggerConditions is null ? true : triggerConditions.Count == 0;
        public bool IsPermanentStatus => turnCount == -1 && statCount == -1;
        public bool IsBaseOnTurn => TurnCount > 0 && statCount == 0;
        public bool IsBaseOnCount => statCount > 0 && turnCount == 0;
        public bool IsDualBase => statCount > 0 && turnCount > 0;
        [Temporary] public bool IsValid => !NoMore || (effect is null);
        public bool NoMore => (turnCount == 0 && statCount == 0);
        public UUID UUID { get => Effect.UUID; set => Effect.UUID = value; }
        public Effect.Types Type { get => Effect.Type; set => Effect.Type = value; }
        public string Name => Effect.Name;

        public bool SimilarTo(Status other)
        {
            bool sameCount = ((IsBaseOnCount == other.IsBaseOnCount == true) || (IsBaseOnTurn == other.IsBaseOnTurn == true) || (IsDualBase == other.IsDualBase == true)) && !IsPermanentStatus && !other.IsPermanentStatus;
            return sameCount && effect.SimilarTo(other.effect) && triggerConditions.Equals(other.TriggerConditions);
        }

        public Status(Effect e)
        {
            effect = e;
            effect[Effect.isStatus] = "true";
        }

        public Status(Effect e, int tc, int sc) : this(e)
        {
            turnCount = tc;
            statCount = sc;
        }

        public Status(Effect e, int tc, int sc, TriggerConditions tr) : this(e, tc, sc)
        {
            triggerConditions = tr;
        }

        public Status(Effect e, int tc, int sc, TriggerCondition tr) : this(e, tc, sc)
        {
            triggerConditions = new TriggerConditions(tr);
        }

        public Status(Effect e, int tc, int sc, params TriggerCondition[] tr) : this(e, tc, sc)
        {
            triggerConditions = new TriggerConditions(tr);
        }


        public static implicit operator Effect(Status status)
        {
            return status?.Effect;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns>Whether merge is successful or not</returns>
        public bool Merge(Status other)
        {
            if (IsBaseOnTurn && other.IsBaseOnTurn)
            {
                turnCount += other.turnCount;
                this.StatusMerge(other);
                return true;
            }
            if (IsBaseOnCount && other.IsBaseOnCount)
            {
                statCount += other.statCount;
                this.StatusMerge(other);
                return true;
            }
            if (IsDualBase && other.IsDualBase)
            {
                statCount += other.statCount;
                turnCount += other.turnCount;
                this.StatusMerge(other);
                return true;
            }


            return false;
        }

        public object Clone()
        {
            return new Status(effect.Clone(), turnCount, statCount, triggerConditions.Clone() as TriggerConditions);
        }

        public override string ToString()
        {
            return "Type: " + effect?.Type.ToString() +
                "\nStatus Count: " + statCount +
                "\nTurn Count: " + turnCount +
                "\n============================\n" + TriggerConditions?.ToString() +
                "============================\nEffect: \n" + effect?.ToString();
            ;

        }
    }
}