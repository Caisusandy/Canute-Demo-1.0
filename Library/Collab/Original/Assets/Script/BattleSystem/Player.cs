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

        [Header("Items")]
        [SerializeField] protected int maxArmyCount;
        [SerializeField] protected int actionPoint;
        [SerializeField] protected List<Card> eventCardPile = new List<Card>();
        [SerializeField] protected List<Card> handCard = new List<Card>();
        [SerializeField] protected StatList stats = new StatList();
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
        public Legion Legion { get => legion; set => legion = value; }

        public StatList StatList { get => stats; set => stats = value; }
        public StatList GetAllStatus() => StatList;


        public double Morale => (BattleArmies.Count + HasCampus) / (double)(MaxArmyCount + HasCampus);
        public int ActionPoint { get => actionPoint; set => actionPoint = value; }
        public int HasCampus => Campus ? 1 : 0;
        public int MaxArmyCount { get => maxArmyCount; set => maxArmyCount = value; }
        public CampusEntity Campus { get => campus; set => campus = value; }
        public AIEntity AI => Entity as AIEntity;
        public AIEntity.PersonalityType Personality { get => personality; set => personality = value; }
        public bool IsInTurn => this == Game.CurrentBattle?.Round?.CurrentPlayer;




        /// <summary>
        /// To Setup a Game Player
        /// </summary>
        /// <param name="legion"></param>
        public Player(string name, PlayerLegionSet playerLegionSet) : this()
        {
            Legion legion = playerLegionSet.legion;
            LeaderItem leaderItem = playerLegionSet.Leader;
            EventCardPile EventCardPile = playerLegionSet.eventCardPile;

            this.name = name;
            this.legion = legion;

            viceCommander = new BattleLeader(leaderItem);
            eventCardPile = Card.Cards(EventCardPile);

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
        /// <param name="legion"></param>
        public Player(string name, Legion legion)
        {
            this.name = name;
            this.legion = legion;
            viceCommander = new BattleLeader(legion.Leader);

            List<BattleArmy> battleArmies = new List<BattleArmy>();


            foreach (ArmyItem item in legion.Armies)
            {
                BattleArmy army = new BattleArmy(item, this);
                battleArmies.Add(army);
            }

            Resonance.Resonate(ref battleArmies);

            foreach (BattleArmy item in battleArmies)
            {
                Game.CurrentBattle.Armies.Add(item);
            }

            maxArmyCount = battleArmies.Count;
        }
        /// <summary>
        /// To Setup a Game Player
        /// </summary>
        /// <param name="legion"></param>
        public Player(string name, Legion legion, UUID uUID)
        {
            this.uuid = uUID;
            this.name = name;
            this.legion = legion;
            viceCommander = new BattleLeader(legion.Leader);

            List<BattleArmy> battleArmies = new List<BattleArmy>();

            foreach (ArmyItem item in legion.Armies)
            {
                BattleArmy army = new BattleArmy(item, this);
                battleArmies.Add(army);
            }

            Resonance.Resonate(ref battleArmies);

            foreach (BattleArmy item in battleArmies)
            {
                Game.CurrentBattle.Armies.Add(item);
            }

            maxArmyCount = battleArmies.Count;
        }

        private Player() { this.NewUUID(); }

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

            foreach (Card item in GetCard(Card.Types.eventCard))
            {
                EventCardPile.Add(item);
                BattleSystem.Entity.Get(item.UUID).Destroy();
            }

            for (int i = 0; i < 2; i++)
            {
                if (EventCardPile.Count == 0)
                {
                    break;
                }

                Card card = EventCardPile[UnityEngine.Random.Range(0, EventCardPile.Count)];
                AddHandCard(card);
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
            playerCopy.stats = stats?.Clone() as StatList;
            playerCopy.viceCommander = viceCommander?.Clone() as BattleLeader;

            return playerCopy;
        }

        /// <summary> 给玩家添加手牌 </summary>
        /// <param name="card"></param>
        public void AddHandCard(Card card)
        {
            HandCard.Add(card);
            card.Owner = this;

            if (Game.CurrentBattle.Player == this)
            {
                CardEntity.Create(card, BattleUI.ClientHandCardBar);
            }
            else if (Game.CurrentBattle.Enemy == this && GameData.BuildSetting.PvP)
            {
                CardEntity.Create(card, BattleUI.EnemyHandCardBar);
            }
            else
            {
                NonControllingCardEntity.Create(card, this);
            }

        }

        public bool RemoveHandCard(Card card)
        {
            return HandCard.Remove(card);
        }

        public bool PlayCard(Card card)
        {
            if (ActionPoint < card.GetActualActionPointSpent())
            {
                BattleUI.SendMessage("Card did not played: no enough action point");
                return false;
            }

            bool sucess = card.Effect.Execute();
            if (sucess)
            {
                CountActionPointSpent(card);
            }
            return sucess;
        }

        public void CountActionPointSpent(Card card)
        {
            ActionPoint -= card.ActionPoint;

            if (card.Effect.Target.Data is ICareerLabled && card.ActionPoint != 0)
            {
                if (((ICareerLabled)card.Effect.Target.Data).Career == card.Career)
                {
                    ActionPoint++;
                }
            }
        }

        public void BackActionPointSpent(Card card)
        {
            ActionPoint += card.ActionPoint;

            if (card.Effect.Target.Data is ICareerLabled && card.ActionPoint != 0)
            {
                if (((ICareerLabled)card.Effect.Target.Data).Career == card.Career)
                {
                    ActionPoint--;
                }
            }
        }
    }
}