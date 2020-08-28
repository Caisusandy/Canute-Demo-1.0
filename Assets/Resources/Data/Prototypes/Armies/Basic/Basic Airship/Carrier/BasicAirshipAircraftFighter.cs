using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem.Armies
{
    public class BasicAirshipAircraftFighter : ArmyEntity
    {
        public override float AttackAtionDuration => 2;

        public override float SkillDuration => 2;

        public override float DefeatedDuration => 2;

        public override float WinningDuration => 2;

        public override float HurtDuration => 2;

        public override void Start()
        {
            base.Start();

            var moveEffect = new Effect(Effect.Types.@event, this, OnCellOf, 1, 0, "name:aircraftFighterReturn");
            var stat = new Status(moveEffect, -1, -1, Status.StatType.resonance, TriggerCondition.OnTurnEnd);
            moveEffect.SetCellParam(OnCellOf);
            StatList.Add(stat);

            Effect towardEffect = new Effect(Effect.Types.effectRelated, this, this, 1, 0, "name:aircreaftFighterTowardEnemy");
            Status towardStatus = new Status(towardEffect, -1, -1, Status.StatType.resonance, TriggerCondition.OnDefenseEnd);
            StatList.Add(towardStatus);
        }

        public override void SkillExecute(Effect effect)
        {
            return;
        }
    }
}