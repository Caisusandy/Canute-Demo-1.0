namespace Canute.BattleSystem.Armies
{
    public class BasicWarMachine : ArmyEntity
    {
        public override float AttackAtionDuration => 4f;
        public override float HurtDuration => 2f;
        public override float SkillDuration => 4f;
        public override float DefeatedDuration => 2;
        public override float WinningDuration => 2;

        public override void SkillExecute(Effect effect)
        {
            var target = ArmyAttack.GetClosestTarget(this);
            if (ArmyAttack.GetPossibleTargets(this).Contains(target))
            {
                effect.Target = target as Entity;
            }
            else
            {
                return;
            }
            effect.Parameter = (int)(data.Damage * 1.2);
            base.SkillExecute(effect);
        }
    }
}