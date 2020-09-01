using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class CardInfoDisplayer : BattleUIBase
    {
        public Text text;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (CardEntity.SelectingCard)
            {
                text.text = CardEntity.SelectingCard.data.Effect.Info();
            }
            else
            {
                text.text = "";
            }
        }
    }
}