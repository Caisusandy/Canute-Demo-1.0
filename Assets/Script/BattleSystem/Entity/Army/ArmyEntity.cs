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
        public ArmyHealthBar armyHealthBar;

        public MarkController attackRange = new MarkController(CellMark.Type.attackRange);
        public MarkController moveRange = new MarkController(CellMark.Type.moveRange);

        public override EntityData Data => data;

        public static List<ArmyEntity> onMap = new List<ArmyEntity>();

        IBattleEntityData IBattleEntity.Data => data;
        IPassiveEntityData IPassiveEntity.Data => data;
        IAggressiveEntityData IAggressiveEntity.Data => data;
        IBattleableEntityData IBattleableEntity.Data => data;

        public override BattleProperty.Position StandPostion => data.StandPosition;
        public GameObject ArmyObject => transform.GetChild(0).gameObject;

        public override void Awake()
        {
            base.Awake();
            if (!onMap.Contains(this)) { onMap.Add(this); }
            entityMark = new MarkController(CellMark.Type.owner);
        }

        public override void Start()
        {
            base.Start();
            CreateHealthBar();
            //Color color = data.Career.GetColor();
            //color.a = 0.4f;
        }

        public override void Update()
        {
            if (transform.localPosition != Vector3.zero && !GetComponent<EntityOnCellMotion>())
            {
                Module.Motion.SetMotion(gameObject, Vector3.zero, Space.Self);
            }
        }

        public override void OnDestroy()
        {
            onMap.Remove(this);
            Game.CurrentBattle?.Armies?.Remove(data);
            Unselect();
            base.OnDestroy();
        }

        public override void OnMouseDown()
        {
            if (isSelected)
            {
                ArmyObject.transform.localScale /= 1.1f;
            }
            else
            {
                ArmyObject.transform.localScale *= 1.1f;
            }
        }


        public override void Highlight()
        {
            if (isHighlighted)
            {
                return;
            }
            try
            {
                ArmyObject.transform.localScale *= 1.2f;
                IsHighlighted = true;
            }
            catch { }
            var attackRange = GetAttackRange();
            var moveRange = GetMoveRange();

            this.attackRange.Refresh(attackRange);
            this.moveRange.Refresh(moveRange);
            this.moveRange.Display();
            this.attackRange.Display();
        }

        public override void Unhighlight()
        {
            if (!isHighlighted)
            {
                return;
            }
            try
            {
                ArmyObject.transform.localScale = new Vector3(1, 1, 1);
                IsHighlighted = false;
            }
            catch { }
            moveRange.ClearDisplay();
            attackRange.ClearDisplay();
        }

        public override void CardTargetHighlight()
        {
            ArmyObject.GetComponent<SpriteRenderer>().color = HightGreen;
            foreach (Transform item in ArmyObject.transform)
            {
                SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
                if (spriteRenderer)
                    spriteRenderer.color = HightGreen;
            }
        }

        public override void CardTargetUnhighlight()
        {
            ArmyObject.GetComponent<SpriteRenderer>().color = Color.white;
            foreach (Transform item in ArmyObject.transform)
            {
                SpriteRenderer spriteRenderer = item.GetComponent<SpriteRenderer>();
                if (spriteRenderer)
                    spriteRenderer.color = Color.white;
            }
        }


        public abstract float AttackAtionDuration { get; }
        public abstract float SkillDuration { get; }
        public abstract float DefeatedDuration { get; }
        public abstract float WinningDuration { get; }
        public abstract float HurtDuration { get; }

        public virtual void Attack(params object[] vs)
        {
            Effect effect = vs[0] as Effect;

            InPerformingAnimation();
            Animator.SetBool(isAttacking, true);

            AttackExecute(effect);
        }
        public virtual void Move(params object[] vs)
        {
            List<CellEntity> path = vs[0] as List<CellEntity>;
            Effect effect = vs[1] as Effect;

            InPerformingAnimation();
            Animator.SetBool(isMoving, true);

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
            IAggressiveEntity damageSource;
            try { damageSource = vs[1] as IAggressiveEntity; }
            catch { damageSource = null; }

            this.Damage(damage, damageSource);


            InPerformingAnimation();
            Animator.SetBool(isDefencing, true);

            Action(new EntityEventPack(IdleDelay, HurtDuration), new EntityEventPack(data.CheckPotentialAction, damageSource));
            Debug.Log(Data.ToString() + " Hurt");
        }
        public virtual void Skill(params object[] vs)
        {
            Effect effect = vs[0] as Effect;
            InPerformingAnimation();
            Animator.SetBool(isPerformingSkill, true);

            SkillAction(effect);
        }
        public virtual void Winning(params object[] vs)
        {
            InPerformingAnimation();
            Animator.SetBool(isWinning, true);
            Action(new EntityEventPack(IdleDelay, WinningDuration));
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


        /// <summary>
        /// skill code
        /// </summary>
        /// <param name="effect">skill effect (<paramref name="effect"/>.type = skill)</param>
        public virtual void SkillExecute(Effect effect)
        {
            effect.Type = Effect.Types.@event;
            effect.Execute();
        }
        /// <summary>
        /// Attack code
        /// </summary>
        /// <param name="effect">attack effect(<paramref name="effect"/>.type = attack)</param>
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
        /// <summary>
        /// get an(or more) actual target when attack
        /// </summary>
        /// <param name="effect"></param>
        public virtual void GetAttackTarget(ref Effect effect)
        {

        }

        /// <summary>
        /// Attack action code
        /// </summary>
        /// <param name="attackingEntity"></param>
        /// <param name="damage"></param>
        protected virtual void AttackAction(IPassiveEntity attackingEntity, int damage)
        {
            Action(new EntityEventPack(IdleDelay, AttackAtionDuration), new EntityEventPack(data.CheckPotentialAction, attackingEntity), new EntityEventPack(attackingEntity.Hurt, damage, this));
        }
        /// <summary>
        /// skill action code
        /// </summary>
        /// <param name="effect"></param>
        protected virtual void SkillAction(Effect effect)
        {
            Debug.Log("Performing skill");
            Action(new EntityEventPack(IdleDelay, SkillDuration), new EntityEventPack(SkillExecute, effect));

            void SkillExecute(params object[] vs)
            {
                Effect skillEffect = vs[0] as Effect;
                this.SkillExecute(skillEffect);
            }
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

        public override void Destroy()
        {
            Game.CurrentBattle.Armies.Remove(data);
            base.Destroy();
        }

        /// <summary> 创建军队实体，自动分配一个ArmyInfoPanel </summary>
        /// <param name="battleArmy"></param> 
        public static ArmyEntity Create(BattleArmy battleArmy, bool createArmyBarInfoIcon = true)
        {
            GameObject gameObject;
            ArmyEntity armyEntity;

            CellEntity cellEntity = Game.CurrentBattle.MapEntity.GetCell(battleArmy.Coordinate);
            Debug.Log(cellEntity.transform);
            Debug.Log(battleArmy.Prefab);
            gameObject = Instantiate(battleArmy.Prefab, cellEntity.transform);
            armyEntity = gameObject.GetComponent<ArmyEntity>();
            armyEntity.data = battleArmy;
            armyEntity.name = "Army";
            cellEntity.Enter(armyEntity, null);

            if (battleArmy.Owner == Game.CurrentBattle.Player && createArmyBarInfoIcon)
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

        public void CreateHealthBar()
        {
            armyHealthBar = Instantiate(GameData.Prefabs.Get("armyHealthBar")).GetComponent<ArmyHealthBar>();
            armyHealthBar.transform.SetParent(transform);
            armyHealthBar.transform.localPosition = new Vector3(0, -18, 0);
            armyHealthBar.transform.localScale = new Vector3(0.4f, 0.5f, 0);
            armyHealthBar.armyEntity = this;
        }

    }
}
