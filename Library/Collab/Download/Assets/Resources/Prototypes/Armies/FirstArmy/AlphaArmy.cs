namespace Canute.BattleSystem.Armies
{
    public class AlphaArmy : ArmyEntity
    {
        public override float AttackAtionDuration => 4f;
        public override float HurtDuration => 2f;
        public override float SkillDuration => 4f;
        public override float DefeatedDuration => 2;
        public override float WinningDuration => 2;

        public override void SkillExecute(Effect effect)
        {
            if (effect.Type == Effect.Types.skill && effect.Name == data.SkillPack.Name)
            {
                StatList.Add(new Status(new Effect(Effect.Types.damageIncreasePercentage, 1, 30), 3, 0, TriggerCondition.OnBeforeAttack));
            }
        }
    }
}