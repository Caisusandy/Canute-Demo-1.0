using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.Exploration
{
    public class PrizeDisplay : MonoBehaviour
    {
        public static PrizeDisplay instance;
        public List<Prize> prizes;
        public Text prizeInfo;

        public void Awake()
        {
            instance = this;
        }
        public void Start()
        {
            int count;
            if (!(Game.PlayerData.ExplorationTeam.CurrentPrize is null))
            {
                if (!(Game.PlayerData.ExplorationTeam.CurrentPrize.prizeBoxes is null))
                    count = Game.PlayerData.ExplorationTeam.CurrentPrize.prizeBoxes.Count;
                else count = 0;
            }
            else count = 0;

            for (int i = 0; i < count; i++)
            {
                prizes[i].Display(Game.PlayerData.ExplorationTeam.CurrentPrize.prizeBoxes[i]);
            }
            for (int i = count; i < 4; i++)
            {
                prizes[i].Disable();
            }
        }
    }
}