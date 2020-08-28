using UnityEngine;

namespace Canute.ExplorationSystem
{
    [CreateAssetMenu(fileName = "Story Price", menuName = "Game Data/Excavation/Story Prize")]
    public class StoryPrizeSheet : ScriptableObject
    {
        public StoryPrize prize;

        public static implicit operator StoryPrize(StoryPrizeSheet prize)
        {
            return prize.prize;
        }
    }
}
