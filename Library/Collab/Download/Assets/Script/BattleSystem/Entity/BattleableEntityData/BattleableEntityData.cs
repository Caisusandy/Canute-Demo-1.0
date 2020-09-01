using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{

    //[Serializable]
    //public abstract class BattleableEntityData : OnMapEntityData, IBattleableEntityData, ICareerLabled
    //{
    //    [Header("Battle Entity Properties")]
    //    [SerializeField] protected BattleProperty properties;
    //    [SerializeField] protected Career career;
    //    [SerializeField] protected int anger;

    //    [SerializeField] protected BattleLeader localLeader;
    //    [SerializeField] protected HalfSkillEffect skillPack;

    //    public Career Career => !IsNullOrEmpty(localLeader) ? (LocalLeader.Career != Career.none ? LocalLeader.Career : career) : career;
    //    public virtual BattleProperty RawProperties { get => properties; set => properties = value; }
    //    public virtual BattleProperty Properties => EffectExecute.GetProperty(this);
    //    public virtual BattleProperty.Position StandPosition => Properties.StandPosition;


    //    public virtual BattleLeader LocalLeader { get => localLeader; set => localLeader = value; }
    //    public virtual BattleLeader ViceCommander => Owner?.ViceCommander;
    //    public virtual HalfSkillEffect SkillPack { get => skillPack; set => skillPack = value; }
    //    public virtual int Anger { get => anger; set => anger = value < 0 ? 0 : value >= 100 ? 100 : value; }

    //    protected BattleableEntityData() : base() { }

    //    protected BattleableEntityData(Prototype prototype) : base(prototype) { }

    //    public virtual void CheckPotentialAction(params object[] vs)
    //    {
    //        if (Anger == 100)
    //        {
    //            Debug.Log("Skill");
    //            PerformSkill();
    //        }
    //    }

    //    public virtual void PerformSkill()
    //    {
    //        Effect effect = SkillPack;
    //        Entity entity = Entity;

    //        effect.Type = Effect.Types.skill;

    //        IBattleableEntity battleable = entity as IBattleableEntity;

    //        if (battleable is null)
    //        {
    //            Debug.LogError("an entity with an imposible identity tried to perform skill " + ToString());
    //            return;
    //        }

    //        effect.Source = entity;
    //        effect.Target = entity;

    //        effect.Execute(true);
    //        Anger = 0;
    //    }

    //    public List<CellEntity> GetMoveArea() => MapEntity.CurrentMap.GetMoveArea(MapEntity.CurrentMap[Coordinate], Properties.MoveRange, this);

    //    protected abstract void AddBounes(params IBattleBounesItem[] bouneses);

    //    protected abstract void RemoveBounes(params IBattleBounesItem[] bouneses);

    //}

    /// <summary> 至少能够上战场的接口 </summary>
    public interface IBattleableEntityData : IOnMapEntityData, ICareerLabled
    {
        int Anger { get; set; }
        BattleLeader ViceCommander { get; }
        HalfSkillEffect SkillPack { get; }
        BattleProperty Properties { get; }
        BattleProperty RawProperties { get; set; }
        BattleProperty.Position StandPosition { get; }
        /// <summary> 生命值 </summary>
        int Health { get; set; }
        /// <summary> 防御值 </summary>
        int Defense { get; }
        /// <summary> 护甲值 </summary>
        int Armor { get; set; }

        List<CellEntity> GetMoveArea();
    }


}