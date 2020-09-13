using UnityEngine;

namespace Canute.SupplyTeam
{
    [CreateAssetMenu(fileName = "Item Price", menuName = "Game Data/Excavation/Item Prize")]
    public class ItemPrizeSheet : ScriptableObject
    {
        public ItemPrize prize;

        public static implicit operator ItemPrize(ItemPrizeSheet prize)
        {
            return prize.prize;
        }
    }
}
