using Canute.BattleSystem.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Canute;
using Canute.Languages;
using Canute.Shops;

namespace Canute.UI
{
    public class PlayerCurrencyDisplayer : MonoBehaviour
    {
        public Text goldDisplayer;
        public Text goldNameDisplayer;

        public Text manpowerDisplayer;
        public Text manpowerNameDisplayer;

        public Text mantleAlloyDisplayer;
        public Text mantleAlloyNameDisplayer;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            goldDisplayer.text = Game.PlayerData.Gold.ToString();
            manpowerDisplayer.text = Game.PlayerData.Manpower.ToString();
            mantleAlloyDisplayer.text = Game.PlayerData.MantleAlloy.ToString();
            goldNameDisplayer.text = Currency.Type.gold.Lang();
            manpowerNameDisplayer.text = Currency.Type.manpower.Lang();
            mantleAlloyNameDisplayer.text = Currency.Type.mantleAlloy.Lang();
        }
    }

}