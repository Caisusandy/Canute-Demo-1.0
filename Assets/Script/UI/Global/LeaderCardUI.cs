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
            leaderName.text = leaderItem.DisplayingName;
            career.sprite = GameData.SpriteLoader.Get(SpriteAtlases.careerIcon, leaderItem.Career.ToString());
        }
    }
}