using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public class EventCard : Prototype
    {
        public enum Type
        {
            @event,
            building,
            dragon,
        }

        [SerializeField] protected int count;
        [SerializeField] protected Type cardType;
        [SerializeField] protected Career career;
        [SerializeField] protected EventCardPropertyList eventCardProperty = new EventCardPropertyList();

        public int Count => count;
        public Effect Effect => EventCardProperty[0].Effect;
        public EventCardPropertyList EventCardProperty => eventCardProperty;
        public override GameObject Prefab => prefab ?? GameData.Prefabs.NormalEventCard;
        public Type CardType { get => cardType; set => cardType = value; }
        public Career Career { get => career; set => career = value; }
    }


    [Serializable]
    public class EventCardPropertyList
    {
        public List<EventCardProperty> properties;

        public EventCardProperty this[int index]
        {
            get => properties[index];
            set => properties[index] = value;
        }
    }

    [Serializable]
    public struct EventCardProperty
    {
        [SerializeField] private TargetType targetTypes;
        [SerializeField] private int cost;
        [SerializeField] private HalfEffect effect;

        public HalfEffect Effect { get => effect; set => effect = value; }
        public int Cost { get => cost; set => cost = value; }
        public TargetType TargetType { get => targetTypes; set => targetTypes = value; }
    }

}
