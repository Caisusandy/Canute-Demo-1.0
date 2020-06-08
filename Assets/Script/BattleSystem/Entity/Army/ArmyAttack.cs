using Canute.BattleSystem.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{
    public static class ArmyAttack
    {
        public static ArmyEntity attackingArmy;
        public static List<CellEntity> border = new List<CellEntity>();
        public static List<Entity> expectedTargets = new List<Entity>();
        public static List<Entity> targets = new List<Entity>();
        public static Effect currentAttackingEffect;

        public static void Initialize()
        {
            attackingArmy = null;
            border = new List<CellEntity>();
            targets = new List<Entity>();
            expectedTargets = new List<Entity>();
            currentAttackingEffect = new Effect();
        }

        public static bool PrepareAttack(Effect effect, ArmyEntity armyEntity)
        {
            /**
             * count :
             * parameter : target count
             */
            Initialize();
            if (effect.Parameter == 0)
            {
                BattleUI.SendMessage(BattleEventError.armyCannotAttack);
                return false;
            }

            expectedTargets = armyEntity.GetTargets().ToList<Entity>();
            currentAttackingEffect = effect;

            if (expectedTargets.Count == 0 && armyEntity.data.Type != Army.Types.airship)
            {
                BattleUI.SendMessage("No enemy is in attack range");
                return false;
            }

            attackingArmy = armyEntity;
            //if (expectedTargets.Count == 1 && attackingArmy.data.TargetType == ArmyProperty.TargetTypes.single)
            //{
            //    SelecteAttackTarget(expectedTargets[0]);
            //    return true;
            //}

            AddAttackEvent();
            return true;
        }

        public static void TryAttack()
        {
            Debug.Log("Try to attack " + targets.Count + " entity");
            int damage = attackingArmy.data.Damage;

            Card.LastCard.Effect.Type = Effect.Types.attack;
            Card.LastCard.Effect.Source = attackingArmy;
            Card.LastCard.Effect.Targets = targets;
            Card.LastCard.Effect.Count = 1;
            Card.LastCard.Effect.Parameter = damage;

            if (attackingArmy.data.Properties.Attack.IsTypeOf(BattleProperty.AttackType.area) || attackingArmy.data.Properties.Attack.IsTypeOf(BattleProperty.AttackType.splash))
            {
                Card.LastCard.Effect.SetCellParam((Card.LastCard.Effect.Target as OnMapEntity).OnCellOf);
            }

            if (!Card.LastCard.Play())
                Card.LastCard.Effect.Type = Effect.Types.enterAttack;
            else
                RemoveAttackEvent();
        }

        public static void SelecteAttackTarget(Entity onMapEntity)
        {
            Entity target;

            if (!(onMapEntity is CellEntity))
            {
                target = onMapEntity;
            }
            else
            {
                if ((onMapEntity as CellEntity).HasArmyStandOn)
                {
                    target = (onMapEntity as CellEntity).HasArmyStandOn;
                }
                else if ((onMapEntity as CellEntity).HasBuildingStandOn)
                {
                    target = (onMapEntity as CellEntity).HasBuildingStandOn;
                }
                else
                {
                    target = onMapEntity;
                }
            }

            if (!(target is CellEntity))
            {
                if (!(target is IPassiveEntity))
                {
                    Debug.Log("not a passive target");
                    return;
                }
                if (!expectedTargets.Contains(target))
                {
                    Debug.Log("not a expected target");
                    return;
                }
                if (targets.Contains(target))
                {
                    bool s = targets.Remove(target);
                    Debug.Log("target remove " + s);
                    return;
                }
                if ((target as IPassiveEntity).Data.StatList.HasStatus(Effect.Types.tag, "protection"))
                {
                    BattleUI.SendMessage("Cannot attack: target is under protection");
                    return;
                }
            }
            //if (attackingArmy.data.Properties.TargetType == ArmyProperty.TargetTypes.single)
            if (attackingArmy.data.Properties.Attack.IsTypeOf(BattleProperty.AttackType.single))
            {
                targets.Add(target);
                Debug.Log("add target");
                if (currentAttackingEffect.Parameter == targets.Count || expectedTargets.Count == targets.Count)
                {
                    TryAttack();
                }
                else
                {
                    return;
                }
            }
            else if (attackingArmy.data.Properties.Attack.IsTypeOf(BattleProperty.AttackType.area) || attackingArmy.data.Properties.Attack.IsTypeOf(BattleProperty.AttackType.splash))
            {
                targets = new List<Entity> { target };
                TryAttack();
            }
            else
            {
                Debug.Log("not understand the attack type " + attackingArmy.data.Properties.Attack.ToString());
            }
        }

        public static void UnselecteAttackTarget(Entity entity)
        {
            Entity target = entity.transform.Find("Army")?.GetComponent<Entity>() ?? entity.transform.Find("Building")?.GetComponent<Entity>() ?? entity;

            if (targets.Contains(target))
            {
                bool s = targets.Remove(target);
                Debug.Log("target remove " + s);
            }
        }

        #region 移动状态切换器
        /// <summary> 进入移动状态 </summary>
        private static void AddAttackEvent()
        {
            Debug.Log("add attack event");
            Game.CurrentBattle.InAttackAction();
            BattleUI.HandCardBar.HideCards(true);
            ShowAttackRange();

            EffectExecute.AddSelectEvent(SelecteAttackTarget);
            Entity.UnselectEvent += UnselecteAttackTarget;
        }

        /// <summary> 退出移动状态 </summary>
        public static void RemoveAttackEvent()
        {
            EffectExecute.RemoveSelectEvent(SelecteAttackTarget);
            Entity.UnselectEvent -= UnselecteAttackTarget;
            EndShowAttackRange();
            BattleUI.HandCardBar.HideCards(false);
            Game.CurrentBattle.TryInNormal();
            Debug.Log("remove attack event");
        }

        public static void ShowAttackRange()
        {
            Game.CurrentBattle.MapEntity.StartCoroutine(new EntityEventPack(Draw).GetEnumerator());

            border = attackingArmy.GetAttackArea();

            IEnumerator Draw(params object[] vs)
            {
                while (Game.CurrentBattle.CurrentStat == Battle.Stat.attack)
                {
                    Mark.Load(Mark.Type.attackRange, border);
                    yield return new WaitForFixedUpdate();
                }
                yield return null;
            }
        }

        public static void EndShowAttackRange()
        {
            Mark.Unload(Mark.Type.attackRange, border);
            border = null;
        }


        #endregion

        public static List<ArmyEntity> GetTargets(this ArmyEntity armyEntity)
        {
            List<ArmyEntity> possibleTargets = new List<ArmyEntity>();
            List<CellEntity> cellEntities = armyEntity.GetAttackArea();
            for (int i = cellEntities.Count - 1; i >= 0; i--)
            {
                CellEntity cellEntity = cellEntities[i];
                if (!cellEntity.HasArmyStandOn)
                {
                    cellEntities.RemoveAt(i);
                    continue;
                }

                ArmyEntity possibleTarget = cellEntity.transform.Find("Army").GetComponent<ArmyEntity>();
                bool v = (possibleTarget.data.StandPosition & armyEntity.data.AttackPosition) == BattleProperty.Position.none;
                //Debug.Log(possibleTarget.data.StandPosition);
                //Debug.Log(armyEntity.data.AttackPosition);
                //Debug.Log(v);
                if (v)
                {
                    cellEntities.RemoveAt(i);
                    continue;
                }
                else if (possibleTarget.Owner == armyEntity.Owner)
                {
                    cellEntities.RemoveAt(i);
                    continue;
                }
                else
                {
                    possibleTargets.Add(possibleTarget);
                }
            }

            Debug.Log(possibleTargets.Count);
            return possibleTargets;
        }

        //public static List<ArmyEntity> GetTargetAfterMove(this ArmyEntity armyEntity)
        //{
        //    List<ArmyEntity> armyEntities = armyEntity.GetTargets();
        //    foreach (var item in armyEntity.GetMoveRange())
        //    {
        //        armyEntities = armyEntities.Union(armyEntity.GetTargets(item, armyEntity.data.Properties.AttackRange)).ToList();
        //    }
        //    return armyEntities;
        //}

        public static List<ArmyEntity> GetEnemyInRange(this ArmyEntity armyEntity, CellEntity origin, int range)
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

                ArmyEntity possibleTarget = cellEntity.transform.Find("Army").GetComponent<ArmyEntity>();
                if ((possibleTarget.data.StandPosition & armyEntity.data.StandPosition) == BattleProperty.Position.none)
                {
                    cellEntities.RemoveAt(i);
                    continue;
                }
                else if (possibleTarget.Owner == armyEntity.Owner)
                {
                    cellEntities.RemoveAt(i);
                    continue;
                }
                else
                {
                    possibleTargets.Add(possibleTarget);
                }
            }

            Debug.Log(possibleTargets.Count);
            return possibleTargets;
        }
    }
}
