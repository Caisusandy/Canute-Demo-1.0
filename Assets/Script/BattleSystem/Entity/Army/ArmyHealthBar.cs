using Canute.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace Canute.BattleSystem
{
    public class ArmyHealthBar : ProgressBar
    {
        public Text text;
        public ArmyEntity armyEntity;

        public void Update()
        {
            SetProgress((float)armyEntity.data.Health / armyEntity.data.MaxHealth);
            text.text = armyEntity.data.Health + " / " + armyEntity.data.MaxHealth;
        }
    }
}
