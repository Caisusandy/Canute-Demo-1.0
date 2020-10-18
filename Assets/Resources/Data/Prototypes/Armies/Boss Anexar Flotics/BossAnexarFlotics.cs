using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem.Armies
{
    public class BossAnexarFlotics : BasicCavalry
    {
        public float time;

        public override float AttackAtionDuration => 2;

        public override float SkillDuration => 2;

        public override float DefeatedDuration => 2;

        public override float WinningDuration => 2;


        public override void Update()
        {
            base.Update();

            time += Time.deltaTime;

            if (time > 1 && Game.CurrentBattle.IsFreeTime && Game.CurrentBattle.CurrentStat != Battle.Stat.begin)
            {
                List<CellEntity> nearByCells = OnCellOf.NearByCells.Where((c) => c.data.canStandOn).ToList();
                CellEntity destination = nearByCells[Random.Range(0, nearByCells.Count)];
                if (destination.HasArmyStandOn && destination.HasArmyStandOn.Exist()?.Owner != Owner)
                {
                    new Effect(Effect.Types.@event, this, destination.HasArmyStandOn, 1, data.Damage, "name:" + EventName.damage).Execute();
                }
                else if (destination)
                {
                    new Effect(Effect.Types.@event, this, destination, 1, 0, "name:" + EventName.move).Execute();
                }
                time = 0;
            }

        }


        public override void SkillExecute(Effect effect)
        {
            effect.Count = 1;
            if (data.HealthPercent > 0.5)
            {
                effect[Effect.name] = EventName.move;
                effect.Target = ArmyAttack.GetClosestTarget(this).OnCellOf;
            }
            else
            {
                effect[Effect.name] = EventName.damage;
                effect.Parameter = data.Damage;
                effect.Target = ArmyAttack.GetClosestTarget(this) as Entity;
            }
            base.SkillExecute(effect);
            return;
        }

    }
}