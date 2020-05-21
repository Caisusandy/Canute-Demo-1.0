using Canute.BattleSystem.AI;
using Canute.BattleSystem.Buildings;
using Canute.BattleSystem.UI;
using Canute.Languages;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public class Player : EntityData, IActionPointUser, INameable, IUUIDLabeled, IStatusContainer, ICloneable
    {
        [SerializeField] protected AIEntity.PersonalityType personality;
        [SerializeField] protected BattleLeader viceCommander;
        [SerializeField] protected Legion legion;
        [SerializeField] protected EventCardPile pile;

        [Header("Items")]
        [SerializeField] protected int maxArmyCount;
        [SerializeField] protected int actionPoint;
        [SerializeField] protected List<Card> eventCardPile = new List<Card>();
        [SerializeField] protected List<Card> handCard = new List<Card>();
        [SerializeField] protected StatusList stats = new StatusList();
        [SerializeField] protected CampusEntity campus;



        public override Prototype Prototype => null;
        public override bool HasPrototype => false;
        public override Player Owner { get => this; set { Debug.LogError("A player cannot be owned!"); } }
        public override GameObject Prefab { get => null; set { } }
        public override string DisplayingName => this.Lang(Name, "name");
        public override Sprite DisplayingIcon => null;
        public override Sprite DisplayingPortrait => null;
        public new PlayerEntity Entity => BattleSystem.Entity.Get<PlayerEntity>(uuid);


        public List<Card> HandCard => handCard;
        public List<Card> EventCardPile => eventCardPile;
        public List<BattleArmy> BattleArmies => Game.CurrentBattle.GetArmies(this);
        public List<BattleBuilding> Buildings => Game.CurrentBattle.GetBuildings(this);
        public BattleLeader ViceCommander { get => viceCommander; set => viceCommander = value; }
        public StatusList StatList { get => stats; set => stats = value; }
        public StatusList GetAllStatus() => StatList;



        public int ActionPoint { get => actionPoint; set => actionPoint = value; }
        public int HasCampus => Campus ? 1 : 0;
        public int MaxArmyCount { get => maxArmyCount; set => maxArmyCount = value; }
        public double Morale => (BattleArmies.Count + HasCampus) / (double)(MaxArmyCount + HasCampus);
        public bool IsInTurn => this == Game.CurrentBattle?.Round?.CurrentPlayer;
        public CampusEntity Campus { get => campus; set => campus = value; }
        public AIEntity AI => Entity as AIEntity;
        public AIEntity.PersonalityType Personality { get => personality; set => personality = value; }


        public Legion Legion { get => legion; set => legion = value; }


        /// <summary>
        /// To Setup a Game Player
        /// </summary>
        /// <param name="legion"></param>
        public Player(string name, LegionSet playerLegionSet) : this()
        {
            Legion legion = playerLegionSet.Legion;
            LeaderItem leaderItem = playerLegionSet.Leader;
            EventCardPile pile = playerLegionSet.EventCardPile;

            this.name = name;
            this.legion = legion;
            this.pile = pile;

            viceCommander = new BattleLeader(leaderItem);

            eventCardPile = Card.ToCards(pile);
            foreach (var item in eventCardPile)
            {
                item.Owner = this;
            }

            List<BattleArmy> battleArmies = new List<BattleArmy>();
            foreach (ArmyItem item in legion.Armies)
            {
                BattleArmy army = new BattleArmy(item, this);
                battleArmies.Add(army);
            }


            Resonance.Resonate(ref battleArmies);

            Game.CurrentBattle.Armies.AddRange(battleArmies);
            maxArmyCount = battleArmies.Count;
        }


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
            this.legion = legion;
            this.pile = pile;

            viceCommander = new BattleLeader(leaderItem);

            eventCardPile = Card.ToCards(pile);
            foreach (var item in eventCardPile)
            {
                item.Owner = this;
            }

            List<BattleArmy> battleArmies = new List<BattleArmy>();
            foreach (ArmyItem item in LegionSet.Legion.Armies)
            {
                BattleArmy army = new BattleArmy(item, this);
                battleArmies.Add(army);
            }
            Resonance.Resonate(ref battleArmies);
            Game.CurrentBattle.Armies.AddRange(battleArmies);

            maxArmyCount = battleArmies.Count;
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

        /// <summary> 补充手牌 </summary>
        public void RefillCard()
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
                BattleSystem.Entity.Get(item.UUID).Destroy();
            }

            int normalCount = GetCard(Card.Types.normal).Count + GetCard(Card.Types.centerEvent).Count;
            if (normalCount < 5)
            {
                int a = 0;
                int m = 0;
                foreach (var item in GetCard(Card.Types.normal))
                {
                    a += item.Effect.Type == Effect.Types.enterAttack ? 1 : 0;
                    m += item.Effect.Type == Effect.Types.enterMove ? 1 : 0;
                }
                if (a == 0 && GetCard(Card.Types.centerEvent).Count < 5)
                {
                    Game.CurrentBattle.GetHandCard(this, Effect.Types.enterAttack, 1);
                    normalCount++;
                }
                if (m == 0 && GetCard(Card.Types.centerEvent).Count < 5)
                {
                    Game.CurrentBattle.GetHandCard(this, Effect.Types.enterMove, 1);
                    normalCount++;
                }
                if (normalCount < 5)
                {
                    Game.CurrentBattle.GetHandCard(this, 5 - normalCount);
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
            ActionPoint = 8;
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
    }
}