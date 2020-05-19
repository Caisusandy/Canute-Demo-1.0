using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{

    [Serializable]
    public class Card : EntityData, IActionPointUser
    {
        public enum Types
        {
            normal,
            centerEvent,
            eventCard
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

        public Card() : base()
        {
            prefab = GameData.Prefabs?.CentralDeckCard;
        }

        public Card(Types type, Career career) : this()
        {
            this.type = type;
            this.career = career;
            this.prefab = type == Types.eventCard ? GameData.Prefabs.EventCard : GameData.Prefabs.CentralDeckCard;
            name = type.ToString() + "-" + career.ToString();
        }

        public Card(Types type, Career career, Effect effect) : this(type, career)
        {
            this.effect = effect;
        }

        public Card(Types type, Career career, Effect effect, int actionPointRequire) : this(type, career, effect)
        {
            this.actionPoint = actionPointRequire;
        }

        public Card(Types type, Career career, Effect effect, int actionPointRequire, TargetType targetType) : this(type, career, effect, actionPointRequire)
        {
            this.target = targetType;
        }

        public Card(EventCardItem eventCard) : this(Types.eventCard, Career.none, eventCard.Effect.Clone(), eventCard.Prototype.Cost, eventCard.Prototype.Target) { }

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

        public int GetActualActionPointSpent()
        {
            ICareerLabled target = effect.Target.Data as ICareerLabled;
            if (target is null)
            {
                return ActionPoint;
            }
            if (target.Career == career)
            {
                return ActionPoint - 1;
            }
            return ActionPoint;
        }

        public override string ToString()
        {
            return base.ToString() + "Card Type" + type.ToString() + " Effect:" + effect.Type.ToString();
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

        public static List<Card> Cards(IEnumerable<EventCardItem> eventCardItems)
        {
            if (eventCardItems is null)
            {
                return null;
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

    [Serializable]
    public class EventCardProperty
    {
        public List<Effect> effects;

        public Effect this[int index]
        {
            get => effects[index];
            set => effects[index] = value;
        }
    }
}
