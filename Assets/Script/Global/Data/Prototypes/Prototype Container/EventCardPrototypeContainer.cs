using System;
using System.Collections.Generic;
using UnityEngine;

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
