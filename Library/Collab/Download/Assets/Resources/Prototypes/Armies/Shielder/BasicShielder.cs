using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem.Armies
{
    public class BasicShielder : ArmyEntity
    {
        public override float AttackAtionDuration => 2;

        public override float SkillDuration => 2;

        public override float DefeatedDuration => 2;

        public override float WinningDuration => 2;

        public override float HurtDuration => 2;

        public override void SkillExecute(Effect effect)
        {
            new Effect(Effect.Types.addArmor, this, this, 2, 1).Execute();
        }


        public override void AttackExecute(Effect effect)
        {
            foreach (var item in effect.Targets)
            {
                Effect effectCopy = effect.Clone();
                IPassiveEntity target = item as IPassiveEntity;

                if ((target as ArmyEntity)?.data.Type != Army.Types.shielder)
                {
                    Debug.Log(effectCopy.Parameter);
                    effectCopy.Parameter += target.Data.Defense;
                    Debug.Log(effectCopy.Parameter);
                    effectCopy.Parameter += (int)(target.Data.Health * 0.1f);
                    Debug.Log(effectCopy.Parameter);
                }

                for (int i = 0; i < effectCopy.Count; i++)
                {
                    AttackAction(target, effectCopy.Parameter);
                }
            }
        }
    }
}