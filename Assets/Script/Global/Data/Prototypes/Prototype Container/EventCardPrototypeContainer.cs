using System;
using UnityEngine;
using Canute.Module;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "EventCard", menuName = "Prototype/EventCard Prototype")]
    public class EventCardPrototypeContainer : PrototypeContainer<EventCard>
    {
        [ContextMenu("Set as add status")]
        public void WriteAddStatusTemplate()
        {
            prototype = new EventCard()
            {
                CardType = EventCard.Type.@event,
                EventCardProperty = new EventCardPropertyList()
                {
                    properties = new System.Collections.Generic.List<EventCardProperty>()
                    {
                        new EventCardProperty(){
                            Effect = new HalfEffect(){
                                Type = Effect.Types.addStatus,
                                Args = new Args(){
                                    Effect.statusCount +": ",  Effect.turnCount +": ",  Effect.statusType +": ",  Effect.effectType +": ",
                                    Effect.name +": ", Effect.effectSpecialName +": ",  "[triggerCondition]: "
                        } } }
                    }
                }
            };
        }
        [ContextMenu("Set as event")]
        public void WriteEventTemplate()
        {
            prototype = new EventCard()
            {
                CardType = EventCard.Type.@event,
                EventCardProperty = new EventCardPropertyList()
                {
                    properties = new System.Collections.Generic.List<EventCardProperty>()
                    {
                        new EventCardProperty(){
                            Effect = new HalfEffect(){
                                Type = Effect.Types.@event,
                                Args = new Args(){ Effect.name +": "}
                            }
                        }
                    }
                }
            };
        }
        [ContextMenu("Set as effect-relate")]
        public void WriteEffectRelatedTemplate()
        {
            prototype = new EventCard()
            {
                CardType = EventCard.Type.@event,
                EventCardProperty = new EventCardPropertyList()
                {
                    properties = new System.Collections.Generic.List<EventCardProperty>()
                    {
                        new EventCardProperty(){
                            Effect = new HalfEffect(){
                                Type = Effect.Types.effectRelated,
                                Args = new Args(){ Effect.name +": "}
                            }
                        }
                    }
                }
            };
        }
    }


    [Serializable]
    public class EventCardPrototypes : DataList<EventCardPrototypeContainer>
    {

    }
}
