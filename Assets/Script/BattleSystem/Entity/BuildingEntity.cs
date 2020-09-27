using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    public abstract class BuildingEntity : OnMapEntity, IBattleableEntity
    {
        public BattleBuilding data;
        public override EntityData Data => data;

        //IPassiveEntityData IPassiveEntity.Data => data;
        IBattleableEntityData IBattleableEntity.Data => data;
        public override BattleProperty.Position StandPostion => data.StandPosition;


        public abstract float AttackAtionDuration { get; }
        public abstract float SkillDuration { get; }
        public abstract float DefeatedDuration { get; }
        public abstract float WinningDuration { get; }
        public abstract float HurtDuration { get; }


        public static List<BuildingEntity> onMap = new List<BuildingEntity>();

        public virtual void Skill(params object[] vs)
        {
            InPerformingAnimation();
            Animator.SetBool(isPerformingSkill, true);
            SkillAction();
        }
        public virtual void Winning(params object[] vs)
        {
            InPerformingAnimation();
            Animator.SetBool(isWinning, true);
            Action(new EntityEventPack(IdleDelay, WinningDuration));
        }
        public virtual void Hurt(params object[] vs)
        {
            if (!(this is IPassiveEntity)) return;

            int damage = (int)vs[0];
            var damageSource = vs[1] as IAggressiveEntity;


            (this as IPassiveEntity).Damage(damage, damageSource);

            InPerformingAnimation();
            Animator.SetBool(isDefensing, true);

            Action(new EntityEventPack(IdleDelay, HurtDuration), new EntityEventPack(data.CheckPotentialAction, damageSource));
            Debug.Log(Data.ToString() + " Hurt");
        }
        public virtual void Move(params object[] vs)
        {
            List<CellEntity> path = vs[0] as List<CellEntity>;
            Effect effect = vs[1] as Effect;

            InPerformingAnimation();
            Animator.SetBool(isMoving, true);

            EntityOnCellMotion.SetMotion(this, path, effect);
            Action(TryEndMoveAction, new EntityEventPack(data.CheckPotentialAction));

            IEnumerator TryEndMoveAction(params object[] a)
            {
                while (true)
                {
                    if (GetComponent<EntityOnCellMotion>())
                    {
                        yield return new WaitForSeconds(0.1f);
                        continue;
                    }
                    else
                    {
                        yield return Idle();
                        break;
                    }
                }
            }
        }
        public virtual void KillEntity(params object[] vs)
        {
            IEnumerator Check(params object[] vs1)
            {
                while (true)
                {
                    if (IsIdle)
                    {
                        yield return new EntityEventPack(DefeatedAnimation).Execute();
                    }
                    else
                    {
                        yield return new WaitForSeconds(0.1f);
                    }
                }
            }
            Action(Check);
        }
        public virtual void DefeatedAnimation(params object[] vs)
        {
            InPerformingAnimation();
            Animator.SetBool(isDefeated, true);
            Action(new EntityEventPack(IdleDelay, DefeatedDuration), new EntityEventPack((object[] vvs) => { Destroy(); }));
        }


        protected virtual void SkillAction()
        {
            Debug.Log("Performing skill");
            Action(new EntityEventPack(IdleDelay, SkillDuration));
        }
        public abstract void SkillExecute(Effect effect);

        public override void Destroy()
        {
            Game.CurrentBattle.Buildings.Remove(data);
            base.Destroy();
        }

        public static BuildingEntity Create(BattleBuilding battleBuilding)
        {
            GameObject gameObject;
            BuildingEntity buildingEntity;

            CellEntity cellEntity = Game.CurrentBattle.MapEntity.GetCell(battleBuilding.Coordinate);
            gameObject = Instantiate(battleBuilding.Prefab, Game.CurrentBattle.MapEntity[battleBuilding.Coordinate].transform);

            buildingEntity = gameObject.GetComponent<BuildingEntity>();
            buildingEntity.data = battleBuilding;
            buildingEntity.name = "Building";
            cellEntity.Enter(buildingEntity, null);

            return buildingEntity;
        }

    }

    [Serializable]
    public class BattleBuilding : BattleEntityData
    {
        public override GameObject Prefab { get => prefab.Exist() ?? GameData.Prefabs.DefaultBuilding; set => prefab = value; }
        public override Prototype Prototype { get => GameData.Prototypes.GetBuildingPrototype(name); set => base.Prototype = value; }
        public new BuildingEntity Entity => base.Entity as BuildingEntity;


        protected override string GetDisplayingName()
        {
            if (HasValidPrototype)
            {
                return base.GetDisplayingName();
            }
            else
            {
                return ("Canute.BattleSystem.Building." + name + ".name").Lang();
            }
        }
    }
}
