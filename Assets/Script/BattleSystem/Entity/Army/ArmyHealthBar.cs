using Canute.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem
{
    public class ArmyHealthBar : ProgressBar
    {
        public Text text;
        public ArmyEntity armyEntity;



        public void Update()
        {
            if (armyEntity) progressImage.color = armyEntity.Owner == Game.CurrentBattle.Player ? Color.green : Color.red;
            SetProgress(armyEntity.data.HealthPercent);
            text.text = armyEntity.data.Health + (armyEntity.data.Armor == 0 ? "" : ("(+" + armyEntity.data.Armor + ")")) + " / " + armyEntity.data.MaxHealth;
        }
    }
}
