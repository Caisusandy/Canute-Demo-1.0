using System.Collections.Generic;
using UnityEngine;
using Motion = Canute.Module.Motion;

namespace Canute.BattleSystem
{
    public class EntityOnCellMotion : Motion
    {
        public Effect moveEffect;
        public OnMapEntity entity;
        public List<CellEntity> path;

        public override void Arrive()
        {
            entity.OnCellOf.Leave(entity, moveEffect);
            path[0].Enter(entity, moveEffect);

            path.RemoveAt(0);
            if (path.Count > 0)
            {
                SetMotion(entity, path, moveEffect);
                return;
            }
            else
            {
                base.Arrive();
            }
        }

        public static void SetMotion(OnMapEntity entity, List<CellEntity> path, Effect moveEffect)
        {
            EntityOnCellMotion component = entity.gameObject.GetComponent<EntityOnCellMotion>() ? entity.gameObject.GetComponent<EntityOnCellMotion>() : entity.gameObject.AddComponent<EntityOnCellMotion>();
            component.path = path;
            component.moveEffect = moveEffect;
            component.minimumDistance = 0.1f;
            component.entity = entity;
            component.finalPos = path[0].transform.position;
            component.moveSpace = Space.World;
            component.speed = 6f;
        }
    }
}
