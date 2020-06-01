using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Canute.BattleSystem.Armies
{
    public class BasicShielder : ArmyEntity
    {
        public List<CellEntity> protectCells;

        public override float AttackAtionDuration => 2;

        public override float SkillDuration => 2;

        public override float DefeatedDuration => 2;

        public override float WinningDuration => 2;

        public override float HurtDuration => 2;

        public override void Start()
        {
            base.Start();
            SetProtection();
        }

        public override void Move(params object[] vs)
        {
            RemoveProtection();
            base.Move(vs);
        }

        protected override void Idle(params object[] vs)
        {
            base.Idle(vs);
            SetProtection();
        }

        protected void SetProtection()
        {
            foreach (var item in GetProtectCell())
            {
                item.StatList.Add(GetProtectionStatus(item));
            }
        }

        protected void RemoveProtection()
        {
            foreach (var (item, stat) in from item in GetProtectCell()
                                         from stat in item.StatList.GetAllStatus(Effect.Types.tag, "name:protection")
                                         where stat.Effect.Source == this
                                         select (item, stat))
            {
                item.StatList.Remove(stat);
            }
        }

        public virtual List<CellEntity> GetProtectCell()
        {
            List<string> vs = data.Properties.Addition.FindAll("ProtectCell");
            List<CellEntity> cellEntities = new List<CellEntity>();
            foreach (var item in vs)
            {
                Vector3 vector = item.ToVector3();
                CellEntity item1 = Game.CurrentBattle.MapEntity.GetCell(HexCoord.x + (int)vector.x, HexCoord.y + (int)vector.y, HexCoord.z + (int)vector.z);
                if (item1)
                {
                    cellEntities.Add(item1);
                }
            }
            return cellEntities;
        }

        public override void SkillExecute(Effect effect)
        {
            new Effect(Effect.Types.@event, this, this, 2, 1, "name:addAmor").Execute(false);
        }

        public override void AttackExecute(Effect effect)
        {
            foreach (var item in effect.Targets)
            {
                Effect effectCopy = effect.Clone();
                IPassiveEntity target = item as IPassiveEntity;

                if ((target as ArmyEntity)?.data.Type != Army.Types.shielder)
                {
                    effectCopy.Parameter += target.Data.Defense;
                    effectCopy.Parameter += (int)(target.Data.Health * 0.16f);
                }

                for (int i = 0; i < effectCopy.Count; i++)
                {
                    AttackAction(target, effectCopy.Parameter);
                }
            }
        }

        public override void Highlight()
        {
            base.Highlight();
            protectCells = GetProtectCell();
            Mark.Load(Mark.Type.select, protectCells);
            foreach (var item in protectCells)
            {
                if (item.HasArmyStandOn)
                {
                    item.HasArmyStandOn.transform.localScale *= 1.2f;
                }
            }
        }

        public override void Unhighlight()
        {
            base.Unhighlight();
            Mark.Unload(Mark.Type.select, protectCells);
            foreach (var item in protectCells)
            {
                if (item.HasArmyStandOn)
                {
                    item.HasArmyStandOn.transform.localScale = Vector3.one;
                }
            }
        }

        protected Status GetProtectionStatus(CellEntity cellEntity)
        {
            Effect protection = new Effect(Effect.Types.@event, this, cellEntity, 1, 0, "name:protection");
            return new Status(protection, -1, -1, Status.StatType.perminant, TriggerCondition.OnEnterCell);
        }
    }
}