using Canute.Module;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class InfoPanelArmy : InfoPanel
    {
        [Header("Displaying entity")]
        public ArmyEntity armyEntity;

        [Header("Progress Bar")]
        public ProgressBar healthBar;
        public Text healthInfo;
        public ProgressBar angerBar;
        public Text angerInfo;

        [Header("Other Info")]
        public Text damage;
        public Text defence;

        public override IStatusContainer StatusContainer => armyEntity.data;

        private void Update()
        {
            if (!armyEntity)
            {
                return;
            }
            BattleArmy data = armyEntity.data;
            healthBar.Progress = data.HealthPercent;
            healthInfo.text = data.Health + (data.Armor == 0 ? "" : ("(+" + data.Armor + ")")) + " / " + data.MaxHealth;
            angerBar.Progress = data.Anger / 100f;
            angerInfo.text = data.Anger + " / 100";
            damage.text = "Damage: " + data.RawDamage;
            defence.text = "Defense: " + data.Defense;
        }
    }
}
