using System.Collections.Generic;

namespace Canute.BattleSystem.Armies
{
    public class BasicInfantry : ArmyEntity
    {
        public override float AttackAtionDuration => 4f;
        public override float SkillDuration => 4f;
        public override float DefeatedDuration => 2;
        public override float WinningDuration => 2;

        //public override void SkillExecute(Effect effect)
        //{
        //    Effect e = new Effect(Effect.Types.effectRelated, this, this, 1, 30, "name:damageIncreasePercentage");
        //    e.SetSpecialName("damageIncreasePercentage");
        //    Status status = new Status(e, 2, 0, Status.StatType.turnBase, TriggerCondition.OnAttack);
        //    StatList.Add(status);
        //}        
        public override void SkillExecute(Effect effect)
        {
            var target = ArmyAttack.GetLowestHealthPointTarget(this);
            if (!(target is Entity)) return;
            effect.Target = target as Entity;

            int damage = data.Damage;
            effect.Parameter = damage;
            base.SkillExecute(effect);
            effect.Parameter = damage.Bonus(-40);
            base.SkillExecute(effect);
        }
    }
}