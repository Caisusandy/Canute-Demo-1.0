using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem.AI
{
    public static partial class AIAction
    {
        public static void ArmyAttack(ArmyEntity entity)
        {
            var targets = entity.GetPossibleTargets();
            if (targets.Count != 0)
            {
                AttackClosest(entity, targets.ToArray());
                return;
            }

            IPassiveEntity closestTarget = entity.GetClosestTarget();
            CellEntity destination = null;
            foreach (var item in entity.data.GetMoveArea())
            {
                if (item.HasArmyStandOn)
                {
                    continue;
                }
                if (!destination)
                {
                    destination = item;
                }
                int d1 = item.GetPointDistanceOf(closestTarget.entity);
                int d = destination.GetPointDistanceOf(closestTarget.entity);

                if (d1 < d && d1 != 0)
                {
                    destination = item;
                }
            }
            if (!destination)
            {
                Debug.Log("no destination to go?");
                return;
            }
            ArmyMovement.SetPath(PathFinder.GetPath(entity.OnCellOf, destination, entity.data.Properties.MoveRange, PathFinder.FinderParam.ignoreBuilding | (entity.data.StandPosition == BattleProperty.Position.land ? PathFinder.FinderParam.ignoreAirArmy : PathFinder.FinderParam.ignoreLandArmy)));
            new Effect(Effect.Types.move, entity, destination, 1, 0).Execute();
        }


        public static void ArmyPatrol(ArmyEntity entity)
        {
            var targets = entity.GetPossibleTargets();
            if (targets.Count != 0)
            {
                AttackClosest(entity, targets.ToArray());
                return;
            }


            Args args = entity.data.RawProperties.Addition;
            var curPos = entity.Coordinate;
            var posID = 0;
            for (int i = 0; i < args.Count; i++)
            {
                Debug.Log(args[i].Value);
                Debug.Log(!args[i].Value.IsVector2());
                if (!args[i].Value.IsVector2())
                {
                    continue;
                }
                if (args[i].Value.ToVector2Int() == curPos)
                {
                    posID = i + 1;
                    break;
                }
            }
            var nextCellCoord = args[posID.ToString()].IsVector2() ? args[posID.ToString()].ToVector2Int() : args["0"].ToVector2Int();
            var nextCell = Game.CurrentBattle.MapEntity[nextCellCoord];
            new Effect(Effect.Types.@event, entity, nextCell, 1, 0, "name:move").Execute();
        }
        private static void AttackClosest(ArmyEntity entity, params IPassiveEntity[] targets)
        {
            IPassiveEntity closestTarget = null;
            foreach (var target in targets)
            {
                if (closestTarget is null)
                {
                    closestTarget = target;
                    continue;
                }
                if (target.GetPointDistanceOf(entity) < closestTarget.GetPointDistanceOf(entity))
                {
                    closestTarget = target;
                }
            }

            new Effect(Effect.Types.attack, entity, closestTarget.entity, 1, entity.data.Damage).Execute();
        }
    }
}