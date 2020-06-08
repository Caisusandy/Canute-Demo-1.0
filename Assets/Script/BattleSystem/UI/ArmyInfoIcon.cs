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
        public bool IsValidArmyInfo => !(ArmyEntity is null);
        public bool IsAvailable => !ArmyEntity;

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
            if (IsValidArmyInfo)
            {
                UpdateInfo();
            }
        }

        public void UpdateInfo()
        {
            healthInfo.text = ArmyEntity.data.Health + "/" + ArmyEntity.data.MaxHealth;
            health.SetProgress(ArmyEntity.data.Health / (float)ArmyEntity.data.MaxHealth);
            angerInfo.text = ArmyEntity.data.Anger + "/100";
            anger.SetProgress(ArmyEntity.data.Anger / 100f);
            icon.sprite = ArmyEntity.data.Icon;
        }

        public void Reassign()
        {
            if (!IsValidArmyInfo)
            {
                gameObject.SetActive(false);
            }
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
            ArmyEntity.OnMouseDown();
            ArmyEntity.OnMouseUp();
        }
    }
}