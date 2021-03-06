﻿using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public class LSArmySkillCardUI : MonoBehaviour
    {
        public Image frame;
        public Text skillName;
        public Text info;
        public ArmyItem displayingArmy;

        public void Display(ArmyItem armyItem)
        {
            displayingArmy = armyItem;

            skillName.text = armyItem.Properties.SkillPack.GetDisplayingName();
            info.text = armyItem.Properties.SkillPack.Info();
        }
    }
}