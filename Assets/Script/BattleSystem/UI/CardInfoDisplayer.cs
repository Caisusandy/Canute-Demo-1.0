using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class CardInfoDisplayer : BattleUIBase
    {
        public Text text;
        public Image image;
        public HorizontalLayoutGroup horizontalLayoutGroup;

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
                image.enabled = true;
                horizontalLayoutGroup.enabled = true;
            }
            else
            {
                horizontalLayoutGroup.enabled = false;
                image.enabled = false;
                text.text = "";
            }
        }
    }
}