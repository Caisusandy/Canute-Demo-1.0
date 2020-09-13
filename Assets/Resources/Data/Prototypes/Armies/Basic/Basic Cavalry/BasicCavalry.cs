using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem.Armies
{
    public class BasicCavalry : ArmyEntity
    {
        public override float AttackAtionDuration => 2;

        public override float SkillDuration => 2;

        public override float DefeatedDuration => 2;

        public override float WinningDuration => 2;


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