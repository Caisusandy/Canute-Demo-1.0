﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.Legion
{
    public class LSLegionPopCount : Icon
    {
        public Text text;
        public LSLegionDisplay LegionDisplay => LSLegionDisplay.instance;

        public void Update()
        {
            text.text = LegionDisplay.Legion.PopCount.ToString();
        }

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Label.text = BattleSystem.PropertyType.pop.Lang();
        }
    }
}
