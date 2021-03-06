﻿using Canute.BattleSystem.AI;
using Canute.BattleSystem.UI;
using Canute.Shops;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Canute.BattleSystem
{
    [Serializable]
    public class Battle : ICloneable, IStatusContainer
    {
        public enum Type
        {
            none = -1,
            normal,
            endless,
            rescueMission,
            special,
        }

        [Header("Basic Settings")]
        [SerializeField] private Type battleType;
        [SerializeField] private bool avoidPlayerDefinedLegionSet;
        [SerializeField] private bool avoidResonance;
        [Header("Prizes")]
        [SerializeField] private List<Prize> firstTimePrize;
        [SerializeField] private List<Prize> prizes;
        [Header("Enemy and 3rd party")]
        [SerializeField] private Player enemy;
        [SerializeField] private List<Player> thirdParties = new List<Player>();
        [Header("Wave Info")]
        [SerializeField] private List<WaveInfo> waveInfo = new List<WaveInfo>();
        [Header("Map Prefab")]
        [SerializeField] private GameObject mapPrefab;

        [Header("In-battle infomations")]
        [Header("======================================")]
        [SerializeField] private Player player;
        [SerializeField] private Round round;
        [SerializeField] private Stat stat;
        [SerializeField] private CentralDeck centralDeck;
        [SerializeField] private List<BattleArmy> armies = new List<BattleArmy>();
        [SerializeField] private List<BattleBuilding> buildings = new List<BattleBuilding>();
        [SerializeField] private StatusList globalStatus = new List<Status>();

        [SerializeField] private List<Animator> ongoingAnimation = new List<Animator>();
        [SerializeField] private List<Effect> passedEffect = new List<Effect>();
        [SerializeField] private ScoreBoard scoreBoard;

        [NonSerialized] protected WaveControl waveControl;
        [NonSerialized] protected MapEntity mapEntity;


        #region Properties  
        public string Name => player.Name + " vs." + enemy.Name;
        public bool AvoidPlayerLegion => avoidPlayerDefinedLegionSet;
        public bool AvoidResonance => avoidResonance;
        public Type BattleType { get => battleType; set => battleType = value; }
        public Player Enemy { get => enemy; set => enemy = value; }
        public List<Player> ThirdParties => thirdParties;
        public Round Round { get => round; set => round = value; }
        public Stat CurrentStat { get => stat; set { stat = value; } }
        public Player Player { get => player; set => player = value; }
        public List<BattleArmy> Armies => armies;
        public List<BattleBuilding> Buildings => buildings;
        public ScoreBoard ScoreBoard { get => scoreBoard; set => scoreBoard = value; }
        public List<Animator> OngoingAnimation => ongoingAnimation;
        public List<Effect> PassingEffect { get => passedEffect; set => passedEffect = value; }
        public StatusList GlobalStatus { get => globalStatus; set => globalStatus = value; }
        public CentralDeck CentralDeck { get => centralDeck; set => centralDeck = value; }
        public List<Prize> FirstTimePrize { get => firstTimePrize; set => firstTimePrize = value; }
        public List<Prize> Prizes { get => prizes; set => prizes = value; }
        public GameObject MapPrefab { get => mapPrefab; set => mapPrefab = value; }
        public MapEntity MapEntity { get => mapEntity; set => mapEntity = value; }
        public Map Map => mapEntity.data;
        public WaveControl WaveControl => waveControl;

        public List<Player> AllPlayers => new List<Player> { Player, Enemy }.Union(ThirdParties).ToList();
        public List<Player> OtherPlayers => new List<Player> { Enemy }.Union(ThirdParties).ToList();
        public List<IStatusContainer> StatusContainers => AllPlayers.OfType<IStatusContainer>().Union(Armies.OfType<IStatusContainer>()).Union(Buildings.OfType<IStatusContainer>()).Union(Map.OfType<IStatusContainer>()).ToList();

        StatusList IStatusContainer.StatList => GlobalStatus;
        StatusList IStatusContainer.GetAllStatus() => GetAllStatus();

        private StatusList GetAllStatus()
        {
            List<Status> stats = new List<Status>();
            foreach (var item in StatusContainers)
            {
                stats.AddRange(item.StatList);
            }
            stats.AddRange(globalStatus);
            return stats;
        }
        #region Minor Properties
        public bool IsFreeTime => ((CurrentStat == Stat.waitForAnimationEnd || CurrentStat == Stat.normal) && Round.CurrentStat == Round.Stat.normal) || (CurrentStat == Stat.begin && Round.CurrentStat == Round.Stat.gameStart);
        public bool HasAnimation => !TryEndAnimation();


        #endregion

        #endregion

        private Battle() { }

        #region Get Battle object
        /// <summary> Get a player by its uuid </summary>
        /// <param name="UUID">uuid</param>
        /// <returns> a player, if not found, return a null </returns>
        public Player GetPlayer(UUID UUID)
        {
            foreach (Player item in AllPlayers)
            {
                if (item.UUID == UUID)
                {
                    return item;
                }
            }

            if (UUID == UUID.Player)
            {
                return Game.CurrentBattle.player;
            }
            return null;
        }

        /// <summary> Get a player's armies </summary>
        /// <param name="player"> player </param>
        /// <returns> list of armies </returns>
        public List<BattleArmy> GetArmies(Player player)
        {
            List<BattleArmy> battleArmies = new List<BattleArmy>();
            foreach (BattleArmy item in Armies)
            {
                if (item.Owner == player)
                {
                    battleArmies.Add(item);
                }
            }
            return battleArmies;
        }
        public List<BattleArmy> GetArmies(Vector2Int position)
        {
            List<BattleArmy> armies = new List<BattleArmy>();
            foreach (BattleArmy item in Armies)
            {
                if (item.Coordinate == position)
                {
                    armies.Add(item);
                }
            }
            return armies;
        }

        public BattleArmy GetArmy(Vector2Int position)
        {
            foreach (BattleArmy item in Armies)
            {
                if (item.Coordinate == position)
                {
                    return item;
                }
            }
            return null;
        }
        public BattleArmy GetArmy(UUID uUID)
        {
            foreach (BattleArmy item in Armies)
            {
                if (item.UUID == uUID)
                {
                    return item;
                }
            }
            return null;
        }


        /// <summary> Get a player's buildings </summary>
        /// <param name="player"> player </param>
        /// <returns> list of buildings </returns>
        public List<BattleBuilding> GetBuildings(Player player)
        {
            List<BattleBuilding> buildings = new List<BattleBuilding>();
            foreach (BattleBuilding item in Buildings)
            {
                if (item.Owner == player)
                {
                    buildings.Add(item);
                }
            }
            return buildings;

        }
        public List<BattleBuilding> GetBuildings(Vector2Int position)
        {
            List<BattleBuilding> buildings = new List<BattleBuilding>();
            foreach (BattleBuilding item in Buildings)
            {
                if (item.Coordinate == position)
                {
                    buildings.Add(item);
                }
            }
            return buildings;
        }

        public BattleBuilding GetBuilding(Vector2Int position)
        {
            foreach (BattleBuilding item in Buildings)
            {
                if (item.Coordinate == position)
                {
                    return item;
                }
            }
            return null;
        }
        public BattleBuilding GetBuilding(UUID uUID)
        {
            foreach (BattleBuilding item in Buildings)
            {
                if (item.UUID == uUID)
                {
                    return item;
                }
            }
            return null;
        }


        #endregion


        #region Preparation
        /// <summary> 
        /// prepare the battle (generate entities)
        /// </summary>
        public void Prepare()
        {
            //other players
            foreach (Player item in OtherPlayers) { if (item.UUID == UUID.Empty) item.NewUUID(); }

            Initialize();
            InBeginning();

            {
                //geneate playerEntity
                BattleUI.instance.CreatePlayerEntity(player);
                //generate AI
                foreach (Player item in OtherPlayers) BattleUI.instance.CreateAI(item);
            }

            Debug.Log(player.BattleArmies.Count);
            if (!AvoidPlayerLegion) player.CreateArmyCard();

            #region Pvp
            if (Game.Configuration.PvP)
            {
                LegionSet legionSet = new LegionSet(Game.PlayerData.Legions[1], Game.PlayerData.EventCardPiles[0], Game.PlayerData.Leaders[0].UUID, " ");
                enemy = new Player("Anexar", legionSet, enemy.UUID);

                if (!AvoidPlayerLegion)
                {
                    enemy.SetupLeader(legionSet.Leader);
                    enemy.SetupLegion(legionSet.Legion);
                    enemy.SetupEventCardPile(legionSet.EventCardPile);
                    enemy.CreateArmyCard();
                }

                waveControl.GenerateBuildingOnly(1);
                return;
            }

            #endregion

            waveControl.GenerateEnemy(1);
        }

        /// <summary>
        /// initialize battle
        /// </summary>
        public void Initialize()
        {
            centralDeck = centralDeck ?? new CentralDeck();

            ScoreBoard = new ScoreBoard();
            waveControl = new WaveControl(waveInfo, this);
            round = new Round(this);
        }

        /// <summary> 
        /// Start the Battle
        /// </summary>
        public void Start()
        {
            if (!AvoidResonance) ResonateArmies();

            Round.TurnBegin();
            foreach (Player player in AllPlayers)
            {
                GetHandCard(player, Effect.Types.enterMove, player.MaxHandCardCount);
                Round.CurrentPlayer.RefillAction();
            }
            Round.Normal();
            InNormal();
        }

        private void ResonateArmies()
        {
            if (!AvoidResonance)
            {
                Resonance.Resonate(player.BattleArmies);
                Resonance.Resonate(enemy.BattleArmies);
            }
        }

        public void SetPlayer(LegionSet playerLegion)
        {
            player = new Player("Canute Svensson", playerLegion);
            if (!AvoidPlayerLegion)
            {
                player.SetupLeader(playerLegion.Leader);
                player.SetupLegion(playerLegion.Legion);
                player.SetupEventCardPile(playerLegion.EventCardPile);
            }
        }

        #endregion


        #region 状态机
        public enum Stat
        {
            begin,
            normal,
            attack,
            move,
            waitForAnimationEnd,
            win,
            lose,
        }

        /// <summary>
        /// a global player operation event stat, not turn stat
        /// </summary> 
        public bool TryEndAnimation()
        {
            //check animation actually ended
            bool animationEnded = ongoingAnimation.Count == 0;
            if (animationEnded)
            {
                if (round.CurrentStat == Round.Stat.gameStart)
                {
                    InBeginning();
                }
                else switch (CurrentStat)
                    {
                        case Stat.attack:
                        case Stat.move:
                            break;
                        default:
                            InNormal();
                            break;
                    }
                return true;
            }
            return false;
        }

        /// <summary> tell battle is it the beginning of the battle </summary>
        public Stat InBeginning() => CurrentStat = Stat.begin;
        /// <summary> tell battle to show a normal stat </summary>
        public Stat TryInNormal() => CurrentStat = ongoingAnimation.Count != 0 ? Stat.waitForAnimationEnd : Stat.normal;

        public Stat InNormal() => CurrentStat = Stat.normal;

        /// <summary> tell battle a player is attacking </summary>
        public Stat InAttackAction() => CurrentStat = Stat.attack;

        /// <summary> tell battle a player is moving army </summary>
        public Stat InMotionAction() => CurrentStat = Stat.move;

        public Round.Stat InAction(SelectEvent selectEvent)
        {
            Entity.SelectEvent += selectEvent;
            return Round.InAction();
        }

        public Round.Stat EndAction(SelectEvent selectEvent)
        {
            Entity.SelectEvent -= selectEvent;
            return Round.Normal();
        }

        /// <summary> tell battle there is an animation going on </summary>
        public Stat InAnimation() => CurrentStat = Stat.waitForAnimationEnd;

        /// <summary> tell battle the battle is end </summary>
        private Stat InWinning() => stat = Stat.win;

        /// <summary> tell battle the battle is end </summary>
        private Stat InLosing() => stat = Stat.lose;

        public void CancelAction()
        {
            if (CurrentStat == Stat.waitForAnimationEnd)
            {
                return;
            }

            Card lastCard = Card.LastCard;

            lastCard.BackActionPoint();
            CardEntity.Create(lastCard);
            Card.LastCard = null;
            EffectExecute.CancelSelectTargetEvent();

            if (CurrentStat == Stat.attack)
            {
                ArmyAttack.RemoveAttackEvent();
            }
            if (CurrentStat == Stat.move)
            {
                ArmyMovement.RemoveMoveEvent();
            }

            OnMapEntity.SelectingEntity.Exist()?.Select();
            OnMapEntity.SelectingEntity.Exist()?.Unselect();

            InNormal();
            Round.Normal();
        }
        #endregion


        #region 回合控制
        public bool TryEndTurn()
        {
            if (Round.CurrentStat == Round.Stat.normal && CurrentStat == Stat.normal)
            {
                EndTurn();
                return true;
            }
            return false;
        }

        /// <summary> 结束现在的回合 </summary>
        public void EndTurn()
        {
            Round.TurnEnd();
            Round.CurrentPlayer.StatTurnDecay();
            Round.CurrentPlayer.EndTurnDiscard();
            BuildingOccupy();
            NextPlayer();
        }

        private void BuildingOccupy()
        {
            foreach (var item in buildings)
            {
                if (item.OnCellOf.HasArmyStandOn)
                {
                    if (item.Owner != Round.NextPlayer)
                    {
                        item.Owner = Round.NextPlayer;
                    }
                }
            }
        }

        /// <summary> 切换到下一个玩家 </summary>
        public void NextPlayer()
        {
            if (Game.Configuration.PvP)
                BattleUI.ChangePlayerUI(Round.CurrentPlayer, false);

            Round.Next();

            if (Game.Configuration.PvP)
                BattleUI.ChangePlayerUI(Round.CurrentPlayer, true);

            NewTurn();
        }

        /// <summary> 处理玩家的到新的回合之后的事件 </summary>
        public void NewTurn()
        {
            Round.TurnBegin();
            Round.CurrentPlayer.RefillCard();
            Round.CurrentPlayer.RefillAction();

            InNormal();
            Round.Normal();

            if (Round.CurrentPlayer.Entity is AIEntity)
            {
                Round.CurrentPlayer.Entity.Action((Round.CurrentPlayer.Entity as AIEntity).Run);
            }

            if (Player.IsInTurn)
            {
                BattleUI.SetPlayerDownBarsActive(true);
            }
        }

        #endregion

        /// <summary> 获取手牌 </summary>
        /// <param name="player"></param>
        /// <param name="count"></param>
        public List<CardEntity> GetHandCard(Player player, int count)
        {
            List<CardEntity> ret = new List<CardEntity>();
            Debug.Log(player.Name + ", " + count);
            for (int i = 0; i < count; i++)
            {
                Card card = centralDeck.DrawCard();
                card.Owner = player;
                ret.Add(CardEntity.Create(card));
            }
            Debug.Log(player.HandCard.Count);
            return ret;
        }

        public List<CardEntity> GetHandCard(Player player, Effect.Types types, int count)
        {
            List<CardEntity> ret = new List<CardEntity>();
            Debug.Log(player.Name + ", " + count);

            for (int i = 0; i < count; i++)
            {
                Card card = centralDeck.DrawCard(types);
                card.Owner = player;
                ret.Add(CardEntity.Create(card));

            }

            Debug.Log(player.HandCard.Count);
            return ret;
        }


        #region End of the Battle

        public void EndCheck()
        {
            if (stat == Stat.begin || Round.CurrentStat == Round.Stat.gameEnd) { return; }

            switch (battleType)
            {
                case Type.normal:
                    if (Enemy.BattleArmies.Count == 0)
                        if (Round.wave < waveInfo.Count)
                            waveControl.NextWave();
                        else
                            Win();
                    if (Player.BattleArmies.Count == 0)
                        Lost();
                    break;
                case Type.endless:
                    if (Enemy.BattleArmies.Count == 0)
                        waveControl.NextWave();
                    if (Player.BattleArmies.Count == 0)
                        Lost();
                    break;
                case Type.rescueMission:
                    //unknown condition...
                    break;
            }
        }

        public void Win()
        {
            InWinning();
            Round.GameEnd();
            Debug.Log("player win");

            FulfillPrize();

            Game.CurrentLevel.OpenEndStory();
            BattleUI.ShowEndUI();
        }

        public void Lost()
        {
            InLosing();
            Round.GameEnd();

            BattleUI.ShowEndUI();
            Debug.Log("player lost");
        }

        public void EndlessEnd()
        {
            InWinning();
            Round.GameEnd();
            Debug.Log("player win");

            FulfillPrize();
            Game.CurrentLevel.OpenEndStory();
            BattleUI.ShowEndUI();
        }

        public void FulfillPrize()
        {
            if (Player.LegionSet.Legion != null) foreach (var prize in prizes) { prize.Fulfill(Player.LegionSet.Legion.Armies); }
            IEnumerable<Prize> enumerable = prizes.Where((p) => p.PrizeType != Item.Type.currency);
            foreach (var prize in enumerable) { prize.Fulfill(); }
            if (!Game.CurrentLevel.IsPassed) foreach (var prize in firstTimePrize) { prize.Fulfill(); }

            int score = ScoreBoard.GetScore();

            var fgPrize = new Prize(Currency.Type.fedgram.ToString(), score / 400, Item.Type.currency);
            var mpPrize = new Prize(Currency.Type.manpower.ToString(), score / 400, Item.Type.currency);
            var maPrize = new Prize(Currency.Type.mantleAlloy.ToString(), (int)(ScoreBoard.TotalAirforceDefeated * (UnityEngine.Random.value + 1) / 1.5f), Item.Type.currency);

            fgPrize.Parameter += Game.CurrentBattle.Prizes.GetCurrencyCount(Currency.Type.fedgram);
            mpPrize.Parameter += Game.CurrentBattle.Prizes.GetCurrencyCount(Currency.Type.manpower);
            maPrize.Parameter += Game.CurrentBattle.Prizes.GetCurrencyCount(Currency.Type.mantleAlloy);

            if (!Game.CurrentLevel.IsPassed)
            {
                fgPrize.Parameter += Game.CurrentBattle.FirstTimePrize.GetCurrencyCount(Currency.Type.fedgram);
                mpPrize.Parameter += Game.CurrentBattle.FirstTimePrize.GetCurrencyCount(Currency.Type.manpower);
                maPrize.Parameter += Game.CurrentBattle.FirstTimePrize.GetCurrencyCount(Currency.Type.mantleAlloy);
            }

            Debug.Log(fgPrize.Parameter);

            fgPrize.Fulfill();
            mpPrize.Fulfill();
            maPrize.Fulfill();

            #region Display Info 

            BattleUI.EndUI.ShowPrize(fgPrize.Parameter, mpPrize.Parameter, maPrize.Parameter);
            #endregion
        }

        public static void End() => Game.ClearBattle();

        #endregion

        object ICloneable.Clone()
        {
            return Clone();
        }

        public Battle Clone()
        {
            Battle battleCopy = new Battle
            {
                battleType = battleType,
                avoidPlayerDefinedLegionSet = avoidPlayerDefinedLegionSet,
                avoidResonance = avoidResonance,

                prizes = prizes.Clone(),
                firstTimePrize = firstTimePrize.Clone(),

                enemy = enemy.Clone() as Player,
                thirdParties = thirdParties.Clone(),

                waveInfo = waveInfo.Clone(),

                //.....................................
                player = Player.Clone() as Player,

                armies = armies.Clone() ?? new List<BattleArmy>(),
                buildings = buildings.Clone() ?? new List<BattleBuilding>(),

                mapPrefab = mapPrefab,
            };
            return battleCopy;
        }

    }

    [Serializable]
    public class ScoreBoard : Args
    {
        [SerializeField] private List<Effect> allPassedEffect;
        [SerializeField] private int totalPlayerDamage = 0;
        [SerializeField] private int totalAirforceDefeated = 0;
        [SerializeField] private int totalLandArmyDefeated = 0;


        public List<Effect> AllPassedEffect { get => allPassedEffect; set => allPassedEffect = value; }
        public int TotalEnemyDefeated => TotalAirforceDefeated + TotalLandArmyDefeated;
        public int TotalAirforceDefeated { get => totalAirforceDefeated; }
        public int TotalLandArmyDefeated { get => totalLandArmyDefeated; }

        public ScoreBoard()
        {
            AllPassedEffect = new List<Effect>();
            PassiveEntities.DamageEvent += CountPlayerDamage;
            PassiveEntities.DefeatEvent += CountPlayerDefeated;
        }

        public void CountPlayerDamage(IBattleableEntity source, IPassiveEntity target, int value)
        {
            //Debug.Log("Counting player damage");

            if (source is null)
            {
                //Debug.Log("No source");
                return;
            }

            if (source.Owner == Game.CurrentBattle.Player)
            {
                totalPlayerDamage += value;
                return;
            }
            //Debug.Log("Not from player");
        }
        public void CountPlayerDefeated(IBattleableEntity source, IPassiveEntity target)
        {
            if (source is null)
            {
                return;
            }

            if (source.Owner == Game.CurrentBattle.Player)
            {
                switch (target.StandPostion)
                {
                    case BattleProperty.Position.land:
                        totalLandArmyDefeated++;
                        break;
                    case BattleProperty.Position.air:
                        totalAirforceDefeated++;
                        break;
                }
            }
        }

        public int GetScore()
        {
            return (Game.CurrentBattle.Round.wave - 1) * 10000 + totalPlayerDamage * 5 + TotalEnemyDefeated * 1000;
        }
    }
}
