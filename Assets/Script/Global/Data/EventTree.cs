using System;
using System.Collections.Generic;

namespace Canute.BattleSystem
{
    /// <summary>
    /// The way to represent player/global event tree
    /// </summary>
    [Serializable]
    public class EventTree : CheckList
    {
        public void EventPassed(string name)
        {
            this[name] = true;
        }

        public EventTree() : base()
        {
        }

        public EventTree(List<EventCard> eventCardList) : base(GameData.Prototypes.TestingEventCards, eventCardList)
        {
        }

    }
}
