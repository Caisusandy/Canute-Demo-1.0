using Canute.BattleSystem;
using System;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public class EventCardItem : Item, IPrototypeCopy<EventCard>
    {
        [SerializeField] private int level;

        public static EventCardItem Empty => new EventCardItem() { protoName = "Empty" };

        public EventCard Prototype { get => GameData.Prototypes.GetEventCardPrototype(protoName); set => protoName = value?.Name; }
        public override Prototype Proto => Prototype;
        public override int Level => level + 1;
        public override Type ItemType => Type.eventCard;

        public int Cost => Prototype.EventCardProperty[level].Cost;
        public Effect Effect => Prototype.EventCardProperty[level].Effect;
        public TargetType Target => Prototype.EventCardProperty[level].TargetType;
        public Career Career => Prototype.Career;

        public EventCardItem()
        {
            protoName = "Empty";
        }

        public EventCardItem(EventCard prototype)
        {
            this.protoName = prototype.Name;
        }
    }
}
