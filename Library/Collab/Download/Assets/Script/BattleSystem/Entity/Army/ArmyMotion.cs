using System.Collections.Generic;
using UnityEngine;
using Motion = Canute.Module.Motion;

namespace Canute.BattleSystem
{
    public class ArmyMotion : Motion
    {
        public ArmyEntity armyEntity;
        public List<CellEntity> path;

        public override void Arrive()
        {
            armyEntity.transform.SetParent(path[0].transform);
            armyEntity.OnCellOf.Enter(armyEntity);

            path.RemoveAt(0);
            if (path.Count > 0)
            {
                SetMotion(armyEntity, path);
                return;
            }
            else
            {
                base.Arrive();
            }
        }

        public static void SetMotion(ArmyEntity armyEntity, List<CellEntity> path)
        {
            ArmyMotion component = armyEntity.gameObject.GetComponent<ArmyMotion>() ? armyEntity.gameObject.GetComponent<ArmyMotion>() : armyEntity.gameObject.AddComponent<ArmyMotion>();
            component.path = path;
            component.minimumDistance = 0.1f;
            component.armyEntity = armyEntity;
            component.finalPos = path[0].transform.position;
            component.moveSpace = Space.World;
            component.speed = 6f;
        }
    }
}
