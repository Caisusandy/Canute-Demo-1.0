using Canute.BattleSystem.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{
    public delegate void EffectExecutor(Effect effect);
    public delegate void StatusExecutor(Status status, ref Effect effect);

    public static class EffectExecute
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
        public static bool Execute(this Effect effect, bool isPrimaryEffect = true)
        {
            //try
            //{

            bool result = false;
            OnMapEntity.SelectingEntity?.Unselect();

            if (effect is null)
            {
                return false;
            }

            if (effect.Args.ContainsKey(Effect.statusAddingEffect))
            {
                result = AddStatus(effect);
            }
            else
            {
                switch (effect.Type)
                {
                    case Effect.Types.createArmy:
                        result = CreateArmy(effect);
                        break;
                    case Effect.Types.none:
                        result = true;    //空effect永远返回真
                        break;
                    case Effect.Types.enterAttack:
                        result = EnterAttack(effect);
                        break;
                    case Effect.Types.attack:
                        result = BeforeAttack(effect);
                        break;
                    case Effect.Types.enterMove:
                        result = EnterMove(effect);
                        break;
                    case Effect.Types.move:
                        result = Move(effect);
                        break;
                    case Effect.Types.realDamage:
                        result = RealDamage(effect);
                        break;
                    case Effect.Types.skill:
                        result = Skill(effect);
                        break;
                    case Effect.Types.occupy:
                        result = Occupy(effect);
                        break;
                    case Effect.Types.addArmor:
                        result = AddArmor(effect);
                        break;
                    case Effect.Types.armySwitch:
                        result = ArmySwitch(effect);
                        break;
                    case Effect.Types.drawCard:
                        result = DrawCard(effect);
                        break;
                    case Effect.Types.addActionPoint:
                        result = AddActionPoint(effect);
                        break;
                }
            }

            if (!result)
            {
                Debug.Log(effect);
                Debug.Log(Entity.entities.Count);
            }
            else if (effect.Type != Effect.Types.none && isPrimaryEffect)
            {
                Game.CurrentBattle.PassingEffect.Add(effect);
            }
            return result;
            //}
            //catch (Exception e)
            //{
            //    Debug.LogError("An unexpected error happened when trying to execute the effect pack\n" + effect.ToString());
            //    throw e;
            //}
        }

        public static bool Execute(this Status status)
        {
            if (!status.IsValid)
            {
                Debug.LogError("An invalid status was trying to trigger " + status.Effect);
                return false;
            }

            bool success = status.Effect.Execute(false);

            if (success)
            {
                AfterExecute(status);
            }

            return success;
        }

        /// <summary>
        /// Status Executor, only those related to another effect
        /// <para>these status will not result failure</para>
        /// </summary>
        /// <param name="status"></param>
        /// <param name="sourceEffect"></param>
        public static void Execute(this Status status, ref Effect sourceEffect)
        {
            if (!status.IsValid)
            {
                Debug.LogError("An invalid status was trying to trigger " + status.Effect);
            }

            switch (status.Type)
            {
                case Effect.Types.damageIncreasePercentage:
                    DamageIncrease(ref sourceEffect, status);
                    break;
                case Effect.Types.damageDecreasePercentage:
                    DamageDecrease(ref sourceEffect, status);
                    break;
                default:
                    status.Execute();
                    break;
            }
            AfterExecute(status);
        }

        private static void AfterExecute(Status status)
        {
            if (status.IsBaseOnCount || status.IsDualBase)
            {
                status.StatCount--;
            }
        }



        public static ArmyProperty GetArmyProperty(IBattleableEntityData battleableEntity)
        {
            ArmyProperty property = battleableEntity.RawProperties;

            foreach (var stat in battleableEntity.StatList)
            {
                for (int i = 0; i < stat.Effect.Count; i++)
                {
                    switch (stat.Type)
                    {
                        case Effect.Types.propertyBounes:
                            property = SinglePropertyBounes(property, stat);
                            break;
                        default:
                            break;
                    }
                }
            }
            return property;
        }

        private static ArmyProperty SinglePropertyBounes(ArmyProperty property, Status stat)
        {
            PropertyType propertyType = stat.Effect.GetParam<PropertyType>(Effect.propertyBounes);
            BounesType bounesType = stat.Effect.GetParam<BounesType>(Effect.bounesType);
            var checkTypeValues = Enum.GetValues(typeof(PropertyType));
            foreach (PropertyType item in checkTypeValues)
            {
                switch (propertyType & item)
                {
                    case PropertyType.moveRange:
                        property.MoveRange = property.MoveRange.Bounes(stat.Effect.Parameter, bounesType);
                        break;
                    case PropertyType.attackRange:
                        property.AttackRange = property.AttackRange.Bounes(stat.Effect.Parameter, bounesType);
                        break;
                    case PropertyType.critRate:
                        property.CritRate = property.CritRate.Bounes(stat.Effect.Parameter, bounesType);
                        break;
                    case PropertyType.critBounes:
                        property.CritBounes = property.CritBounes.Bounes(stat.Effect.Parameter, bounesType);
                        break;
                    case PropertyType.pop:
                        property.Pop = property.Pop.Bounes(stat.Effect.Parameter, bounesType);
                        break;
                    default:
                        break;
                }
            }
            return property;
        }

        public static void StatusMerge(this Status status, Status mergingStatus)
        {
            switch (status.Type)
            {
                default:
                    status.Effect.Count += mergingStatus.Effect.Count;
                    status.Effect.Parameter += mergingStatus.Effect.Parameter;
                    break;
            }
        }

        public static bool AddStatus(Effect effect)
        {
            Debug.Log("Adding Status");

            if (effect[Effect.statusAddingEffect] == null)
            {
                Debug.Log("Not Adding Status");
                return false;
            }

            foreach (var item in effect.Targets)
            {
                if (!(item.Data is IStatusContainer))
                {
                    Debug.Log("not a scontainer");
                    return false;
                }
            }

            foreach (var item in effect.Targets)
            {
                IStatusContainer statusContainer = item as IStatusContainer;
                Status status = effect.ToStatus();
                statusContainer.StatList.Add(status);
            }

            return true;
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
            if (effect.Type != Effect.Types.attack)
            {
                Debug.LogError("No idea why a effect with type of " + effect.Type + " comes here. " + effect.ToString());
                return false;

            }

            if (!(effect.Source is IAggressiveEntity))
            {
                Debug.LogError("the source is not aggresive");
                return false;
            }

            if (effect.Targets.Count == 0)
            {
                Debug.LogError("no target found");
                return false;
            }

            if ((effect.Source as IAggressiveEntity).Data.Properties.Attack.IsTypeOf(ArmyProperty.AttackType.area) || (effect.Source as IAggressiveEntity).Data.Properties.Attack.IsTypeOf(ArmyProperty.AttackType.splash))
            {
                Debug.Log("multiple attack");
                List<Entity> passiveEntities = effect.Targets.ShallowClone();
                for (int i = effect.Targets.Count - 1; i > -1; i--)
                {
                    CellEntity item = (effect.Targets[i] as OnMapEntity)?.OnCellOf;
                    if (item)
                    {
                        BattleArmy data = (effect.Source as ArmyEntity).data;
                        List<ArmyEntity> posibleArmy = (effect.Source as ArmyEntity).GetTargets(item as CellEntity, data.Properties.AttackArea);
                        passiveEntities = passiveEntities.Union(posibleArmy).ToList();
                    }
                    if (!(item is IPassiveEntity))
                        passiveEntities.Remove(item);


                }
                if (passiveEntities.Count == 0)
                {
                    Debug.Log("The position for multiple target is illegal");
                    return false;
                }

                effect.Targets = passiveEntities;
            }

            foreach (Entity item in effect.Targets)
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

            Debug.Log("Attack Start");

            foreach (Entity entity in effect.AllEntities)
            {
                IBattleEntity battleEntityData = entity as IBattleEntity;
                if (battleEntityData is null)
                {
                    continue;
                }
                battleEntityData.Data.Trigger(TriggerCondition.Conditions.beforeAttack, ref effect);
            }
            InAttack(effect);
            return true;
        }

        private static void InAttack(Effect effect)
        {
            IAggressiveEntity agressiveEntity = effect.Source as IAggressiveEntity;
            agressiveEntity.Data.Trigger(TriggerCondition.Conditions.attack, ref effect);
            Defence(effect);
        }

        private static void Defence(Effect effect)
        {
            Debug.Log(effect.ToString());
            IAggressiveEntity source = effect.Source as IAggressiveEntity;


            foreach (Entity item in effect.Targets)
            {
                Effect effectCopy = effect.Clone();
                IPassiveEntity target = item as IPassiveEntity;
                target.Data.Trigger(TriggerCondition.Conditions.defense, ref effectCopy);
            }
            //int finalDamage = target.Data.GetFinalDamage(effect.Parameter);
            //effect.Parameter = finalDamage;
            string type = effect["type"];
            if (string.IsNullOrEmpty(type))
            {
                source.Attack(effect);
            }
            else
            {
                foreach (Entity item in effect.Targets)
                {
                    Effect effectCopy = effect.Clone();
                    effectCopy.Target = item;
                    IPassiveEntity target = item as IPassiveEntity;
                    CellEntity centerCell = effect.GetCellParam();

                    switch (type)
                    {
                        case "area":
                            if (target.OnCellOf == centerCell)
                            {
                                effectCopy.Parameter = effectCopy.Parameter.Bounes(-30);
                                goto last;
                            }

                            for (int i = 0; i < source.Data.Properties.AttackArea + 1; i++)
                            {
                                Debug.Log("Level " + (i + 1));
                                List<CellEntity> cellEntities = MapEntity.CurrentMap.GetBorderCell(centerCell, i + 1, ArmyProperty.AttackType.projectile);

                                CellEntity standOn = null;
                                foreach (var cell in cellEntities.Where(cell => target.Position == cell.Position).Select(cell => cell))
                                {
                                    standOn = cell;
                                }

                                if (standOn != null)
                                {
                                    Debug.Log("found target " + target.Name + "at level " + i);
                                    effectCopy.Parameter = effectCopy.Parameter.Bounes((Math.Pow(0.7, i + 2) - 1) * 100);
                                    goto last;
                                }
                                else continue;
                            }
                            break;
                        case "splash":
                            if (target.OnCellOf == centerCell)
                                source.Attack(effectCopy);
                            effectCopy.Parameter /= 2;
                            goto last;
                        case "shielder":
                            if ((target as ArmyEntity)?.data.Type != Army.Types.shielder)
                            {
                                effectCopy.Parameter += target.Data.Defense;
                                effectCopy.Parameter += (int)(target.Data.Health * 0.16f);
                            }
                            goto last;
                    }
                last:
                    source.Attack(effectCopy);
                }
            }

            AfterDefence(effect);
        }

        private static void AfterDefence(Effect effect)
        {
            foreach (Entity entity in effect.AllEntities)
            {
                IBattleableEntity battleEntity = entity.Data as IBattleableEntity;
                if (battleEntity is null)
                {
                    continue;
                }
                battleEntity.Data.Trigger(TriggerCondition.Conditions.afterDefence, ref effect);
            }
            Debug.Log("Attack End");

            IAggressiveEntity agressiveEntity = effect.Source as IAggressiveEntity;
            agressiveEntity.Data.Anger += 40;

            foreach (Entity item in effect.Targets)
            {
                IPassiveEntity passiveEntity = item as IPassiveEntity;
                passiveEntity.Data.Anger += 20;
            }
        }

        #endregion

        #endregion

        #region RelatedStatus

        private static void DamageIncrease(ref Effect sourceEffect, Status status)
        {
            for (int i = 0; i < status.Effect.Count; i++)
            {
                if (status.Type == Effect.Types.damageIncreasePercentage)
                {
                    sourceEffect.Parameter = (int)(sourceEffect.Parameter * (1 + status.Effect.Parameter / 100f));
                }
            }
        }

        private static void DamageDecrease(ref Effect sourceEffect, Status status)
        {
            for (int i = 0; i < status.Effect.Count; i++)
            {
                if (status.Type == Effect.Types.damageDecreasePercentage)
                {
                    sourceEffect.Parameter = (int)(sourceEffect.Parameter / (1 + status.Effect.Parameter / 100f));
                }
            }
        }



        #endregion

        #region Actions
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

            if (battleArmy.Owner.Campus?.GetPointDistance(cellEntity) > 3)
            {
                return false;
            }

            battleArmy.Position = cellEntity.Position;

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
            Debug.Log("Enter Moving");
            return ArmyMovement.PrepareMove(armyEntity);
        }

        private static bool Move(Effect effect)
        {
            ArmyEntity movingArmy = effect.Source as ArmyEntity;
            CellEntity destination = (effect.Target as OnMapEntity)?.OnCellOf;

            Debug.Log(movingArmy);
            Debug.Log(destination);

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

            List<CellEntity> path = PathFinder.GetPath(movingArmy.OnCellOf, destination, movingArmy);

            if (path is null)
            {
                return false;
            }
            else if (path.Count == 0)
            {
                return false;
            }
            else
            {
                movingArmy.data.Trigger(TriggerCondition.Conditions.move, ref effect);
                movingArmy.Move(path);
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

        private static bool Occupy(Effect effect)
        {
            if (effect.Type != Effect.Types.occupy)
            {
                return false;
            }

            foreach (var item in effect.Targets)
            {
                IOwnable ownable = item;
                if (ownable is null)
                {
                    return false;
                }
                if (ownable.Owner == effect.Source.Owner)
                {
                    return false;
                }
            }

            foreach (var item in effect.Targets)
            {
                BuildingEntity buildingEntity = item as BuildingEntity;
                buildingEntity.Owner = effect.Source.Owner;
            }

            return true;
        }



        private static bool RealDamage(Effect effect)
        {
            for (int i = 0; i < effect.Count; i++)
            {
                foreach (Entity item in effect.Targets)
                {
                    IPassiveEntityData passiveEntity = item.Data as IPassiveEntityData;
                    if (passiveEntity is null)
                    {
                        continue;
                    }
                    passiveEntity.RealDamage(effect.Parameter);
                }
            }
            return true;
        }

        private static bool DrawCard(Effect effect)
        {
            if (effect.Type != Effect.Types.drawCard)
            {
                return false;
            }

            for (int i = 0; i < effect.Count; i++)
            {
                Game.CurrentBattle.GetHandCard(effect.Source.Owner, effect.Parameter);
            }
            return true;
        }

        private static bool AddActionPoint(Effect effect)
        {
            if (effect.Type != Effect.Types.addActionPoint)
            {
                return false;
            }

            foreach (var item in effect.Targets)
            {
                for (int i = 0; i < effect.Count; i++)
                {
                    if (item.Data is IActionPointUser)
                    {
                        ((IActionPointUser)item.Data).ActionPoint += effect.Parameter;
                    }
                    else item.Owner.ActionPoint += effect.Parameter;
                }
            }
            return true;
        }

        private static bool AddArmor(Effect effect)
        {
            if (effect.Type != Effect.Types.addArmor)
            {
                return false;
            }

            foreach (var item in effect.Targets)
            {
                if (!(item is IPassiveEntity))
                {
                    return false;
                }
            }

            foreach (var item in effect.Targets)
            {
                IPassiveEntity passiveEntity = item as IPassiveEntity;
                for (int i = 0; i < effect.Count; i++)
                {
                    passiveEntity.Data.Armor += effect.Parameter;
                }
            }
            return true;
        }

        private static bool ArmySwitch(Effect effect)
        {
            ArmyEntity target = effect.Target as ArmyEntity;
            if (!target)
            {
                return false;
            }
            if (effect.Source is ArmyEntity)
            {
                ArmyEntity source = effect.Source as ArmyEntity;

                List<CellEntity> path1 = PathFinder.GetPath(source.OnCellOf, target.OnCellOf, source, false, true);
                List<CellEntity> path2 = PathFinder.GetPath(target.OnCellOf, source.OnCellOf, source, false, true);

                if (path1.Count == 0 || path2.Count == 0)
                {
                    return false;
                }

                ArmyMotion.SetMotion(source, path1);
                ArmyMotion.SetMotion(target, path2);

                Game.CurrentBattle.Round.Normal();
                return true;

            }
            else if (effect.Source is CardEntity)
            {
                if (ArmyEntity.GetArmies(target.OnCellOf, target.data.Properties.MoveRange).Count == 0)
                {
                    return false;
                }

                AddSelectEvent(Switch);
                ArmyMovement.movingArmy = target;
                ArmyMovement.ShowMoveRange();
                return true;
            }
            else return false;

            void Switch(Entity entity)
            {
                if (new Effect(Effect.Types.armySwitch, ArmyMovement.movingArmy, entity, 1, 0).Execute())
                {
                    RemoveSelectEvent(Switch);
                    ArmyMovement.EndShowMoveRange();
                    ArmyMovement.movingArmy = null;
                }
            }
        }

        //private static bool CardMinusPoint(Effect effect)
        //{
        //    if (effect.Target is CardEntity)
        //    {
        //        CardEntity card = effect.Target as CardEntity;
        //        card.data.ActionPoint -= effect.Parameter;
        //        Debug.Log(card.data.ActionPoint);
        //        card.Unselect();
        //        return true;
        //    }
        //    else
        //    {
        //        if (Game.CurrentBattle.Round.CurrentStat == Round.Stat.action)
        //        {
        //            return false;
        //        }
        //        if (effect.Source.Owner.HandCard.Count < 2)
        //        {
        //            return false;
        //        }
        //        DelayTarget = effect.Source;
        //        AddSelectEvent(Minus);
        //        return true;
        //    }

        //    void Minus(Entity entity)
        //    {
        //        if (new Effect(Effect.Types.cardMinusPoint, DelayTarget, entity, LastEffect.Count, LastEffect.Parameter).Execute())
        //        {
        //            RemoveSelectEvent(Minus);
        //        }
        //    }
        //}

        //private static bool LessActionPoint(Effect effect)
        //{
        //    if (effect.Target is PlayerEntity)
        //    {
        //        foreach (var item in effect.Targets)
        //        {
        //            if (!(item is PlayerEntity))
        //            {
        //                return false;
        //            }
        //        }

        //        foreach (var item in effect.Targets)
        //        {
        //            item.Owner.ActionPoint -= effect.Parameter * effect.Count;
        //        }

        //        return true;
        //    }

        //    if (effect.Type != Effect.Types.lessActionPointNextTime)
        //    {
        //        return false;1
        //    }

        //    foreach (var item in effect.Targets)
        //    {
        //        if (!(item is IBattleableEntity))
        //        {
        //            return false;
        //        }
        //    }

        //    foreach (var target in effect.Targets)
        //    {
        //        IStatusContainer statusContainer = target as IStatusContainer;
        //        Status status = effect.ToStatus();
        //        statusContainer.StatList.Add(status);
        //    }


        //    return true;
        //}

        #endregion

        //#region Add Status

        //private static bool AddConfusion(Effect effect)
        //{
        //    if (effect.Type != Effect.Types.confusion)
        //    {
        //        return false;
        //    }

        //    foreach (var item in effect.Targets)
        //    {
        //        if (!(item is IBattleableEntity))
        //        {
        //            return false;
        //        }
        //    }

        //    foreach (var item in effect.Targets)
        //    {
        //        IStatusContainer statusContainer = item as IStatusContainer;
        //        Status status = effect.ToStatus();
        //        statusContainer.Stats.Add(status);
        //    }

        //    return true;
        //}

        //private static bool AddMoveRangeIncrease(Effect effect)
        //{
        //    if (effect.Type != Effect.Types.moveRangeIncrease)
        //    {
        //        return false;
        //    }

        //    foreach (var item in effect.Targets)
        //    {
        //        if (!(item is IBattleableEntity))
        //        {
        //            return false;
        //        }
        //    }

        //    foreach (var target in effect.Targets)
        //    {
        //        IStatusContainer statusContainer = target as IStatusContainer;
        //        Status status = effect.ToStatus();
        //        statusContainer.Stats.Add(status);
        //    }

        //    return true;
        //}

        //private static bool AddAttackRangeIncrease(Effect effect)
        //{
        //    if (effect.Type != Effect.Types.attackRangeIncrease)
        //    {
        //        return false;
        //    }

        //    foreach (var item in effect.Targets)
        //    {
        //        if (!(item is IBattleableEntity))
        //        {
        //            return false;
        //        }
        //    }

        //    foreach (var target in effect.Targets)
        //    {
        //        IStatusContainer statusContainer = target as IStatusContainer;
        //        Status status = effect.ToStatus();
        //        statusContainer.Stats.Add(status);
        //    }

        //    return true;
        //}

        //#endregion

    }

}