﻿using System.Collections.Generic;

namespace Canute.BattleSystem.Armies
{
    public class NewUtoCityInfantry : BasicInfantry
    {
        public override float AttackAtionDuration => 4f;
        public override float SkillDuration => 4f;
        public override float DefeatedDuration => 2;
        public override float WinningDuration => 2;
    }
}