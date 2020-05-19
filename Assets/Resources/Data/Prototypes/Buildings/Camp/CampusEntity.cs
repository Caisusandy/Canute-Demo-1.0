using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem.Buildings
{
    public class CampusEntity : BuildingEntity
    {
        public List<CellEntity> Range => Game.CurrentBattle.MapEntity.GetBorderCell(OnCellOf, 3, true);
        public bool IsPlayers => Owner == Game.CurrentBattle.Player;
        public bool BorderOpened { get; set; }

        public override float AttackAtionDuration => 0f;
        public override float SkillDuration => 0f;
        public override float DefeatedDuration => 0f;
        public override float WinningDuration => 0f;
        public override float HurtDuration => 0f;

        public override void SkillExecute(Effect effect)
        {
            return;
        }


        // Start is called before the first frame update
        public override void Start()
        {
            base.Start();
            Owner.Campus = this;

            Debug.LogWarning("draw range");
            DrawRange();
            BorderOpened = true;

        }

        // Update is called once per frame
        public override void Update()
        {
            if (BorderOpened)
            {
                DrawRange();
                TryCloseRange();
            }
        }

        private void DrawRange()
        {
            Mark.Load(Mark.Type.moveRange, Range);
        }

        private void TryCloseRange()
        {
            if (Game.CurrentBattle.CurrentStat != Battle.Stat.begin)
            {
                Debug.LogWarning("unload range to " + Range.Count + " cells");
                Mark.Unload(Mark.Type.moveRange, Range);
                BorderOpened = false;
            }
        }
    }

}