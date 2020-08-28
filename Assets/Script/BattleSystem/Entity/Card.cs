using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{

    [Serializable]
    public class Card : EntityData, IActionPointUser
    {
        public static Card LastCard { get; set; }

        public enum Types
        {
            normal,
            centerEvent,
            eventCard,
            special
        }

        [SerializeField] protected Effect effect;
        [SerializeField] protected Career career;
        [SerializeField] protected Types type;
        [SerializeField] protected TargetType target;
        [SerializeField] protected int actionPoint;

        public Effect Effect { get => effect; set => effect = value; }
        public Career Career => career;
        public Types Type => type;
        public TargetType Target { get => target; set => target = value; }
        public int ActionPoint { get => actionPoint; set => actionPoint = value > 0 ? value : 0; }

        private Card(Types type, Career career, Effect effect) : base()
        {
            this.type = type;
            this.career = career;
            this.prefab = (type == Types.normal) || (type == Types.centerEvent) ? GameData.Prefabs.CentralDeckCard : GameData.Prefabs.NormalEventCard;

            this.name = type.ToString() + "-" + career.ToString();
            this.effect = effect;
        }

        public Card(Types type, Career career, Effect effect, int actionPoint) : this(type, career, effect)
        {
            this.actionPoint = actionPoint;
        }

        public Card(Types type, Career career, Effect effect, int actionPointRequire, TargetType targetType) : this(type, career, effect, actionPointRequire)
        {
            this.target = targetType;
        }

        public Card(EventCardItem eventCard) : this(Types.eventCard, Career.none, eventCard.Effect.Clone(), eventCard.Cost, eventCard.Target)
        {
            if (type == Types.eventCard)
            {
                switch (eventCard.Prototype.CardType)
                {
                    case EventCard.Type.@event:
                        prefab = GameData.Prefabs.NormalEventCard;
                        break;
                    case EventCard.Type.building:
                        prefab = GameData.Prefabs.BuildingEventCard;
                        type = Types.special;
                        break;
                    case EventCard.Type.dragon:
                        prefab = GameData.Prefabs.DragonEventCard;
                        type = Types.special;
                        break;
                    default:
                        prefab = GameData.Prefabs.NormalEventCard;
                        break;
                }
            }
            else
            {
                this.prefab = GameData.Prefabs.CentralDeckCard;
            }
        }


        public Card(Types types, EventCard eventCard, int level) : this(types, Career.none, eventCard.EventCardProperty[level - 1].Effect.Clone(), eventCard.EventCardProperty[level - 1].Cost, eventCard.EventCardProperty[level - 1].TargetType)
        {
            if (type != Types.normal)
            {
                switch (eventCard.CardType)
                {
                    case EventCard.Type.@event:
                        prefab = GameData.Prefabs.NormalEventCard;
                        break;
                    case EventCard.Type.building:
                        prefab = GameData.Prefabs.BuildingEventCard;
                        break;
                    case EventCard.Type.dragon:
                        prefab = GameData.Prefabs.DragonEventCard;
                        break;
                    default:
                        prefab = GameData.Prefabs.NormalEventCard;
                        break;
                }
            }
            else
            {
                this.prefab = GameData.Prefabs.CentralDeckCard;
            }
        }

        /// <summary>
        /// Only for central deck
        /// </summary>
        /// <param name="types"></param>
        /// <param name="eventCard"></param>
        public Card(Types types, EventCard eventCard) : this(types, Career.none, eventCard.EventCardProperty[0].Effect.Clone(), eventCard.EventCardProperty[0].Cost, eventCard.EventCardProperty[0].TargetType)
        {
            prefab = GameData.Prefabs.NormalEventCard;
        }

        public bool IsValidTarget(Entity entity)
        {
            bool result = true;
            int targettypeid = (int)Target;

            for (int i = 0; i < 10; i++)
            {
                int cur = (int)Mathf.Pow(2, i);
                if ((targettypeid - cur) % (int)Mathf.Pow(2, i + 1) == 0)
                {
                    result = result && IsValidTarget((TargetType)cur, Owner, entity);
                    targettypeid -= cur;
                }
            }
            return result;
        }

        public bool Play()
        {
            if (!OwnerHaveEnoughActionPoint())
            {
                UI.BattleUI.SendMessage("Card did not played: no enough action point");
                return false;
            }
            bool ret = effect.Execute(true);
            if (ret)
            {
                LastCard = this;
                TriggerPlayCardStatus();
                SpentActionPoint();
            }
            return ret;
        }

        /// <summary>
        /// Triggerer of Play Card
        /// </summary>
        public void TriggerPlayCardStatus()
        {
            (effect.Source as IStatusContainer)?.Trigger(TriggerCondition.Conditions.playCard, ref effect);
            effect.Source?.Owner?.Trigger(TriggerCondition.Conditions.playCard, ref effect);
        }

        public int GetActualActionPointSpent()
        {
            if (effect.Type == Effect.Types.enterAttack || effect.Type == Effect.Types.enterMove)
            {
                return 0;
            }

            ICareerLabled source = effect.Source.Exist()?.Data as ICareerLabled;
            if (source is null)
            {
                return ActionPoint;
            }
            if (source.Career == career)
            {
                int v = (ActionPoint - 1);
                return v >= 0 ? v : 0;
            }
            return ActionPoint;
        }

        public bool OwnerHaveEnoughActionPoint()
        {
            IStatusContainer statusContainer = effect.Source as IStatusContainer;

            if (!(statusContainer is null))
            {
                foreach (var item in statusContainer.StatList.GetAllStatus(Effect.Types.@event, TriggerCondition.Conditions.playCard, "name:noActionPointRequire"))
                    if (item.TriggerConditions.IsValid())
                        return true;
            }
            foreach (var item in Effect.Source.Owner.StatList.GetAllStatus(Effect.Types.@event, TriggerCondition.Conditions.playCard, "name:noActionPointRequire"))
                if (item.TriggerConditions.IsValid())
                    return true;

            return Owner.ActionPoint >= GetActualActionPointSpent();
        }

        public void SpentActionPoint()
        {
            Debug.Log(GetActualActionPointSpent());
            Debug.Log(Owner.ActionPoint);
            Owner.ActionPoint -= GetActualActionPointSpent();
            Debug.Log(Owner.ActionPoint);
        }

        public void BackActionPoint()
        {
            if (Effect.Type == Effect.Types.enterMove || Effect.Type == Effect.Types.enterAttack)
            {
                return;
            }

            Owner.ActionPoint += GetActualActionPointSpent();
        }

        public override string ToString()
        {
            return base.ToString() + ";\n Card Type:" + type.ToString() + ";\nEffect:" + effect.Type.ToString();
        }

        public static bool IsValidTarget(TargetType targetTypes, Player Owner, Entity entity)
        {
            switch (targetTypes)
            {
                case TargetType.self:
                    return Owner == entity.Owner;
                case TargetType.enemy:
                    return Owner != entity.Owner;
                case TargetType.player:
                    return entity.Owner != null;
                case TargetType.armyEntity:
                    return entity is ArmyEntity;
                case TargetType.buildingEntity:
                    return entity is BuildingEntity;
                case TargetType.cellEntity:
                    return entity is CellEntity;
                case TargetType.cardEntity:
                    return entity is CardEntity;
                case TargetType.passiveEntity:
                    return entity is IPassiveEntity;
                case TargetType.aggressiveEntity:
                    return entity is IAggressiveEntity;
                default:
                    break;
            }
            return false;
        }

        public static List<Card> ToCards(IEnumerable<EventCardItem> eventCardItems)
        {
            if (eventCardItems is null)
            {
                return new List<Card>();
            }
            List<Card> cards = new List<Card>();
            foreach (var item in eventCardItems)
            {
                for (int i = 0; i < item.Prototype.Count; i++)
                {
                    Card card = new Card(item);
                    cards.Add(card);
                }
            }
            return cards;
        }


    }


}
