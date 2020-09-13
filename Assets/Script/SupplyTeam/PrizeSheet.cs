using UnityEngine;

namespace Canute.SupplyTeam
{
    [CreateAssetMenu(fileName = "Excavation Price", menuName = "Game Data/Excavation/Prize")]
    public class PrizeSheet : ScriptableObject
    {
        public ConditionalPrize prize;

        public static implicit operator Prize(PrizeSheet prize)
        {
            return prize.prize;
        }
    }
}
