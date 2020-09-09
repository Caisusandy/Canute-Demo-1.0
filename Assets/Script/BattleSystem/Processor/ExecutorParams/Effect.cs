using Canute.LanguageSystem;
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
    public partial class Effect : IUUIDLabeled, INameable, ISimilar<Effect>, ICloneable, IEffect
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

        /// <summary> name of the effect </summary>
        public string Name => GetName();
        /// <summary> type of the effect </summary>
        public Types Type { get => type; set => type = value; }
        /// <summary> count of the effect </summary>
        public int Count { get => count; set => count = value; }
        /// <summary> parameter of the effect </summary>
        public int Parameter { get => parameter; set => parameter = value; }
        /// <summary> args of the effect </summary>
        public Args Args { get => args; set => args = value; }
        /// <summary> source of the effect (attacker)</summary>
        public Entity Source { get => GetSource(); set => SetSource(value); }
        /// <summary> target of the effect (defender)<para>When there are more than 1 target, target return </para></summary>
        public Entity Target { get => GetTarget(); set => SetTarget(value); }
        /// <summary> targets of the effect (defender)<para>always return all the targets </para> </summary>
        public List<Entity> Targets { get => Entity.Get(targetEntities); set => SetTargets(value); }

        /// <summary> Get the arg value of the effect by <paramref name="key"/></summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public string this[string key]
        {
            get { try { return args[key]; } catch { return null; } }
            set { SetParam(key, value); }
        }

        /// <summary> (Readonly) all entity involved in the effect </summary>
        public IEnumerable<Entity> AllEntities => new List<Entity>(Targets).Union(new List<Entity> { Source });
        public bool IsCompleteEffect => type != Types.none && targetEntities != null && sourceEntity != UUID.Empty && count != 0;
        /// <summary> Get Icon of the effect </summary>
        public Sprite Icon => GameData.SpriteLoader.GetIcon(name: Name);


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
        public Effect(PropertyType i, BonusType bt, int c, int p = 0) : this()
        {
            type = Types.propertyBonus;
            count = c;
            parameter = p;
            this[propertyType] = i.ToString();
            this[bonusType] = bt.ToString();
        }
        public Effect(Types i, int c, int p = 0, params Arg[] args) : this(i, c, p)
        {
            this.args = new Args(args);
        }
        public Effect(PropertyType i, BonusType bt, int c, int p = 0, params Arg[] args) : this(i, bt, c, p)
        {
            this.args.AddRange(args);
        }
        public Effect(Types i, Entity s, Entity te, int c, int p = 0, params Arg[] vs) : this(i, c, p)
        {
            Source = s;
            Target = te;
            args = new Args();
            foreach (var item in vs)
            {
                args.Add(item);
            }
        }
        public Effect(PropertyType i, BonusType bt, Entity s, Entity te, int c, int p = 0, params Arg[] args) : this(i, bt, c, p)
        {
            Source = s;
            Target = te;
            this.args.AddRange(args);
        }
        public Effect(Types i, Entity s, List<Entity> te, int c, int p, params Arg[] vs) : this(i, c, p)
        {
            Source = s;
            Targets = te;
            args = new Args(vs);
        }
        public Effect(PropertyType i, BonusType bt, Entity s, List<Entity> te, int c, int p = 0, params Arg[] args) : this(i, bt, c, p)
        {
            Source = s;
            Targets = te;
            this.args.AddRange(args);
        }


        private string GetName()
        {
            if (string.IsNullOrEmpty(specialName))
            {
                if (type == Types.@event || type == Types.effectRelated || type == Types.tag)
                {
                    return this[name];
                }
                else if (type == Types.propertyBonus || type == Types.propertyPanalty)
                {
                    return this[propertyType];
                }
                return type.ToString();
            }
            return specialName;

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

        private void SetParam(string key, string value)
        {
            if (args.ContainsKey(key))
            {
                if (value is null)
                {
                    args.Remove(key);
                    return;
                }
                else if (string.IsNullOrEmpty(value))
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

        private Entity GetSource()
        {
            return Entity.Get(sourceEntity);
        }

        private Entity GetTarget()
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

        /// <summary>
        /// set source
        /// </summary>
        /// <param name="value"></param>
        public void SetSource(IUUIDLabeled value)
        {
            sourceEntity = value?.UUID == null ? UUID.Empty : value.UUID;
        }

        /// <summary>
        /// set target of the effect
        /// </summary>
        /// <param name="value"></param>
        public void SetTarget(params IUUIDLabeled[] value)
        {
            targetEntities = new List<UUID>();
            foreach (var item in value)
            {
                targetEntities.Add(item.UUID);
            }
        }

        /// <summary>
        /// remove <paramref name="entity"/> from targets
        /// </summary>
        /// <param name="entity"></param>
        public void RemoveTarget(Entity entity)
        {
            targetEntities.Remove(entity.UUID);
        }

        /// <summary>
        /// set the special name to <paramref name="newSpecialName"/>
        /// </summary>
        /// <param name="newSpecialName">new special name</param>
        public void SetSpecialName(string newSpecialName)
        {
            specialName = newSpecialName;
        }

        /// <summary>
        /// Get the cell parameter in the effect
        /// <para>the cell parameter formed by arg {x: value} and {y: value}</para>
        /// </summary>
        /// <returns>cellEntity in the effect args</returns>
        public CellEntity GetCellParam()
        {
            return Game.CurrentBattle.MapEntity.GetCell(Args.GetIntParam("x"), Args.GetIntParam("y"));
        }

        /// <summary>
        /// Set the cell parameter
        /// </summary>
        /// <param name="cellEntity"></param>
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
                return false;
            foreach (var item in targetEntities)
            {
                if (!effect.targetEntities.Contains(item))
                {
                    return false;
                }
            }

            if (effect.args.Count != args.Count)
                return false;
            foreach (var item in args)
            {
                if (!effect.args.Contains(item))
                {
                    return false;
                }
            }

            return (type == effect.type) && (sourceEntity == effect.sourceEntity);
        }

        /// <summary>
        /// Convert the effect to a status
        /// <para>require parameter of {tc}, {sc}, {effectType}, {statType}</para>
        /// <para>use when effect is type of addStatus</para>
        /// </summary>
        /// <returns></returns>
        public Status ToStatus()
        {
            if (type != Types.addStatus)
            {
                return null;
            }

            Effect e = Clone();
            e[isStatus] = "true";
            e.SetSpecialName(e[effectSpecialName]);
            e.type = e.Args.GetEnumParam<Types>(effectType);

            int sc = Args.GetIntParam(statusCount);
            int tc = Args.GetIntParam(turnCount);
            TriggerConditions cd = TriggerConditions.GetTriggerCondition(this);
            Status.StatType st = Args.GetEnumParam<Status.StatType>(statusType);

            e[effectSpecialName] = null;
            e[effectType] = null;
            var showToPlayer = (e.Args.HasParam(statusShowToPlayer) ? e.Args.GetBoolParam(statusShowToPlayer) : true);


            Status status = new Status(e, tc, sc, st, cd, showToPlayer);
            Debug.Log(status);
            return status;
        }

        /// <summary>
        /// Convert the effect to a status with <paramref name="args"/>
        /// <para>require parameter of {tc}, {sc}, {effectType}, {statType}</para>
        /// <para>use when effect is type of addStatus</para>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Status ToStatus(params TriggerCondition[] args)
        {
            if (type != Types.addStatus)
            {
                return null;
            }

            Effect e = Clone();
            e[isStatus] = "true";
            e.SetSpecialName(e[effectSpecialName]);
            e.type = e.Args.GetEnumParam<Types>(effectType);

            int sc = Args.GetIntParam(statusCount);
            int tc = Args.GetIntParam(turnCount);
            TriggerConditions cd = TriggerConditions.GetTriggerCondition(this); cd.AddRange(args);
            Status.StatType st = Args.GetEnumParam<Status.StatType>(statusType);

            e[effectSpecialName] = null;
            e[effectType] = null;
            var showToPlayer = e.Args.HasParam(statusShowToPlayer) ? e.Args.GetBoolParam(statusShowToPlayer) : true;

            Status status = new Status(e, tc, sc, st, cd, showToPlayer);
            Debug.Log(status);
            return status;
        }

        /// <summary>
        /// Clone a Effect
        /// <para>Hint: The new Effect would have a different UUID</para>
        /// </summary>
        /// <returns>The new effect clone from the original</returns>
        public Effect Clone()
        {
            Effect effect = new Effect(type, Source, Targets, count, parameter, args.ToArray());
            effect.SetSpecialName(specialName);
            return effect;
        }

        object ICloneable.Clone()
        {
            return Clone();
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

            return "=======Effect=======\n"
                + "Type: " + type.ToString() + "\n"
                + "Count: " + count + "\n"
                + "Parameter: " + parameter + "\n"
                + "Source name: " + Source?.Name + ", " + sourceEntity + "\n"
                + "Target names: " + GetTargetName() + "\n"
                + (args is null ? "" : "Args: " + args.ToString());
        }


#if UNITY_EDITOR
        public void SetAsStatusAddingEffect()
        {
            this[turnCount] = "1";
            this[statusCount] = "1";
            this[statusType] = "turnCount";
            this[effectType] = "none";
        }
#endif
    }

    public interface IEffect : INameable
    {
        Effect.Types Type { get; set; }
        int Count { get; set; }
        int Parameter { get; set; }
        Args Args { get; set; }
    }

    [Serializable]
    public struct HalfEffect : IEffect, ICloneable
    {
        [SerializeField] private Effect.Types type;
        [SerializeField] private string specialName;
        [SerializeField] private int count;
        [SerializeField] private int parameter;
        [SerializeField] private ArgList args;

        private HalfEffect(string specialName, Effect.Types type, int count, int parameter, ArgList args)
        {
            this.specialName = specialName;
            this.type = type;
            this.count = count;
            this.parameter = parameter;
            this.args = args;
        }


        public Effect.Types Type { get => type; set => type = value; }
        public int Count { get => count; set => count = value; }
        public int Parameter { get => parameter; set => parameter = value; }
        public string Name => string.IsNullOrEmpty(specialName) ? type.ToString() : specialName;
        public Args Args { get => args; set => args = value; }

        public Effect ToEffect()
        {
            Effect effect = new Effect(Type, count, parameter, args.ToArray());
            effect.SetSpecialName(specialName);
            return effect;
        }

        public HalfEffect Clone()
        {
            return new HalfEffect(specialName, type, count, parameter, args);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public static implicit operator Effect(HalfEffect effect)
        {
            return effect.ToEffect();
        }

        public static implicit operator HalfEffect(Effect effect)
        {
            return new HalfEffect(effect.Name, effect.Type, effect.Count, effect.Parameter, effect.Args);
        }

        public override string ToString()
        {
            return "Type: " + type + "\n"
                + "Count: " + count + "\n"
                + "Parameter: " + parameter + "\n"
                + "Args: " + args.ToString();
        }

    }

    [Serializable]
    public struct HalfSkillEffect : IEffect, ICloneable
    {
        [SerializeField] private string specialName;
        [SerializeField] private int count;
        [SerializeField] private int parameter;
        [SerializeField] private ArgList args;

        private HalfSkillEffect(string specialName, int count, int parameter, ArgList args)
        {
            this.specialName = specialName;
            this.parameter = parameter;
            this.count = count;
            this.args = args;
        }


        public Effect.Types Type { get => Effect.Types.skill; set { } }
        public int Count { get => count; set => count = value; }
        public int Parameter { get => parameter; set => parameter = value; }
        public string Name => specialName;
        public Args Args { get => args; set => args = value; }

        public Effect ToEffect()
        {
            Effect effect = new Effect(Type, Count, Parameter, args.ToArray());
            effect.SetSpecialName(specialName);
            return effect;
        }

        public HalfSkillEffect Clone()
        {
            return new HalfSkillEffect(specialName, count, parameter, args);
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public static implicit operator Effect(HalfSkillEffect effect)
        {
            return effect.ToEffect();
        }


        public override string ToString()
        {
            return "Type: " + Type + "\n"
                + "Count: " + Count + "\n"
                + "Parameter: " + Parameter + "\n"
                + "Args: " + args.ToString();
        }

    }


}