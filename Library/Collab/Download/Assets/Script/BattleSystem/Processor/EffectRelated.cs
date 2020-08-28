using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.BattleSystem
{
    public static partial class EffectExecute
    {
        #region RelatedStatus

        /// <summary>
        /// Status Executor, only those related to another effect
        /// <para>these status will not result failure</para>
        /// </summary>
        /// <param name="status"></param>
        /// <param name="sourceEffect"></param>
        public static void Execute(this Status status, ref Effect sourceEffect)
        {
            if (status.Effect.Type == Effect.Types.effectRelated || status.Effect.Type == Effect.Types.propertyBonus || status.Effect.Type == Effect.Types.propertyPanalty)
            {
                StatusExecutor statusExecutor;
                switch (status.Effect[Effect.name])
                {
                    case "damageIncreasePercentage":
                        statusExecutor = DamageIncrease;
                        break;
                    case "damageDecreasePercentage":
                        statusExecutor = DamageDecrease;
                        break;

                    //special
                    case "aircraftFighterReturnPositionRecorder":
                        statusExecutor = FighterAircraftChangeReturnPos;
                        break;
                    case "aircreaftFighterTowardEnemy":
                        statusExecutor = AircreaftFighterTowardEnemy;
                        break;
                    case "dragonCardAcceptance":
                        statusExecutor = DragonCardAcceptance;
                        break;
                    case "dragonAirAttack":
                        statusExecutor = DragonAirAttackBonus;
                        break;
                    case "dragonDualAttack":
                        statusExecutor = DragonDualAttack;
                        break;
                    case "dragonStrike":
                        statusExecutor = DragonStrike;
                        break;
                    case "dragonCritAttack":
                        statusExecutor = DragonCritAttackBonus;
                        break;
                    case "MoveChildren":
                        statusExecutor = MoveChildren;
                        break;
                    default:
                        status.Execute();
                        return;
                }
                statusExecutor(ref sourceEffect, status);
            }
            else
            {
                status.Execute();
                return;
            }
            AfterExecute(status);
        }

        public static bool Execute(this Status status)
        {
            bool success = status.Effect.Execute(false);

            if (success)
            {
                AfterExecute(status);
            }

            return success;
        }

        private static void AfterExecute(Status status)
        {
            if (status.IsBaseOnCount || status.IsDualBase)
            {
                status.StatCount--;
            }
        }


        private static void DamageIncrease(ref Effect sourceEffect, Status status)
        {
            if (sourceEffect.Type != Effect.Types.attack)
            {
                return;
            }

            for (int i = 0; i < status.Effect.Count; i++)
            {
                sourceEffect.Parameter = (int)(sourceEffect.Parameter * (1 + status.Effect.Parameter / 100f));
            }
        }

        private static void DamageDecrease(ref Effect sourceEffect, Status status)
        {
            if (sourceEffect.Type != Effect.Types.attack)
            {
                return;
            }

            for (int i = 0; i < status.Effect.Count; i++)
            {
                sourceEffect.Parameter = (int)(sourceEffect.Parameter / (1 + status.Effect.Parameter / 100f));
            }
        }

        private static void FighterAircraftChangeReturnPos(ref Effect sourceEffect, Status status)
        {
            ArmyEntity armyEntity = status.Effect.Source as ArmyEntity;

            if (!armyEntity)
            {
                return;
            }

            if (armyEntity.data.Type != Army.Types.aircraftFighter)
            {
                return;
            }

            var stat = armyEntity.StatList.GetEvent("name:aircraftFighterReturn");
            if (stat is null)
            {
                Effect moveEffect = new Effect(Effect.Types.@event, armyEntity, armyEntity, 1, 0, "name:aircraftFighterReturn");
                stat = new Status(moveEffect, -1, -1, Status.StatType.perminant, TriggerCondition.OnTurnEnd, false);
                armyEntity.StatList.Add(stat);
            }
            else
            {
                stat.Effect.GetCellParam().data.canStandOn = true;
            }
            stat.Effect.SetCellParam(sourceEffect.Target as CellEntity);
            stat.Effect.GetCellParam().data.canStandOn = false;
            Debug.Log("record original pos");

            return;
        }

        private static void AircreaftFighterTowardEnemy(ref Effect sourceEffect, Status status)
        {
            Entity source = status.Effect.Target;
            Entity target = sourceEffect.Target;
            CellEntity destination = null;
            destination = GetCloseDestination(source, target);

            Effect moveToward = new Effect(Effect.Types.@event, source, destination, 1, 0, "name:move");
            var s = moveToward.Execute(false);
            if (!s) Debug.Log("failed to move");

            CellEntity GetCloseDestination(Entity start, Entity end)
            {
                CellEntity pDestination = null;
                int minDist = 1000;
                foreach (var item in (end as OnMapEntity).OnCellOf.NearByCells)
                {
                    if (!pDestination)
                    {
                        pDestination = item;
                    }

                    if (item.HasArmyStandOn)
                    {
                        continue;
                    }

                    int newDist = (start as OnMapEntity).GetRealDistanceOf(item, start as OnMapEntity);
                    Debug.Log(newDist);
                    if (newDist < minDist && newDist != 0)
                    {
                        pDestination = item;
                        minDist = newDist;
                    }
                }

                return pDestination;
            }
        }

        private static void DragonCardAcceptance(ref Effect sourceEffect, Status status)
        {
            if (!status.Effect.Args.HasParam("isEffectFromDragon"))
            {
                return;
            }

            SwitchDragonStatus(sourceEffect);
            sourceEffect = new Effect(Effect.Types.none, 0, 0);

        }

        private static void DragonAirAttackBonus(ref Effect sourceEffect, Status status)
        {
            if (sourceEffect.Type != Effect.Types.attack)
            {
                return;
            }

            for (int i = 0; i < status.Effect.Count; i++)
            {
                sourceEffect.Parameter = sourceEffect.Parameter.Bonus(10);
            }
        }

        private static void DragonDualAttack(ref Effect sourceEffect, Status status)
        {
            Debug.Log("Dual attack");
            sourceEffect.Count *= 2;
            sourceEffect.Parameter = (int)(sourceEffect.Parameter * 0.45f);
        }

        private static void DragonCritAttackBonus(ref Effect sourceEffect, Status status)
        {
            if (sourceEffect.Type != Effect.Types.attack)
            {
                return;
            }

            sourceEffect.Parameter = sourceEffect.Parameter.Bonus(20);
        }

        private static void DragonStrike(ref Effect sourceEffect, Status status)
        {
            Debug.Log("strike start");
            if (sourceEffect.Type != Effect.Types.move)
            {
                Debug.LogError("it is not moving");
                return;
            }

            IBattleableEntity battleableEntity = sourceEffect.Source as IBattleableEntity;
            if (battleableEntity is null)
            {
                Debug.LogError("source is not battleable entity");
                return;
            }

            if (sourceEffect.Target != battleableEntity.OnCellOf)
            {
                Debug.LogWarning("not arrive yet");
                return;
            }

            foreach (var cell in battleableEntity.OnCellOf.NearByCells)
            {
                if (cell.HasArmyStandOn)
                {
                    if (cell.HasArmyStandOn.Owner != battleableEntity.Owner)
                    {
                        Vector3Int d = cell.HasArmyStandOn.HexCoord - battleableEntity.HexCoord;
                        if (Game.CurrentBattle.MapEntity[cell.HexCoord + d].Exist())
                        {
                            if (!Game.CurrentBattle.MapEntity[cell.HexCoord + d].HasArmyStandOn)
                            {
                                if (Game.CurrentBattle.MapEntity[cell.HexCoord + 2 * d].Exist())
                                {
                                    if (!Game.CurrentBattle.MapEntity[cell.HexCoord + 2 * d].HasArmyStandOn)
                                    {
                                        Effect move = new Effect(Effect.Types.@event, cell.HasArmyStandOn, Game.CurrentBattle.MapEntity[cell.HexCoord + 2 * d], 1, 0, "name:move");
                                        move.Execute();
                                    }
                                    else
                                    {
                                        Effect move = new Effect(Effect.Types.@event, cell.HasArmyStandOn, Game.CurrentBattle.MapEntity[cell.HexCoord + 1 * d], 1, 0, "name:move");
                                        move.Execute();
                                    }
                                }
                                else Debug.Log("2nd cell not exist");
                            }
                            else Debug.Log("some army is stand on 1st cell" + Game.CurrentBattle.MapEntity[cell.HexCoord + d].HasArmyStandOn);
                        }
                        else Debug.Log("1st cell not exist");
                    }
                    else Debug.Log("it is your own army");
                }
                else Debug.Log("No army at " + cell);
            }
        }

        private static void MoveChildren(ref Effect sourceEffect, Status status)
        {
            ArmyEntity mother = sourceEffect.Target as ArmyEntity;
            List<Status> childRecord = mother.StatList.GetAllTags();

            foreach (var item in childRecord)
            {
                if (item.Effect[Effect.name] != "motherOf")
                {
                    continue;
                }

                ArmyEntity child = item.Effect.Source as ArmyEntity;
                Vector3Int d = child.HexCoord - mother.HexCoord;
                CellEntity destination = Game.CurrentBattle.MapEntity[(status.Effect.Target as OnMapEntity).HexCoord + d];

                if (!destination)
                {
                    child.KillEntity();
                }
                else if (!destination.IsValidDestination(child as OnMapEntity))
                {
                    child.KillEntity();
                }

                var move = new Effect(Effect.Types.@event, child, destination, 1, 0, "name:move");
                move.Execute();

                var curStat = child.StatList.GetEvent("name:aircraftFighterReturn");
                curStat.Effect.Target = destination;
            }
        }

        #endregion

    }
}
