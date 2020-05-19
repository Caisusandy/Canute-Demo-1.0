using UnityEngine;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "Game Data", menuName = "Game Data/Event Tree", order = 2)]
    public class EventTreeData : ScriptableObject
    {
        public EventTree eventTree;
    }
}
