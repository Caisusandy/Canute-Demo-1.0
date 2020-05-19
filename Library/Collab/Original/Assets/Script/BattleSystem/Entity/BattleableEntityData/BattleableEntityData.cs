using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public abstract class BattleableEntityData : OnMapEntityData, IBattleableEntityData, ICareerLabled
    {
        [Header("Battle Entity Properties")]
        [SerializeField] protected ArmyProperty properties;
        [SerializeField] protected BattleLeader localLeader;
        [SerializeField] protected Effect skillPack;
        [SerializeField] protected Career career;
        [SerializeField] protected int anger;

        public Career Career => HasLocalCommander ? (LocalLeader.Career != Career.none ? LocalLeader.Career : career) : career;
        public virtual ArmyProperty RawProperties => properties;
        public virtual ArmyProperty Properties => EffectExecute.GetArmyProperty(this);
        public virtual ArmyProperty.Position StandPosition => Properties.StandPosition;


        public virtual BattleLeader LocalLeader { get => localLeader; set => localLeader = value; }
        public virtual BattleLeader ViceCommander => Owner?.ViceCommander;
        public virtual Effect SkillPack { get => skillPack.Clone(); set => skillPack = value; }
        public virtual double Morale => Owner.Morale;
        public virtual int Anger { get => anger; set => anger = value < 0 ? 0 : value >= 100 ? 100 : value; }
        public virtual bool HasViceCommander => !IsNullOrEmpty(ViceCommander);
        public virtual bool HasLocalCommander => !IsNullOrEmpty(localLeader);


        protected BattleableEntityData() : base() { }

        protected BattleableEntityData(Prototype prototype) : base(prototype) { }

        public virtual void CheckPotentialAction(params object[] vs)
        {
            if (Anger == 100)
            {
                Debug.Log("Skill");
                PerformSkill();
            }
        }

        public virtual void PerformSkill()
        {
            Effect effect = SkillPack;
            Entity entity = Entity;

            effect.Type = Effect.Types.skill;

            IBattleableEntity battleable = entity as IBattleableEntity;

            if (battleable is null)
            {
                Debug.LogError("an entity with an imposible identity tried to perform skill " + ToString());
                return;
            }

            effect.Source = entity;
            effect.Target = entity;

            effect.Execute();
            Anger = 0;
        }

        public List<CellEntity> GetMoveArea() => MapEntity.CurrentMap.GetMoveArea(MapEntity.CurrentMap[Position], Properties.MoveRange, this);

        public List<CellEntity> GetMoveRange() => GetMoveArea().Except(MapEntity.CurrentMap.GetMoveArea(MapEntity.CurrentMap[Position], Properties.MoveRange - 1, this)).ToList();

        protected virtual void AddBounes(params IBattleBounesItem[] bouneses)
        {
            if (bouneses is null)
            {
                return;
            }
            foreach (var item in bouneses)
            {
                foreach (var property in item.Bounes)
                {
                    var checkTypeValues = Enum.GetValues(typeof(PropertyType));
                    foreach (PropertyType type in checkTypeValues)
                    {
                        switch (property.Type & type)
                        {
                            case PropertyType.moveRange:
                                properties.MoveRange = property.Bounes(properties.MoveRange, item.Level); ;
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }


        //private void LeaderBounes(BattleLeader battleLeader)
        //{
        //    health = maxHealth.Bounes(battleLeader.HealthBounesRate);
        //    defence = defence.Bounes(battleLeader.DefenceBounesRate);
        //    damage = damage.Bounes(battleLeader.DamageBounesRate);
        //}


    }

    /// <summary> 至少能够上战场的接口 </summary>
    public interface IBattleableEntityData : IOnMapEntityData, ICareerLabled
    {
        int Anger { get; set; }
        bool HasViceCommander { get; }
        BattleLeader ViceCommander { get; }
        Effect SkillPack { get; set; }
        ArmyProperty Properties { get; }
        ArmyProperty RawProperties { get; }
        ArmyProperty.Position StandPosition { get; }

        List<CellEntity> GetMoveArea();
        List<CellEntity> GetMoveRange();
    }

}