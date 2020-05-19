using Canute.Module;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class InfoPanelBuilding : InfoPanel
    {
        [Header("Displaying entity")]
        public BuildingEntity buildingEntity;

        [Header("Progress Bar")]
        public ProgressBar healthBar;
        public Text healthInfo;

        [Header("Other Info")]
        public Text damage;
        public Text defence;

        public override IStatusContainer StatusContainer => buildingEntity.data;

        private void Update()
        {
            if (!buildingEntity)
            {
                return;
            }
            BattleBuilding data = buildingEntity.data;
            healthBar.Progress = data.HealthPercent;
            healthInfo.text = data.Health + "/" + data.MaxHealth;
            defence.text = "Defence: " + data.Defense;
        }
    }
}
