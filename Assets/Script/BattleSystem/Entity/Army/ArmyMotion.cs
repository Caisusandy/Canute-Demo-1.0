using System.Collections.Generic;
using UnityEngine;
using Motion = Canute.Module.Motion;

namespace Canute.BattleSystem
{
    public class ArmyMotion : Motion
    {
        public Effect moveEffect;
        public ArmyEntity armyEntity;
        public List<CellEntity> path;

        public override void Arrive()
        {
            armyEntity.OnCellOf.Leave(armyEntity, moveEffect);
            path[0].Enter(armyEntity, moveEffect);

            path.RemoveAt(0);
            if (path.Count > 0)
            {
                SetMotion(armyEntity, path, moveEffect);
                return;
            }
            else
            {
                base.Arrive();
            }
        }

        public static void SetMotion(ArmyEntity armyEntity, List<CellEntity> path, Effect moveEffect)
        {
            ArmyMotion component = armyEntity.gameObject.GetComponent<ArmyMotion>() ? armyEntity.gameObject.GetComponent<ArmyMotion>() : armyEntity.gameObject.AddComponent<ArmyMotion>();
            component.path = path;
            component.moveEffect = moveEffect;
            component.minimumDistance = 0.1f;
            component.armyEntity = armyEntity;
            component.finalPos = path[0].transform.position;
            component.moveSpace = Space.World;
            component.speed = 6f;
        }
    }
}
