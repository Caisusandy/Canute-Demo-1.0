using Canute.BattleSystem;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public class LeaderIcon : MonoBehaviour
    {
        public Text levelDisplayer;
        public Text nameDisplayer;
        public Image leaderIcon;
        public Image BG;

        public LeaderItem displayingLeader;

        // Update is called once per frame

        public void Display(LeaderItem item)
        {
            displayingLeader = item;
            if (!displayingLeader)
            {
                leaderIcon.sprite = null;
                levelDisplayer.text = string.Empty;
                nameDisplayer.text = string.Empty;
            }
            else
            {
                leaderIcon.sprite = displayingLeader.Prototype.Icon;
                levelDisplayer.text = "Lv." + displayingLeader?.Level;
                nameDisplayer.text = displayingLeader.Name;
            }
        }


        private void Update()
        {
        }
    }
}