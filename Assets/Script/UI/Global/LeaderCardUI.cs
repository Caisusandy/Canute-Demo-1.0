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
            leaderName.text = leaderItem.DisplayingName;

        }
    }
}