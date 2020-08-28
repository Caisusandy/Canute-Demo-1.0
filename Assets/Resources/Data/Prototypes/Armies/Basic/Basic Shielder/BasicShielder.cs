using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Canute.BattleSystem.Armies
{
    public class BasicShielder : ArmyEntity
    {
        public MarkController protectCells = new MarkController(CellMark.Type.select);

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
            //add protection effect to the nearby protecting cell
            foreach (var item in GetProtectCell())
            {
                item.StatList.Add(GetProtectionStatus(item));
            }

            //add protection to armies that is already stand there
            foreach (var item in GetProtectCell().Where((CellEntity entity) => entity.HasArmyStandOn))
            {
                Effect protection = new Effect(Effect.Types.tag, this, item.HasArmyStandOn, 1, 0, "name:protection");
                protection.SetSpecialName("protection");
                item.HasArmyStandOn.StatList.Add(new Status(protection, -1, -1, Status.StatType.perminant));

                Effect removeProtection = new Effect(Effect.Types.removeStatus, this, item.HasArmyStandOn, 1);
                removeProtection["uuid"] = protection.UUID.ToString();
                Status removeStat = new Status(removeProtection, 0, 1, Status.StatType.countBase, false, TriggerCondition.OnExitCell);
                item.HasArmyStandOn.StatList.Add(removeStat);
                StatList.Add(removeStat);
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
            effect.Targets = Owner.BattleArmies.Select((BattleArmy entity) => entity.Entity as Entity).ToList();
            base.SkillExecute(effect);
            //new Effect(Effect.Types.addStatus, this, Owner.BattleArmies.Select((BattleArmy entity) => entity.Entity as Entity).ToList(), 2, 1, effect.Args.ToArray()).Execute();
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
            List<CellEntity> cells = GetProtectCell();
            protectCells.Refresh(cells);
            protectCells.Display();
            foreach (var item in cells)
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
            protectCells.ClearDisplay();
            foreach (var item in protectCells.CellMarks.Select(mark => transform.parent.GetComponent<CellEntity>()))
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