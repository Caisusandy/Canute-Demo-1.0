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

        public string Name => GetName();
        public Types Type { get => type; set => type = value; }
        public int Count { get => count; set => count = value; }
        public int Parameter { get => parameter; set => parameter = value; }
        public Args Args { get => args; set => args = value; }

        public Entity Source { get => Entity.Get(sourceEntity); set => sourceEntity = value?.UUID == null ? UUID.Empty : value.UUID; }
        public Entity Target { get => GetUniTarget(); set => SetTarget(value); }
        public List<Entity> Targets { get => Entity.Get(targetEntities); set => SetTargets(value); }


        public string this[string index]
        {
            get { try { return args[index]; } catch { return null; } }
            set { SetParam(index, value); }
        }


        public IEnumerable<Entity> AllEntities => new List<Entity>(Targets).Union(new List<Entity> { Source });
        public bool IsCompleteEffect => type != Types.none && targetEntities != null && sourceEntity != UUID.Empty && count != 0;
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
        public Effect(PropertyType i, BounesType bt, int c, int p = 0) : this()
        {
            type = Types.propertyBounes;
            count = c;
            parameter = p;
            this[propertyBounesType] = i.ToString();
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
        public Effect(Types i, Entity s, List<Entity> te, int c, int p = 0) : this(i, c, p)
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


        private string GetName()
        {
            if (string.IsNullOrEmpty(specialName))
            {
                if (type == Types.@event || type == Types.effectRelated || type == Types.tag)
                {
                    return this[name];
                }
                else if (type == Types.propertyBounes || type == Types.propertyPanalty)
                {
                    return this[propertyBounesType];
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

        public void SetUniTarget(IUUIDLabeled value)
        {
            Debug.Log("set unitarget");
            targetEntities = new List<UUID> { value.UUID };
        }

        public void SetTarget(params IUUIDLabeled[] value)
        {
            targetEntities = new List<UUID>();
            foreach (var item in value)
            {
                targetEntities.Add(item.UUID);
            }
        }

        public void SetSource(IUUIDLabeled value)
        {
            sourceEntity = value.UUID;
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

        public void SetParams(params Arg[] args)
        {
            foreach (var item in args)
            {
                SetParam(item.Key, item.Value);
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
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return default;
            }
        }

        public bool HasParam(string key)
        {
            return string.IsNullOrEmpty(this[key]);
        }

        public bool HasParam(string key, string value)
        {
            return this[key] == value;
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
            Effect e = Clone();
            e[isStatus] = "true";
            e.type = e.GetParam<Types>("effectType");

            int sc = GetParam("sc");
            int tc = GetParam("tc");
            TriggerConditions cd = TriggerConditions.GetTriggerCondition(this);
            Status.StatType st = GetParam<Status.StatType>(statType);

            Status status = new Status(e, tc, sc, st, cd);
            Debug.Log(status);
            return status;
        }

        public Status ToStatus(params TriggerCondition[] args)
        {
            Effect e = Clone();
            e[isStatus] = "true";
            e.type = e.GetParam<Types>("effectType");

            int sc = GetParam("sc");
            int tc = GetParam("tc");
            TriggerConditions cd = TriggerConditions.GetTriggerCondition(this); cd.AddRange(args);
            Status.StatType st = GetParam<Status.StatType>(statType);

            Status status = new Status(e, tc, sc, st, cd);
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

            return "=======Effect=======\n"
                + "Type: " + type.ToString() + "\n"
                + "Count: " + count + "\n"
                + "Parameter: " + parameter + "\n"
                + "Source name: " + Source?.Name + ", " + sourceEntity + "\n"
                + "Target names: " + GetTargetName() + "\n"
                + (args is null ? "" : "Args: " + args.ToString());
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }

    public interface IEffect : INameable
    {
        Effect.Types Type { get; set; }
        int Count { get; set; }
        int Parameter { get; set; }
        Args Args { get; set; }
    }

    [Serializable]
    public struct HalfEffect : IEffect
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


}