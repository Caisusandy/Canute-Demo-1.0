using Canute.BattleSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public class ArmyIcon : MonoBehaviour
    {
        public Text levelDisplayer;
        public Image armyIcon;
        public Image BG;

        public ArmyItem displayingArmy;

        // Start is called before the first frame update
        private void Awake()
        {
            BG = GetComponent<Image>();
            armyIcon = transform.Find("ArmyIcon").GetComponent<Image>();
            levelDisplayer = transform.Find("ArmyLevel").GetComponent<Text>();
        }

        public void Display(ArmyItem armyItem)
        {
            displayingArmy = armyItem;

            if (!displayingArmy)
            {
                armyIcon.sprite = null;
                levelDisplayer.text = string.Empty;
            }
            else
            {
                BG.color = displayingArmy.Prototype.Rarity.GetColor();
                armyIcon.sprite = displayingArmy.Prototype.Icon;
                levelDisplayer.text = "Lv." + displayingArmy.Level;
            }
        }

    }
}