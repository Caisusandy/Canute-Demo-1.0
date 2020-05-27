using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem.Armies
{
    public class BasicDragon : ArmyEntity
    {
        public override void Start()
        {
            Effect effect = new Effect(Effect.Types.effectRelated, this, this, 1, 0, "name:dragonCardAcceptance");
            var status = new Status(effect, -1, -1, Status.StatType.perminant, false);
            StatList.Add(status);
            StatList.Add(GetStatusOnFast());
        }

        public override float AttackAtionDuration => 2;

        public override float SkillDuration => 2;

        public override float DefeatedDuration => 2;

        public override float WinningDuration => 2;

        public override float HurtDuration => 2;



        public Status GetStatusOnFast()
        {
            Effect effect = new Effect(PropertyType.attackRange, BonusType.additive, this, this, 1, 4);
            return new Status(effect, -1, -1, Status.StatType.resonance, true);
        }

        public Status GetStatusOnSlow()
        {
            Effect effect = new Effect(PropertyType.defense, BonusType.percentage, this, this, 1, 800);
            return new Status(effect, -1, -1, Status.StatType.resonance, true);
        }

        public override void SkillExecute(Effect effect)
        {
        }
    }
}