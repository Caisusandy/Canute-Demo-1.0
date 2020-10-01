using Canute.BattleSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public class LeaderCardUI : MonoBehaviour
    {
        public Text leaderName;
        public Image portrait;
        public Image bg;
        public Image nameBG;
        public Image career;

        public LeaderItem leaderItem;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Display(LeaderItem leaderItem)
        {
            this.leaderItem = leaderItem;
            if (leaderItem)
            {
                leaderName.text = leaderItem.DisplayingName;
                career.sprite = GameData.SpriteLoader.Get(SpriteAtlases.careerIcon, leaderItem.Career.ToString());
                nameBG.sprite = GameData.SpriteLoader.Get(SpriteAtlases.rarity, leaderItem.Rarity.ToString());
                //nameBG.sprite =
                //portrait.sprite = leaderItem.Prototype.Portrait;
            }
            else
            {
                leaderName.text = "";
                career.sprite = GameData.SpriteLoader.Get(SpriteAtlases.careerIcon, Career.none.ToString());
                nameBG.sprite = GameData.SpriteLoader.Get(SpriteAtlases.rarity, Rarity.none.ToString());
            }
        }
    }
}