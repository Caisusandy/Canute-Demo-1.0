using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.SupplyTeam
{
    [CreateAssetMenu(fileName = "Excavation Price List", menuName = "Game Data/Excavation/Prize Lists")]
    [Obsolete]
    public class ExcavationPrice : ScriptableObject
    {
        [SerializeField] private List<StoryPrizeSheet> storyPrizes;
        [SerializeField] private List<ItemPrizeSheet> armyPrizes;
        [SerializeField] private List<ItemPrizeSheet> equipmentPrizes;
        [SerializeField] private List<ItemPrizeSheet> leaderPrizes;
        [SerializeField] private List<PrizeSheet> uselessPrize;
        [SerializeField] private List<PrizeSheet> failedPrize;

        [Obsolete]
        public List<StoryPrize> StoryPrizes { get => storyPrizes.Select(e => e.prize).ToList(); }
        [Obsolete]
        public List<ItemPrize> ArmyPrizes { get => armyPrizes.Select(e => e.prize).ToList(); }
        [Obsolete]
        public List<ItemPrize> LeaderPrizes { get => leaderPrizes.Select(e => e.prize).ToList(); }
        [Obsolete]
        public List<ItemPrize> EquipmentPrizes { get => equipmentPrizes.Select(e => e.prize).ToList(); }
        [Obsolete]
        public List<ConditionalPrize> UselessPrize { get => uselessPrize.Select(e => e.prize).ToList(); }
        [Obsolete]
        public List<ConditionalPrize> FailedPrize { get => failedPrize.Select(e => e.prize).ToList(); }
    }
}
