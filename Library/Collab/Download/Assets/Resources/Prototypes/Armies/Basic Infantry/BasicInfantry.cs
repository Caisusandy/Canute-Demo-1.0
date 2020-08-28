using System.Collections.Generic;

namespace Canute.BattleSystem.Armies
{
    public class BasicInfantry : ArmyEntity
    {
        public override float AttackAtionDuration => 4f;
        public override float HurtDuration => 2f;
        public override float SkillDuration => 4f;
        public override float DefeatedDuration => 2;
        public override float WinningDuration => 2;

        public override void SkillExecute(Effect effect)
        {
            Status status = new Status(new Effect(Effect.Types.damageIncreasePercentage, this, this, 1, 30), 2, 0, TriggerCondition.OnAttack);
            StatList.Add(status);
        }
    }
}