using Canute.BattleSystem.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{
    public delegate void EffectExecutor(Effect effect);
    public delegate void StatusExecutor(ref Effect effect, Status status);

    //public abstract class EffectExecution
    //{
    //    public abstract void Execute(Effect effect);
    //}

    //public abstract class StatusExecution
    //{
    //    public abstract void Execute(ref Effect effect, Status status);
    //}

    public static partial class EffectExecute
    {
        #region delegates
        public static event SelectEvent TargetSelectEvent;

        public static void SelectExecutorTarget(Entity entity)
        {
            TargetSelectEvent?.Invoke(entity);
        }

        public static void AddSelectEvent(SelectEvent selectEvent)
        {
            TargetSelectEvent += selectEvent;
            Game.CurrentBattle.InAction(SelectExecutorTarget);
        }

        public static void RemoveSelectEvent(params SelectEvent[] selectEvents)
        {
            foreach (var selectEvent in selectEvents)
            {
                TargetSelectEvent -= TargetSelectEvent;
                if (TargetSelectEvent is null)
                {
                    Game.CurrentBattle.EndAction(SelectExecutorTarget);
                }
            }
        }

        public static void CancelSelectTargetEvent()
        {
            foreach (var item in TargetSelectEvent.GetInvocationList())
            {
                TargetSelectEvent -= item as SelectEvent;
            }
        }
        #endregion

        public static Entity DelayTarget;
        public static Effect LastEffect => Game.CurrentBattle.PassingEffect.Count == 0 ? null : Game.CurrentBattle.PassingEffect[Game.CurrentBattle.PassingEffect.Count - 1];

        #region Framework
        /// <summary>
        /// Execute a effect pack
        /// <para>if is a primary effect, it will be recorded</para>
        /// </summary>
        /// <param name="effect"></param>
        /// <param name="isPrimaryEffect"></param>
        /// <returns></returns>
        public static bool Execute(this Effect effect, bool isPrimaryEffect = false)
        {
            bool result = false;

            if (effect is null)
            {
                return false;
            }

            switch (effect.Type)
            {

                case Effect.Types.none:
                    result = true;    //空effect永远返回真
                    break;
                case Effect.Types.enterMove:
                    result = EnterMove(effect);
                    break;
                case Effect.Types.move:
                    result = Move(effect);
                    break;
                case Effect.Types.enterAttack:
                    result = EnterAttack(effect);
                    break;
                case Effect.Types.attack:
                    result = BeforeAttack(effect);
                    break;
                case Effect.Types.realDamage:
                    result = RealDamage(effect);
                    break;
                case Effect.Types.skill:
                    result = Skill(effect);
                    break;
                case Effect.Types.createArmy:
                    result = CreateArmy(effect);
                    break;
                case Effect.Types.addStatus:
                    result = AddStatus(effect);
                    break;
                case Effect.Types.removeStatus:
                    result = RemoveStatus(effect);
                    break;
                case Effect.Types.@event:
                    result = Event(effect);
                    break;
            }

            Debug.Log(effect);
            if (result)
            {
                if (effect.Type != Effect.Types.none && isPrimaryEffect)
                {
                    Game.CurrentBattle.PassingEffect.Add(effect);
                }
            }
            else
            {
                //Debug.Log(Entity.entities.Count);
            }

            Game.CurrentBattle.ScoreBoard.AllPassedEffect.Add(effect);
            return result;
        }

        public static void StatusMerge(this Status status, Status mergingStatus)
        {
            switch (status.Effect.Type)
            {
                default:
                    status.Effect.Count += mergingStatus.Effect.Count;
                    status.Effect.Parameter += mergingStatus.Effect.Parameter;
                    break;
            }
        }

        #region Attack
        /*
         * Attack 全流程
         * 1. ArmyAttack内选择目标
         * 2. 获得军队攻击值
         * 3. 触发Source/Target的效果
         * 4. Source开始打击Target
         * 5. 计算Target护甲值
         * 6. Target扣血
         */
        public static bool BeforeAttack(Effect effect)
        {
            #region Checking

            if (effect.Type != Effect.Types.attack)
            {
                Debug.LogError("No idea why a effect with type of " + effect.Type + " comes here. " + effect.ToString());
                return false;
            }

            IAggressiveEntity source = effect.Source as IAggressiveEntity;
            if (source is null)
            {
                Debug.LogError("the source is not aggresive");
                return false;
            }

            if (effect.Targets.Count == 0)
            {
                Debug.LogError("no target found");
                return false;
            }

            source.GetAttackTarget(ref effect);

            if (effect.Targets.Count == 0)
            {
                Debug.Log("The position for multiple target is illegal");
                return false;
            }


            if (effect.Type != Effect.Types.attack)
            {
                return effect.Execute();
            }

            Debug.Log(effect.Targets.Count);

            foreach (var item in effect.Targets)
            {
                if (effect.Source.Owner == item.Owner)
                {
                    Debug.Log("you are attacking yourself " + effect.ToString());
                    ArmyAttack.targets.Clear();
                    return false;
                }
                if (!(item is IPassiveEntity))
                {
                    Debug.Log("target is not a passive entity " + effect.ToString());
                    return false;
                }
            }

            #endregion

            Debug.Log("Attack Start");

            foreach (Entity entity in effect.AllEntities)
            {
                IBattleableEntity battleEntityData = entity as IBattleableEntity;
                if (battleEntityData is null)
                {
                    continue;
                }
                battleEntityData.Data.Trigger(TriggerCondition.Conditions.beforeAttack, ref effect);
            }

            InAttack(effect);
            AfterDefence(effect);

            return true;
        }

        private static void InAttack(Effect effect)
        {
            IAggressiveEntity agressiveEntity = effect.Source as IAggressiveEntity;
            agressiveEntity.Data.Trigger(TriggerCondition.Conditions.attack, ref effect);

            foreach (var item in effect.Targets)
            {
                Effect attackEffect = effect.Clone();
                attackEffect.UUID = effect.UUID;
                attackEffect.Target = item;
                Defence(attackEffect);
            }
        }

        private static void Defence(Effect effect)
        {
            IAggressiveEntity source = effect.Source as IAggressiveEntity;
            IPassiveEntity target = effect.Target as IPassiveEntity;

            target.Data.Trigger(TriggerCondition.Conditions.defense, ref effect);

            string type = effect["attackType"];
            if (string.IsNullOrEmpty(type))
            {
                source.Attack(effect);
            }
            else
            {
                RewriteAttackEffect(ref effect);
                source.Attack(effect);
            }
        }

        private static void AfterDefence(Effect effect)
        {
            foreach (Entity entity in effect.AllEntities)
            {
                IBattleableEntity battleEntity = entity as IBattleableEntity;
                if (battleEntity is null)
                {
                    continue;
                }
                battleEntity.Data.Trigger(TriggerCondition.Conditions.afterDefence, ref effect);
            }

            Debug.Log("Attack End");

            {
                IAggressiveEntity agressiveEntity = effect.Source as IAggressiveEntity;
                agressiveEntity.Data.Anger += 40;
            }

            foreach (Entity item in effect.Targets)
            {
                IPassiveEntity passiveEntity = item as IPassiveEntity;
                passiveEntity.Data.Anger += 20;
            }
        }

        /// <summary>
        /// Reorganize attack effect to a normal attack effect
        /// <para>When attack effect has arg {attackType}, it behave different than a normal one</para>
        /// </summary>
        /// <param name="effect"></param>
        private static void RewriteAttackEffect(ref Effect effect)
        {
            string type = effect["attackType"];
            CellEntity centerCell = effect.GetCellParam();

            IPassiveEntity target = effect.Target as IPassiveEntity;
            IAggressiveEntity source = effect.Source as IAggressiveEntity;

            switch (type)
            {
                case "splash":
                    if (target.OnCellOf != centerCell) effect.Parameter /= 2;
                    break;
                case "line":

                    break;
                case "percentage":
                    effect.Parameter = target.Data.Health * effect.Parameter / 100;
                    break;
                case "shielder":
                    if ((target as ArmyEntity).Exist()?.data.Type != Army.Types.shielder)
                    {
                        effect.Parameter += target.Data.Defense;
                        effect.Parameter += (int)(target.Data.Health * 0.16f);
                    }
                    break;
                case "area":
                    if (target.OnCellOf == centerCell)
                    {
                        effect.Parameter = effect.Parameter.Bonus(-30);
                        break;
                    }

                    for (int i = 0; i < source.Data.Properties.AttackArea + 1; i++)
                    {
                        Debug.Log("Level " + (i + 1));
                        List<CellEntity> cellEntities = MapEntity.CurrentMap.GetBorderCell(centerCell, i + 1, BattleProperty.AttackType.projectile);

                        CellEntity standOn = null;
                        foreach (var cell in cellEntities.Where(cell => target.Coordinate == cell.Coordinate).Select(cell => cell))
                        {
                            standOn = cell;
                        }

                        if (standOn != null)
                        {
                            Debug.Log("found target " + target.Name + "at level " + i);
                            effect.Parameter = effect.Parameter.Bonus((Math.Pow(0.7, i + 2) - 1) * 100);
                            break;
                        }
                        else continue;
                    }
                    break;
                default:
                    return;
            }

        }

        #endregion

        #endregion

        private static bool AddStatus(Effect effect)
        {
            Debug.Log("Adding Status");

            if (effect.Type != Effect.Types.addStatus)
            {
                Debug.Log("Not Adding Status");
                return false;
            }

            foreach (var item in effect.Targets)
            {
                if (!(item.Data is IStatusContainer))
                {
                    Debug.Log("not a status container");
                    return false;
                }
            }

            foreach (var item in effect.Targets)
            {
                IStatusContainer statusContainer = item as IStatusContainer;
                statusContainer.Trigger(TriggerCondition.Conditions.addingStatus, ref effect);
                Status status = effect.ToStatus();
                statusContainer.StatList.Add(status);
            }

            return true;
        }

        private static bool RemoveStatus(Effect effect)
        {
            IStatusContainer statusContainer = effect.Target as IStatusContainer;
            UUID uuid = new Guid(effect["uuid"]);

            if (effect.Type != Effect.Types.removeStatus)
            {
                BattleUI.SendMessage("Remove Failed");
                return false;
            }

            if (statusContainer is null)
            {
                BattleUI.SendMessage("Remove Failed");
                return false;
            }

            if (uuid == UUID.Empty)
            {
                BattleUI.SendMessage("Remove Failed");
                return false;
            }

            for (int i = 0; i < statusContainer.StatList.Count; i++)
            {
                Status status = statusContainer.StatList[i];
                if (status.UUID == uuid)
                {
                    statusContainer.StatList.RemoveAt(i);
                    return true;
                }
            }

            BattleUI.SendMessage("status does not exist");
            return true;
        }

        private static bool EnterAttack(Effect effect)
        {
            if (effect.Type != Effect.Types.enterAttack)
            {
                return false;
            }
            if (effect.Source is CardEntity && effect.Target.Owner != effect.Source.Owner)
            {
                return false;
            }
            ArmyEntity armyEntity = effect.Target as ArmyEntity;
            if (armyEntity is null)
            {
                return false;
            }
            Debug.Log("Enter Attacking");
            effect.Parameter = armyEntity.data.Properties.TargetCount;
            bool sucess = ArmyAttack.PrepareAttack(effect, armyEntity);
            return sucess;
        }

        /// <summary>
        /// Player create army when game start
        /// </summary>
        /// <param name="effect"></param>
        /// <returns></returns>
        private static bool CreateArmy(Effect effect)
        {
            CellEntity cellEntity = (effect.Target as OnMapEntity)?.OnCellOf;
            BattleArmy battleArmy = (effect.Source as ArmyCardEntity)?.battleArmy;

            if (!(cellEntity || battleArmy is null))
            {
                return false;
            }

            if (cellEntity.transform.Find("Army"))
            {
                return false;
            }

            if (Buildings.CampusEntity.GetCampus(battleArmy.Owner).GetPointDistanceOf(cellEntity) > 5)
            {
                return false;
            }

            battleArmy.Coordinate = cellEntity.Coordinate;

            return ArmyEntity.Create(battleArmy);
        }

        private static bool EnterMove(Effect effect)
        {
            if (effect.Type != Effect.Types.enterMove)
            {
                return false;
            }
            ArmyEntity armyEntity = effect.Target as ArmyEntity;
            if (armyEntity is null)
            {
                return false;
            }
            //Debug.Log("Enter Moving");
            return ArmyMovement.PrepareMove(armyEntity);
        }

        private static bool Move(Effect effect)
        {
            ArmyEntity movingArmy = effect.Source as ArmyEntity;
            CellEntity destination = (effect.Target as OnMapEntity)?.OnCellOf;

            //Debug.Log(movingArmy);
            //Debug.Log(destination);

            if (movingArmy is null || destination is null)
            {
                Debug.Log("invalid effect");
                return false;
            }
            else if (!movingArmy.AllowMove)
            {

                return false;
            }
            else if (destination.HasArmyStandOn)
            {
                return false;
            }

            List<CellEntity> path = ArmyMovement.GetPath(); //PathFinder.GetPath(movingArmy.OnCellOf, destination, movingArmy);

            if (path is null)
            {
                return false;
            }
            else if (path.Count < 1)
            {
                return false;
            }
            else
            {
                movingArmy.data.Trigger(TriggerCondition.Conditions.move, ref effect);
                movingArmy.Move(path, effect);

                return true;
            }
        }

        private static bool Skill(Effect effect)
        {
            ISkillable sourceEntity = effect.Source as ISkillable;
            if (sourceEntity is null)
            {
                return false;
            }

            sourceEntity.Skill(effect);
            return true;
        }
    }

}