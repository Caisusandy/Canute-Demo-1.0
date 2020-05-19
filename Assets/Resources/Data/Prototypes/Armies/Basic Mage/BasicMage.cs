using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
                    if (CanAttack(item.HasArmyStandOn))
                    {
                        targets.Add(item.HasArmyStandOn);
                    }

                }
            }
            Effect attack = new Effect(Effect.Types.attack, this, targets, 1, data.GetDamage(), data.SkillPack.Args);
            attack.Execute();
        }


        public override void AttackExecute(Effect effect)
        {
            IPassiveEntity target = effect.Target as IPassiveEntity;
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

        public override void GetAttackTarget(ref Effect effect)
        {
            Debug.Log("multiple attack");
            List<Entity> passiveEntities = effect.Targets.ShallowClone();

            for (int i = effect.Targets.Count - 1; i > -1; i--)
            {
                CellEntity item = (effect.Targets[i] as OnMapEntity)?.OnCellOf;
                if (item)
                {
                    var data = (effect.Source as ArmyEntity).data;
                    List<ArmyEntity> posibleArmy = (effect.Source as ArmyEntity).GetTargets(item as CellEntity, data.Properties.AttackArea);
                    passiveEntities = passiveEntities.Union(posibleArmy).ToList();
                }
                if (!(item is IPassiveEntity))
                    passiveEntities.Remove(item);
            }

            effect.Targets = passiveEntities;
        }
    }
}