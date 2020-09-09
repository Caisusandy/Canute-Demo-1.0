using Canute.Module;
using Canute.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class InfoPanelArmy : InfoPanel
    {
        [Header("Displaying entity")]
        [SerializeField] private ArmyEntity armyEntity;

        public StatusDisplayer skillDisplayer;
        public StatusDisplayer leaderDisplayer;

        [Header("Progress Bar")]
        public ProgressBar healthBar;
        public Text healthInfo;
        public ProgressBar angerBar;
        public Text angerInfo;

        [Header("Other Info")]
        public Text damage;
        public Text defense;
        public AttackPostionIcon attackPostionIcon;
        public StandPostionIcon standPostionIcon;

        public override IStatusContainer StatusContainer => ArmyEntity.data;

        public ArmyEntity ArmyEntity { get => armyEntity; set => armyEntity = value; }

        public void Start()
        {
            if (!armyEntity)
            {
                return;
            }

            if (string.IsNullOrEmpty(ArmyEntity.data.SkillPack.Name))
                Destroy(skillDisplayer.gameObject);
            else
                skillDisplayer.ShowStatus(new Status(ArmyEntity.data.SkillPack));

            //if (string.IsNullOrEmpty(ArmyEntity.data.LocalLeader.Skill))
            Destroy(leaderDisplayer.gameObject);
            //else
            //    leaderDisplayer.ShowStatus(new Status(ArmyEntity.data.SkillPack));

        }

        private void Update()
        {
            if (!armyEntity)
            {
                return;
            }
            BattleArmy data = ArmyEntity.data;
            healthBar.SetProgress((float)ArmyEntity.data.Health / ArmyEntity.data.MaxHealth);
            healthInfo.text = data.Health + (data.Armor == 0 ? "" : ("(+" + data.Armor + ")")) + " / " + data.MaxHealth;

            angerBar.SetProgress(data.Anger / 100f);
            angerInfo.text = data.Anger + " / 100";

            damage.text = "Damage: " + data.RawDamage;
            defense.text = "Defense: " + data.Defense;

            attackPostionIcon.SetArmyItem(ArmyEntity.data);
            standPostionIcon.SetArmyItem(ArmyEntity.data);
        }
    }
}
