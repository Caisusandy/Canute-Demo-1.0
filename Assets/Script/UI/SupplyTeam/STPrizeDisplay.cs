using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.SupplyTeam
{
    public class STPrizeDisplay : MonoBehaviour
    {
        public static STPrizeDisplay instance;
        public List<STPrize> prizes;

        public void Awake()
        {
            instance = this;
        }

        public void Start()
        {
            int count;
            if (!(Game.PlayerData.SupplyTeam.CurrentPrize is null))
            {
                if (!(Game.PlayerData.SupplyTeam.CurrentPrize.prizeBoxes is null))
                    count = Game.PlayerData.SupplyTeam.CurrentPrize.prizeBoxes.Count;
                else count = 0;
            }
            else count = 0;

            for (int i = 0; i < count; i++)
            {
                prizes[i].Display(Game.PlayerData.SupplyTeam.CurrentPrize.prizeBoxes[i]);
            }
            for (int i = count; i < 4; i++)
            {
                prizes[i].Disable();
            }
        }
    }
}