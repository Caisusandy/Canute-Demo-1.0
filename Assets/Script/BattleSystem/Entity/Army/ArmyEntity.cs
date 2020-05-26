using Canute.BattleSystem.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem
{
    /// <summary>
    /// Army 的基类
    /// <para>所有的Army都是从这个类型分出的</para>
    /// </summary>
    [RequireComponent(typeof(Animator))]
    public abstract class ArmyEntity : OnMapEntity, IBattleEntity, IPassiveEntity, IAggressiveEntity, IBattleableEntity
    {
        public BattleArmy data;

        public List<CellEntity> attackRange = new List<CellEntity>();
        public List<CellEntity> moveRange = new List<CellEntity>();

        public override EntityData Data => data;

        public static List<ArmyEntity> onMap = new List<ArmyEntity>();

        IBattleEntityData IBattleEntity.Data => data;
        IPassiveEntityData IPassiveEntity.Data => data;
        IAggressiveEntityData IAggressiveEntity.Data => data;
        IBattleableEntityData IBattleableEntity.Data => data;

        public override BattleProperty.Position StandPostion => data.StandPosition;

        public override void Awake()
        {
            base.Awake();
            if (!onMap.Contains(this)) { onMap.Add(this); }
        }

        public override void Start()
        {
            base.Start();
            Color color = data.Career.GetColor();
            color.a = 0.4f;
        }

        public override void Update()
        {
            OnCellOf.Highlight(Mark.Type.owner);
            if (transform.localPosition != Vector3.zero && !GetComponent<EntityOnCellMotion>())
            {
                Module.Motion.SetMotion(gameObject, Vector3.zero, Space.Self);
            }
        }

        public override void OnDestroy()
        {
            onMap.Remove(this);
            Game.CurrentBattle?.Armies?.Remove(data);

            base.OnDestroy();
        }

        public override void OnMouseDown()
        {
            if (isSelected)
            {
                transform.localScale /= 1.1f;
            }
            else
            {
                transform.localScale *= 1.1f;
            }
        }


        public override void Highlight()
        {
            base.Highlight();
            attackRange = GetAttackRange();
            moveRange = GetMoveRange();

            Mark.Load(Mark.Type.attackRange, attackRange);
            Mark.Load(Mark.Type.moveRange, moveRange);
        }

        public override void Unhighlight()
        {
            base.Unhighlight();
            Mark.Unload(Mark.Type.attackRange, attackRange);
            Mark.Unload(Mark.Type.moveRange, moveRange);
        }



        public abstract float AttackAtionDuration { get; }
        public abstract float SkillDuration { get; }
        public abstract float DefeatedDuration { get; }
        public abstract float WinningDuration { get; }
        public abstract float HurtDuration { get; }

        /// <summary>
        /// Animation when entity is defeated
        /// </summary>
        /// <param name="vs"></param>
        public virtual void Defeated(params object[] vs)
        {
            InPerformingAnimation();
            animator.SetBool(isDefeated, true);
            Action(new EntityEventPack(IdleDelay, DefeatedDuration), new EntityEventPack(Remove));
        }
        public virtual void Skill(params object[] vs)
        {
            Effect effect = vs[0] as Effect;
            InPerformingAnimation();
            animator.SetBool(isPerformingSkill, true);
            SkillExecute(effect);
            SkillAction(effect);
        }
        public virtual void Winning(params object[] vs)
        {
            InPerformingAnimation();
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
        public virtual void Attack(params object[] vs)
        {
            Effect effect = vs[0] as Effect;

            InPerformingAnimation();
            animator.SetBool(isAttacking, true);

            AttackExecute(effect);
        }
        public virtual void Move(params object[] vs)
        {
            List<CellEntity> path = vs[0] as List<CellEntity>;
            Effect effect = vs[1] as Effect;

            InPerformingAnimation();
            animator.SetBool(isMoving, true);

            EntityOnCellMotion.SetMotion(this, path, effect);
            Action(TryEndMoveAction, new EntityEventPack(data.CheckPotentialAction));
            Unselect();


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
        public virtual void Hurt(params object[] vs)
        {
            int damage = (int)vs[0];
            var damageSource = vs[1] as IAggressiveEntity;

            if (damageSource is null)
            {
                this.Damage(damage);
            }
            else
            {
                this.Damage(damage, damageSource);
            }

            InPerformingAnimation();
            animator.SetBool(isDefencing, true);

            Action(new EntityEventPack(IdleDelay, HurtDuration), new EntityEventPack(data.CheckPotentialAction));
            Debug.Log(Data.ToString() + " Hurt");
        }

        public abstract void SkillExecute(Effect effect);
        public virtual void AttackExecute(Effect effect)
        {
            foreach (var item in effect.Targets)
            {
                IPassiveEntity target = item as IPassiveEntity;
                for (int i = 0; i < effect.Count; i++)
                {

                    AttackAction(target, effect.Parameter);
                }
            }
        }
        public virtual void GetAttackTarget(ref Effect effect)
        {

        }

        protected virtual void AttackAction(IPassiveEntity attackingEntity, int damage)
        {
            Action(new EntityEventPack(IdleDelay, AttackAtionDuration), new EntityEventPack(data.CheckPotentialAction), new EntityEventPack(attackingEntity.Hurt, damage, this));
        }
        protected virtual void SkillAction(Effect effect)
        {
            Debug.Log("Performing skill");
            /*
             */
            Action(new EntityEventPack(IdleDelay, SkillDuration));
        }

        /// <summary>
        /// Try to end animation "move"
        /// </summary>  

        public virtual List<CellEntity> GetMoveArea() => data.GetMoveArea();

        public virtual List<CellEntity> GetMoveRange() => MapEntity.GetBorderCell(GetMoveArea());

        public virtual List<CellEntity> GetAttackArea() => data.GetAttackArea();

        public virtual List<CellEntity> GetAttackRange() => MapEntity.GetBorderCell(GetAttackArea());

        public virtual bool CanAttack(IPassiveEntity other)
        {
            return data.CanAttack(other.Data);
        }



        /// <summary> 创建军队实体，自动分配一个ArmyInfoPanel </summary>
        /// <param name="battleArmy"></param> 
        public static ArmyEntity Create(BattleArmy battleArmy)
        {
            GameObject gameObject;
            ArmyEntity armyEntity;

            CellEntity cellEntity = Game.CurrentBattle.MapEntity.GetCell(battleArmy.Coordinate);
            gameObject = Instantiate(battleArmy.Prefab, cellEntity.transform);
            armyEntity = gameObject.GetComponent<ArmyEntity>();
            armyEntity.data = battleArmy;
            armyEntity.name = "Army";
            cellEntity.Enter(armyEntity, null);

            if (battleArmy.Owner == Game.CurrentBattle.Player)
            {
                ArmyInfoIcon armyInfo = BattleUI.ArmyBar.GetAvailableSlot();
                armyInfo.Connect(armyEntity);
            }

            return armyEntity;
        }

        public static List<ArmyEntity> GetArmies(CellEntity origin, int range)
        {
            List<ArmyEntity> possibleTargets = new List<ArmyEntity>();
            List<CellEntity> cellEntities = Game.CurrentBattle.MapEntity.GetNearbyCell(origin, range);
            Debug.Log(cellEntities.Count);
            for (int i = cellEntities.Count - 1; i >= 0; i--)
            {
                CellEntity cellEntity = cellEntities[i];
                if (!cellEntity.HasArmyStandOn)
                {
                    cellEntities.RemoveAt(i);
                    continue;
                }

                possibleTargets.Add(cellEntity.HasArmyStandOn);
            }

            possibleTargets.Remove(origin.HasArmyStandOn);

            Debug.Log(possibleTargets.Count);
            return possibleTargets;
        }



    }
}
