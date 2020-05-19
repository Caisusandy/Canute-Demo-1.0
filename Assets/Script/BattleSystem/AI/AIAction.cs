using System.Collections.Generic;
using System.Linq;

namespace Canute.BattleSystem.AI
{
    public static partial class AIAction
    {
        public static KeyValuePair<CellEntity, ArmyEntity> GetBestDestinationAndTarget(ArmyEntity armyEntity)
        {
            CellEntity bestDestination = null;
            ArmyEntity target = null;
            List<CellEntity> moveArea = armyEntity.GetAttackArea();
            List<ArmyEntity> possibleTargets = armyEntity.GetTargetAfterMove();

            if (possibleTargets.Count == 0)
            {
                return default;
            }

            //try to find a destination that enemy cannnot attack but army can attack enemy
            foreach (var possibleTarget in possibleTargets)
            {
                if (possibleTarget.data.Properties.AttackRange > armyEntity.data.Properties.MoveRange)
                {
                    continue;
                }

                target = possibleTarget;
                IEnumerable<CellEntity> areaCannotBeAttacked = moveArea.Concat(possibleTarget.GetAttackRange());
                CellEntity cellEntity = areaCannotBeAttacked.First();
                foreach (var item in areaCannotBeAttacked)
                {
                    if (item.GetRealDistanceOf(possibleTarget, armyEntity) < cellEntity.GetRealDistanceOf(possibleTarget, armyEntity))
                    {
                        cellEntity = item;
                    }
                }
                bestDestination = cellEntity;
            }
            if (bestDestination)
            {
                return new KeyValuePair<CellEntity, ArmyEntity>(bestDestination, target);
            }
            //if there is not
            target = possibleTargets[0];
            foreach (var possibleTarget in possibleTargets)
            {
                if (possibleTarget.data.RawDamage < target.data.RawDamage)
                {
                    target = possibleTarget;
                }
            }
            bestDestination = moveArea[0];
            foreach (var item in moveArea)
            {
                if (item.GetRealDistanceOf(target, armyEntity) < bestDestination.GetRealDistanceOf(target, armyEntity))
                {
                    bestDestination = item;
                }
            }
            return new KeyValuePair<CellEntity, ArmyEntity>(bestDestination, target);
        }

        /// <summary>
        /// Get the best target (determine by HP)
        /// </summary>
        /// <param name="armyEntity"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public static ArmyEntity BestTarget(ArmyEntity armyEntity, CellEntity origin = null)
        {
            if (origin)
            {
                origin = armyEntity.OnCellOf;
            }
            List<ArmyEntity> list = armyEntity.GetTargets(origin, armyEntity.data.Properties.AttackRange);
            if (list.Count == 1)
            {
                return list[0];
            }
            ArmyEntity target = list[0];
            for (int i = 1; i < list.Count; i++)
            {
                ArmyEntity item = list[i];
                if (item.data.Health < target.data.Health)
                {
                    target = item;
                }
            }
            return target;
        }

    }
}