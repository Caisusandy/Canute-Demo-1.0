using Canute.BattleSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public class ArmyIcon : MonoBehaviour
    {
        public Text levelDisplayer;
        public Image armyIcon;
        public Image rarity;

        public ArmyItem displayingArmy;

        // Start is called before the first frame update
        private void Awake()
        {
            rarity = GetComponent<Image>();
            armyIcon = transform.Find("ArmyIcon").GetComponent<Image>();
            levelDisplayer = transform.Find("ArmyLevel").GetComponent<Text>();
        }

        public void Display(ArmyItem item)
        {
            displayingArmy = item;

            if (!displayingArmy)
            {
                armyIcon.sprite = null;
                levelDisplayer.text = string.Empty;
                rarity.sprite = GameData.SpriteLoader.Get(SpriteAtlases.rarity, Rarity.none.ToString());
            }
            else
            {
                rarity.sprite = GameData.SpriteLoader.Get(SpriteAtlases.rarity, item.Rarity.ToString());
                armyIcon.sprite = displayingArmy.Prototype.Icon;
                levelDisplayer.text = "Lv." + displayingArmy.Level;
            }
        }

    }
}