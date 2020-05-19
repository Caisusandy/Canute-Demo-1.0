using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class HighBar : BattleUIBase
    {
        public Text RoundDisplayer;

        // Start is called before the first frame update
        private void Start()
        {
        }

        // Update is called once per frame
        private void Update()
        {
            if (Battle?.CurrentStat == Battle.Stat.begin)
            {
                RoundDisplayer.text = "Start";
            }
            else
            {
                RoundDisplayer.text = Battle.Round.wave + "-" + Battle.Round.round;
            }
        }
    }

}