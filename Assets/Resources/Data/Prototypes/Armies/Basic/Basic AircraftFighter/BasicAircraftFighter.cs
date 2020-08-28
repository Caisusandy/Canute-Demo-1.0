using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem.Armies
{
    public class BasicAircraftFighter : ArmyEntity
    {
        public override float AttackAtionDuration => 2;

        public override float SkillDuration => 2;

        public override float DefeatedDuration => 2;

        public override float WinningDuration => 2;

        public override float HurtDuration => 2;

        public override void Start()
        {
            base.Start();

            var RecorderEffect = new Effect(Effect.Types.effectRelated, this, this, 1, 0, "name:aircraftFighterReturnPositionRecorder");
            var positionRecorder = new Status(RecorderEffect, -1, -1, Status.StatType.perminant, TriggerCondition.OnMove, false);
            StatList.Add(positionRecorder);

            var maximizeAttackArea = new Effect(Effect.Types.addStatus, this, this, 1, 0, "name:areaBuffer", "effectType:tag", "tc:1", "sc:1", "statType:dualBase", "showToPlayer:false");
            var areaBuffer = new Status(maximizeAttackArea, -1, -1, Status.StatType.resonance, TriggerCondition.OnTurnBegin, false);
            StatList.Add(areaBuffer);

            var towardEffect = new Effect(Effect.Types.effectRelated, this, this, 1, 0, "name:aircreaftFighterTowardEnemy");
            var towardStatus = new Status(towardEffect, -1, -1, Status.StatType.perminant, TriggerCondition.OnDefenseEnd, true);
            StatList.Add(towardStatus);

            var moveEffect = new Effect(Effect.Types.@event, this, this, 1, 0, "name:aircraftFighterReturn");
            var stat = new Status(moveEffect, -1, -1, Status.StatType.perminant, TriggerCondition.OnTurnEnd, true);
            stat.Effect.SetCellParam(OnCellOf);
            StatList.Add(stat);
        }

        public override void SkillExecute(Effect effect)
        {
            return;
        }

        public override List<CellEntity> GetAttackArea()
        {
            var cellEntities = base.GetAttackArea();
            foreach (var item in GetMoveArea())
            {
                if (item.HasArmyStandOn)
                {
                    if (item.HasArmyStandOn.Owner == Owner && item.HasArmyStandOn != this)
                    {
                        cellEntities = cellEntities.Union(item.HasArmyStandOn.data.GetAttackArea()).ToList();
                    }
                }
            }
            return cellEntities;
        }
    }
}