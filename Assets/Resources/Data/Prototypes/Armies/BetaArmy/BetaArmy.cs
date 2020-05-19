namespace Canute.BattleSystem.Armies
{
    public class BetaArmy : ArmyEntity
    {
        public override float AttackAtionDuration => 4f;
        public override float HurtDuration => 2f;
        public override float SkillDuration => 4f;
        public override float DefeatedDuration => 0;
        public override float WinningDuration => 0;

        public override void SkillExecute(Effect effect)
        {
            throw new System.NotImplementedException();
        }
    }
}