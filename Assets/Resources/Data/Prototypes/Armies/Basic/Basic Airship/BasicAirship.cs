using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem.Armies
{
    public class BasicAirship : ArmyEntity
    {
        public override float AttackAtionDuration => 2;

        public override float SkillDuration => 2;

        public override float DefeatedDuration => 2;

        public override float WinningDuration => 2;

        public override float HurtDuration => 2;

        public override void Start()
        {
            base.Start();
        }

        public override void SkillExecute(Effect effect)
        {
            return;
        }

        public override void Move(params object[] vs)
        {
            Effect effect = vs[1] as Effect;
            base.Move(vs);

            var childsRecord = StatList.GetAllStatus(Effect.Types.tag, "name:motherOf");
            //var childs = childsRecord.Select((Status stat) => { return stat.Effect.Target as ArmyEntity; }).ToList();

            for (int i = childsRecord.Count - 1; i > -1; i--)
            {
                var child = childsRecord[i].Effect.Target as ArmyEntity;
                var status = child.StatList.GetStatus(Effect.Types.@event, "name:aircraftFighterReturn");
                if (status is null)
                {
                    Debug.LogError("child airport record not find");
                }
                Debug.Log("child airport record find");
                var d = child.HexCoord - HexCoord;
                CellEntity cellEntity = Game.CurrentBattle.MapEntity[(effect.Target as CellEntity).HexCoord + d];
                Debug.Log(status.Effect.GetCellParam().HexCoord + d);
                Debug.Log(cellEntity);
                if (cellEntity)
                {
                    if (cellEntity.IsValidDestination(child))
                    {
                        status.Effect.SetCellParam(cellEntity);
                    }
                    else
                    {
                        StatList.Remove(childsRecord[i]);
                        child.KillEntity();
                    }
                }
            }
        }

        public override void GetAttackTarget(ref Effect effect)
        {
            var targets = effect.Targets;

            if (targets.Where((Entity e) => e is IPassiveEntity).Count() == targets.Count)
            {
                return;
            }
            if (targets.Where((Entity e) => e is CellEntity).Count() != targets.Count)
            {
                return;
            }
            if (targets.Where((Entity e) => !(e as CellEntity).HasArmyStandOn).Count() != targets.Count)
            {
                return;
            }
            if (targets.Count != 1)
            {
                return;
            }
            /* 
             *  CellParam ["armyName"]["type"]["career"]["attackPosition"]["attackType"]["prefabPath"]
             *  ["attack"]["health"]["defense"]["critRate"]["critBonus"]["moveRange"]["moveRange"] 
             */
            effect.Type = Effect.Types.@event;
            effect.Parameter = 1;
            effect.SetCellParam(effect.Target as CellEntity);
            effect[Effect.name] = "airship" + EventName.createArmy;
            effect["prototype"] = "Basic AirshipAircraftFighter";
            effect["level"] = "60";
            effect["star"] = "1";
            //effect["armyName"] = "carrierAircraftFighter";
            //effect["armyType"] = Army.Types.aircraftFighter.ToString();
            //effect["career"] = data.Career.ToString();
            //effect["attackPosition"] = BattleProperty.Position.all.ToString();
            //effect["standPosition"] = BattleProperty.Position.air.ToString();
            //effect["attackType"] = BattleProperty.AttackType.areaProjectile.ToString();
            //effect["prefabPath"] = @"Data\Prototypes\Armies\Basic AirshipAircraftFighter";

            //effect["health"] = data.MaxHealth.Bonus(-0.83).ToString();
            //effect["damage"] = data.RawDamage.Bonus(-0.4).ToString();
            //effect["attackRange"] = "2";
            //effect["moveRange"] = "4";

        }
        public override void AttackExecute(Effect effect)
        {
            IPassiveEntity target = effect.Target as IPassiveEntity;
            CellEntity centerCell = effect.GetCellParam();
            Debug.Log(centerCell);
            if (target.OnCellOf == centerCell)
                for (int i = 0; i < effect.Count; i++)
                {
                    AttackAction(target, effect.Parameter);
                }

            effect.Parameter /= 2;
            for (int i = 0; i < effect.Count; i++)
            {
                AttackAction(target, effect.Parameter);
            }

        }
    }
}