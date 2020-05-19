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
            Effect e = new Effect(Effect.Types.effectRelated, this, this, 1, 30, "name:damageIncreasePercentage");
            e.SetSpecialName("damageIncreasePercentage");
            Status status = new Status(e, 2, 0, Status.StatType.turnBase, TriggerCondition.OnAttack);
            StatList.Add(status);
        }
    }
}