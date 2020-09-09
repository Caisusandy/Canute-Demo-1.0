using System;
using UnityEngine;
using Canute.Module;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "EventCard", menuName = "Prototype/EventCard Prototype")]
    public class EventCardPrototypeContainer : PrototypeContainer<EventCard>
    {

    }


    [Serializable]
    public class EventCardPrototypes : DataList<EventCardPrototypeContainer>
    {

    }
}
