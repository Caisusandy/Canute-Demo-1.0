using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{

    public abstract class BuildingEntity : OnMapEntity, IBattleableEntity, IPassiveEntity
    {
        public BattleBuilding data;
        public override EntityData Data => data;

        IPassiveEntityData IPassiveEntity.Data => data;
        IBattleableEntityData IBattleableEntity.Data => data;
        public override BattleProperty.Position StandPostion => data.StandPosition;


        public abstract float AttackAtionDuration { get; }
        public abstract float SkillDuration { get; }
        public abstract float DefeatedDuration { get; }
        public abstract float WinningDuration { get; }
        public abstract float HurtDuration { get; }


        public static List<BuildingEntity> onMap = new List<BuildingEntity>();

        public virtual void Defeated(params object[] vs)
        {
            PerformingAnimation();
            animator.SetBool(isDefeated, true);
            Action(new EntityEventPack(IdleDelay, DefeatedDuration), new EntityEventPack(Remove), new EntityEventPack(data.CheckPotentialAction));
        }
        public virtual void Skill(params object[] vs)
        {
            PerformingAnimation();
            animator.SetBool(isPerformingSkill, true);
            SkillAction();
        }
        public virtual void Winning(params object[] vs)
        {
            PerformingAnimation();
            animator.SetBool(isWinning, true);
            Action(new EntityEventPack(IdleDelay, WinningDuration));
        }
        public virtual void ReadyToDie(params object[] vs)
        {
            IEnumerator Check(params object[] vs1)
            {
                while (true)
                {
                    if (IsIdle)
                    {
                        yield return new EntityEventPack(Defeated).Execute();
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
            Action(Check);
        }
        public virtual void Remove(params object[] vs)
        {
            Destroy(gameObject);
        }
        public virtual void Hurt(params object[] vs)
        {
            int damage = (int)vs[0];
            this.Damage(damage);

            PerformingAnimation();
            animator.SetBool(isDefencing, true);

            Action(new EntityEventPack(IdleDelay, HurtDuration), new EntityEventPack(data.CheckPotentialAction));
            Debug.Log(Data.ToString() + " Hurt");
        }


        protected virtual void SkillAction()
        {
            Debug.Log("Performing skill");
            Action(new EntityEventPack(IdleDelay, SkillDuration));
        }
        public abstract void SkillExecute(Effect effect);


        public static BuildingEntity Create(BattleBuilding item)
        {
            GameObject prefab;
            GameObject gameObject;
            BuildingEntity buildingEntity;

            prefab = item.Prefab;
            gameObject = Instantiate(prefab, Game.CurrentBattle.MapEntity[item.Position].transform);

            buildingEntity = gameObject.GetComponent<BuildingEntity>();
            buildingEntity.data = item;

            return buildingEntity;
        }

    }

    [Serializable]
    public class BattleBuilding : PassiveEntityData
    {
        public override GameObject Prefab { get => prefab ?? GameData.Prefabs.DefaultBuilding; set => prefab = value; }
        public override Prototype Prototype { get => GameData.Prototypes.GetBuildingPrototype(name); set => base.Prototype = value; }
        public new BuildingEntity Entity => base.Entity as BuildingEntity;
    }
}
