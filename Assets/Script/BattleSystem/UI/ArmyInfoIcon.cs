using Canute.Module;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    /// <summary>
    /// 军队在UI的界面的显现控件
    /// </summary>
    public class ArmyInfoIcon : MonoBehaviour
    {
        public UUID connectedArmyEntityUUID = UUID.Empty;

        public ProgressBar health;
        public ProgressBar anger;

        public Text healthInfo;
        public Text angerInfo;
        public Image icon;

        public ArmyEntity ArmyEntity => Entity.Get<ArmyEntity>(connectedArmyEntityUUID);

        private void Awake()
        {
            icon = GetComponent<Image>();
        }

        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
            if (ArmyEntity)
            {
                UpdateInfo();
            }
        }

        public void UpdateInfo()
        {
            healthInfo.text = ArmyEntity.data.Health + "/" + ArmyEntity.data.MaxHealth;
            health.SetProgress((float)ArmyEntity.data.Health / ArmyEntity.data.MaxHealth);
            angerInfo.text = ArmyEntity.data.Anger + "/100";
            anger.SetProgress(ArmyEntity.data.Anger / 100f);
            icon.sprite = ArmyEntity.data.Icon;
        }


        public void Connect(ArmyEntity armyEntity)
        {
            Debug.Log(name + " connected to army " + ArmyEntity?.Name);
            //Debug.Log(armyEntity.UUID);
            //Debug.Log(connectedArmyEntityUUID);
            connectedArmyEntityUUID = armyEntity.UUID;
        }

        public void Goto()
        {
            if (!ArmyEntity)
            {
                return;
            }
            ArmyEntity.OnMouseDown();
            ArmyEntity.OnMouseUp();
        }
    }
}