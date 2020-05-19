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

        public override float HurtDuration => 2;

        public override void SkillExecute(Effect effect)
        {
            return;
        }

    }
}