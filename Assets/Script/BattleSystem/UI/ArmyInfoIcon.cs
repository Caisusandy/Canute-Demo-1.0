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
        //public UUID connectedArmyEntityUUID = UUID.Empty;

        public ProgressBar health;
        public ProgressBar anger;

        public Text healthInfo;
        public Text angerInfo;
        public Image icon;

        public ArmyEntity armyEntity;// => Entity.Get<ArmyEntity>(connectedArmyEntityUUID);

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
            if (armyEntity)
            {
                UpdateInfo();
            }
        }

        public void UpdateInfo()
        {
            healthInfo.text = armyEntity.data.Health + "/" + armyEntity.data.MaxHealth;
            health.SetProgress((float)armyEntity.data.Health / armyEntity.data.MaxHealth);
            angerInfo.text = armyEntity.data.Anger + "/100";
            anger.SetProgress(armyEntity.data.Anger / 100f);
            icon.sprite = armyEntity.data.Icon;
        }


        public void Connect(ArmyEntity armyEntity)
        {
            Debug.Log(name + " connected to army " + this.armyEntity.Exist()?.Name);
            //Debug.Log(armyEntity.UUID);
            //Debug.Log(connectedArmyEntityUUID);
            this.armyEntity = armyEntity;
        }

        public void Goto()
        {
            if (!armyEntity)
            {
                return;
            }
            armyEntity.OnMouseDown();
            armyEntity.OnMouseUp();
        }
    }
}