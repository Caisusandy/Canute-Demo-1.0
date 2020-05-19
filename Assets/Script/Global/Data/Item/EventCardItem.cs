using Canute.BattleSystem;
using System;

namespace Canute
{
    [Serializable]
    public class EventCardItem : Item, IPrototypeCopy<EventCard>
    {
        public static EventCardItem Empty => new EventCardItem() { protoName = "Empty" };

        public EventCard Prototype { get => GameData.Prototypes.GetEventCardPrototype(protoName); set => protoName = value?.Name; }
        public Effect Effect => Prototype.EventCardProperty[Level - 1];
        public override Prototype Proto => Prototype;
        public override int Level => 1;
        public Career Career => Prototype.Career;

    }
}
