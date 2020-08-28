using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem.Armies
{
    public class BasicMage : ArmyEntity
    {
        public override float AttackAtionDuration => 2;

        public override float SkillDuration => 2;

        public override float DefeatedDuration => 2;

        public override float WinningDuration => 2;

        public override float HurtDuration => 2;

        public override void SkillExecute(Effect effect)
        {
            List<Entity> targets = new List<Entity>();
            int y = this.y;

            foreach (var item in Game.CurrentBattle.MapEntity[y])
            {
                if (item.HasArmyStandOn)
                {
                    foreach (var army in item.Armies)
                    {
                        if (CanAttack(army))
                        {
                            targets.Add(army);
                        }
                    }
                }
            }
            Effect attack = new Effect(Effect.Types.attack, this, targets, 1, data.GetDamage(), data.SkillPack.Args);
            attack.Execute();
        }


        public override void AttackExecute(Effect effect)
        {
            foreach (var item in effect.Targets)
            {
                IPassiveEntity target = item as IPassiveEntity;
                CellEntity centerCell = effect.GetCellParam();
                Debug.Log(centerCell);
                if (target.OnCellOf == centerCell)
                    for (int i = 0; i < effect.Count; i++)
                    {
                        AttackAction(target, effect.Parameter);
                    }

                effect.Parameter /= 2;
                for (int i = 0; i < effect.Count; i++)
                {
                    AttackAction(target, effect.Parameter);
                }
            }
        }
    }
}