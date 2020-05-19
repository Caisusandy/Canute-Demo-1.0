using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public class EventCard : Prototype
    {
        [SerializeField] protected int count;
        [SerializeField] protected int cost;
        [SerializeField] protected TargetType targetTypes;
        [SerializeField] protected Career career;
        [SerializeField] protected EventCardProperty eventCardProperty = new EventCardProperty();

        public int Count => count;
        public int Cost => cost;
        public Effect Effect => EventCardProperty[0];
        public EventCardProperty EventCardProperty => eventCardProperty;
        public override GameObject Prefab => prefab ?? GameData.Prefabs.EventCard;
        public TargetType Target { get => targetTypes; set => targetTypes = value; }
        public Career Career { get => career; set => career = value; }
    }
}
