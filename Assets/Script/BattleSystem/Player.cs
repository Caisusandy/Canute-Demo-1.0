using Canute.BattleSystem.AI;
using Canute.BattleSystem.Buildings;
using Canute.BattleSystem.UI;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public class Player : EntityData, IActionPointUser, INameable, IUUIDLabeled, IStatusContainer, ICloneable
    {
        [SerializeField] protected PlayerEntity.Personality personality;
        [SerializeField] protected BattleLeader viceCommander;
        [SerializeField] protected LegionSet legion;
        [SerializeField] protected EventCardPile pile;

        [SerializeField] protected int maxHandCardCount = 5;
        [SerializeField] protected int maxArmyCount;
        [Header("Items")]
        [SerializeField] protected int actionPoint;
        [SerializeField] protected List<Card> eventCardPile = new List<Card>();
        [SerializeField] protected List<Card> handCard = new List<Card>();
        [SerializeField] protected StatusList stats = new StatusList();



        public override Prototype Prototype => null;
        public override bool HasValidPrototype => false;
        public override Player Owner { get => this; set { Debug.LogError("A player cannot be owned!"); } }
        public override GameObject Prefab { get => null; set { } }
        public override string DisplayingName => this.Lang(Name, "name");
        public override Sprite Icon => null;
        public new PlayerEntity Entity => BattleSystem.Entity.Get<PlayerEntity>(uuid);


        public List<Card> HandCard => handCard;
        public List<Card> EventCardPile => eventCardPile;
        public List<BattleArmy> BattleArmies => Game.CurrentBattle.GetArmies(this);
        public List<BattleBuilding> Buildings => Game.CurrentBattle.GetBuildings(this);
        public BattleLeader ViceCommander { get => viceCommander; set => viceCommander = value; }
        public StatusList StatList { get => stats; set => stats = value; }
        public StatusList GetAllStatus() => StatList;



        public int MaxHandCardCount { get => maxHandCardCount; set => maxHandCardCount = value; }
        public int MaxArmyCount { get => maxArmyCount; set => maxArmyCount = value; }
        public int ActionPoint { get => actionPoint; set => actionPoint = value; }
        public bool IsInTurn => this == Game.CurrentBattle?.Round?.CurrentPlayer;
        public AIEntity AI => Entity as AIEntity;
        public PlayerEntity.Personality Personality { get => personality; set => personality = value; }


        public LegionSet LegionSet { get => legion; set => legion = value; }


        /// <summary>
        /// To Setup a Game Player
        /// </summary>
        /// <param name="legion"></param>
        public Player(string name, LegionSet playerLegionSet) : this(name, playerLegionSet, UUID.Player) { }


        /// <summary>
        /// To Setup a Game Player
        /// </summary>
        /// <param name="LegionSet"></param> 
        public Player(string name, LegionSet LegionSet, UUID uuid)
        {
            Legion legion = LegionSet.Legion;
            LeaderItem leaderItem = LegionSet.Leader;
            EventCardPile pile = LegionSet.EventCardPile;

            this.uuid = uuid;
            this.name = name;
            this.legion = LegionSet;
            this.pile = pile;
        }

        public void SetupLeader(LeaderItem leaderItem)
        {
            viceCommander = new BattleLeader(leaderItem);
        }

        public void SetupEventCardPile(EventCardPile pile)
        {
            if (pile is null)
            {
                return;
            }
            eventCardPile = Card.ToCards(pile);
            foreach (var item in eventCardPile)
                item.Owner = this;
        }

        public void SetupLegion(Legion legion)
        {
            if (legion is null)
            {
                return;
            }
            List<BattleArmy> battleArmies = new List<BattleArmy>();
            foreach (ArmyItem item in legion.Armies)
            {
                if (!item) continue;
                BattleArmy army = new BattleArmy(item, this);
                battleArmies.Add(army);
            }

            Game.CurrentBattle.Armies.AddRange(battleArmies);
            maxArmyCount = battleArmies.Count;
        }

        public void CreateArmyCard()
        {
            for (int i = 0; i < BattleArmies.Count; i++)
            {
                BattleArmy item = BattleArmies[i];
                Card card = new Card(Card.Types.eventCard, Career.none, new Effect(Effect.Types.createArmy, 1, i), 0, TargetType.cellEntity) { Owner = this };
                card.Effect["name"] = item.Name;
                ArmyCardEntity.Create(card, item);
            }
        }

        private Player() { NewUUID(); }

        public List<Card> GetCard(Card.Types type)
        {
            List<Card> cards = new List<Card>();
            foreach (Card item in HandCard)
            {
                if (item.Type == type)
                {
                    cards.Add(item);
                }
            }
            return cards;
        }

        /// <summary>
        /// discard all (when player end turn)
        /// </summary>
        public void EndTurnDiscard()
        {
            //Send player event card back
            foreach (Card item in GetCard(Card.Types.eventCard))
            {
                EventCardPile.Add(item);
                BattleSystem.Entity.Get(item.UUID).Destroy();
            }

            //clear all player card
            foreach (Card item in GetCard(Card.Types.normal))
            {
                BattleSystem.Entity.Get(item.UUID).Exist()?.Destroy();
            }

            //clear all player card
            foreach (Card item in GetCard(Card.Types.centerEvent))
            {
                BattleSystem.Entity.Get(item.UUID).Exist()?.Destroy();
            }

            //clear all player card
            foreach (Card item in GetCard(Card.Types.special))
            {
                BattleSystem.Entity.Get(item.UUID).Exist()?.Destroy();
            }

        }

        /// <summary> 补充手牌 </summary>
        public void RefillCard()
        {
            int normalCount = GetCard(Card.Types.normal).Count + GetCard(Card.Types.centerEvent).Count;
            if (normalCount < MaxHandCardCount)
            {
                int a = 0;
                int m = 0;
                foreach (var item in GetCard(Card.Types.normal))
                {
                    a += item.Effect.Type == Effect.Types.enterAttack ? 1 : 0;
                    m += item.Effect.Type == Effect.Types.enterMove ? 1 : 0;
                }
                if (a == 0 && GetCard(Card.Types.centerEvent).Count < MaxHandCardCount)
                {
                    Game.CurrentBattle.GetHandCard(this, Effect.Types.enterAttack, 1);
                    normalCount++;
                }
                if (m == 0 && GetCard(Card.Types.centerEvent).Count < MaxHandCardCount)
                {
                    Game.CurrentBattle.GetHandCard(this, Effect.Types.enterMove, 1);
                    normalCount++;
                }
                if (normalCount < MaxHandCardCount)
                {
                    Game.CurrentBattle.GetHandCard(this, MaxHandCardCount - normalCount);
                }
            }

            for (int i = 0; i < 2; i++)
            {
                if (eventCardPile.Count == 0)
                {
                    break;
                }

                Card card = eventCardPile[UnityEngine.Random.Range(0, eventCardPile.Count)];
                card.Owner = this;
                eventCardPile.Remove(card);
                CardEntity.Create(card);
            }
        }

        /// <summary> 补充行动点数 </summary>
        public void RefillAction()
        {
            ActionPoint = 7;
        }

        public override object Clone()
        {
            Player playerCopy = MemberwiseClone() as Player;
            playerCopy.UUID = UUID;
            playerCopy.eventCardPile = eventCardPile?.Clone();
            playerCopy.handCard = HandCard?.Clone();
            playerCopy.stats = stats?.Clone() as StatusList;
            playerCopy.viceCommander = viceCommander?.Clone() as BattleLeader;

            return playerCopy;
        }

        /// <summary> 给玩家添加手牌 </summary>
        /// <param name="card"></param>
        public void AddHandCard(Card card)
        {
            HandCard.Add(card);
            card.Owner = this;
        }

        public bool RemoveHandCard(Card card)
        {
            return HandCard.Remove(card);
        }

        public bool TryEndTurn()
        {
            if (!IsInTurn)
            {
                return false;
            }
            else
            {
                return Game.CurrentBattle.TryEndTurn();
            }
        }
    }
}