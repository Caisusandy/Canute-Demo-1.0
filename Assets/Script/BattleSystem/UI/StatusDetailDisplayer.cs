using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Canute.BattleSystem;


namespace Canute.BattleSystem.UI
{
    public class StatusDetailDisplayer : BattleUIBase
    {
        public Text text;
        public Status status;

        // Start is called before the first frame update
        void Start()
        {
            text.text = status.Info();
        }

        // Update is called once per frame
        void Update()
        {

        }


    }
}