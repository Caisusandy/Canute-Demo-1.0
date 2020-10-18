using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem.Armies
{
    public class Amlos : BasicDragon
    {
        public override void Start()
        {
            base.Start();
            StatList.Add(GetDragonCardAcceptance());
            StatList.Add(GetStatusOnFast());
        }

        private Status GetDragonCardAcceptance()
        {
            Effect effect = new Effect(Effect.Types.effectRelated, this, this, 1, 0, "name:dragonCardAcceptance");
            var status = new Status(effect, -1, -1, Status.StatType.resonance, false, TriggerCondition.OnAddingStatus);
            return status;
        }

        public override float AttackAtionDuration => 2;

        public override float SkillDuration => 2;

        public override float DefeatedDuration => 2;

        public override float WinningDuration => 2;




        protected Status GetStatusOnFast()
        {
            Effect attack = new Effect(PropertyType.attackRange, BonusType.additive, this, this, 1, 2, "name:dragonBonus", "dragonStatus:attack");
            attack[Effect.propertyType] = "dragonAttack";
            attack.SetSpecialName("dragonAttack");
            return new Status(attack, -1, -1, Status.StatType.resonance, true);
        }


        protected Status GetStatusOnSlow()
        {
            Effect defense = new Effect(PropertyType.defense, BonusType.percentage, this, this, 1, 800, "name:dragonBonus", "dragonStatus:defense");
            defense[Effect.propertyType] = "dragonDefense";
            defense.SetSpecialName("dragonDefense");
            return new Status(defense, -1, -1, Status.StatType.resonance, true);
        }

        public override void SkillExecute(Effect effect)
        {
            var cards = Game.CurrentBattle.GetHandCard(Owner, 3);
            foreach (var item in cards)
            {
                item.data.ActionPoint = 0;
            }
        }
    }
}