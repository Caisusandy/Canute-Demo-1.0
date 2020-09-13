using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem.Buildings
{
    public class CampusEntity : BuildingEntity
    {
        public MarkController markController;
        public List<CellEntity> Range => Game.CurrentBattle.MapEntity.GetBorderCell(OnCellOf, 5, true);
        public List<CellEntity> PossibleCells => Game.CurrentBattle.MapEntity.GetNearbyCell(OnCellOf, 5, true);
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

            Debug.LogWarning("draw range");
            DrawRange();
            BorderOpened = true;

        }

        // Update is called once per frame
        public override void Update()
        {
            if (BorderOpened)
            {
                TryCloseRange();
            }
        }

        private void DrawRange()
        {
            markController = new MarkController(CellMark.Type.moveRange, Range);
        }

        private void TryCloseRange()
        {
            if (Game.CurrentBattle.Round.CurrentStat != Round.Stat.gameStart)
            {
                Debug.LogWarning("unload range to " + Range.Count + " cells");
                markController.ClearDisplay();
                BorderOpened = false;
            }
        }

        public static CampusEntity GetCampus(Player player)
        {
            foreach (var entity in entities)
            {
                if (entity is CampusEntity)
                {
                    if (entity.Owner == player)
                    {
                        return entity as CampusEntity;
                    }
                }
            }
            return null;
        }

    }

}