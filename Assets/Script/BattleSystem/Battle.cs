using Canute.BattleSystem.UI;
using Canute.Shops;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Canute.BattleSystem
{
    [Serializable]
    public class Battle : ICloneable
    {
        [Header("Settings")]
        [SerializeField] protected Player enemy;
        [SerializeField] protected List<Player> thirdParties = new List<Player>();
        [SerializeField] protected List<WaveInfo> waveInfo = new List<WaveInfo>();
        [SerializeField] protected BattlePrize prize;
        [SerializeField] protected GameObject mapPrefab;

        [Header("In-battle infomations")]
        [SerializeField] protected Player player;
        [SerializeField] protected Round round;
        [SerializeField] protected Stat stat;
        [SerializeField] protected CentralDeck centralDeck;
        [SerializeField] protected List<Animator> ongoingAnimation = new List<Animator>();

        [SerializeField] protected List<BattleArmy> armies = new List<BattleArmy>();
        [SerializeField] protected List<BattleBuilding> buildings = new List<BattleBuilding>();
        [SerializeField] protected List<Status> globalStatus = new List<Status>();

        [SerializeField] protected List<Effect> passedEffect = new List<Effect>();
        [SerializeField] protected MapEntity mapEntity;


        #region Properties 

        public Player Enemy { get => enemy; set => enemy = value; }
        public List<Player> ThirdParties => thirdParties;
        public Round Round { get => round; set => round = value; }
        public Stat CurrentStat { get => stat; set { stat = value; } }
        public Player Player { get => player; set => player = value; }
        public List<BattleArmy> Armies => armies;
        public List<BattleBuilding> Buildings => buildings;
        public List<Animator> OngoingAnimation => ongoingAnimation;
        public List<Effect> PassingEffect { get => passedEffect; set => passedEffect = value; }
        public CentralDeck CentralDeck { get => centralDeck; set => centralDeck = value; }
        public GameObject MapPrefab { get => mapPrefab; set => mapPrefab = value; }
        public MapEntity MapEntity { get => mapEntity; set => mapEntity = value; }
        public Map Map => mapEntity.data;

        public List<Player> AllPlayers => new List<Player> { Player, Enemy }.Union(ThirdParties).ToList();
        public List<Player> OtherPlayers => new List<Player> { Enemy }.Union(ThirdParties).ToList();
        public List<IStatusContainer> StatusContainers => AllPlayers.OfType<IStatusContainer>().Union(Armies.OfType<IStatusContainer>()).Union(Buildings.OfType<IStatusContainer>()).Union(Map.OfType<IStatusContainer>()).ToList();

        #region Minor Properties
        public bool IsFreeTime => (CurrentStat == Stat.normal && Round.CurrentStat == Round.Stat.normal) || (CurrentStat == Stat.begin && Round.CurrentStat == Round.Stat.gameStart);
        public bool HasAnimation => TryEndAnimation();

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
            //Player
            foreach (Player item in OtherPlayers)
            {
                if (item.UUID == UUID.Empty)
                {
                    item.NewUUID();
                }
                Debug.Log(item.Name);
            }

            round = new Round(this);
            centralDeck = new CentralDeck();
            InBeginning();

            //geneate playerEntity
            BattleUI.instance.CreatePlayerEntity(player);

            //generate AI
            foreach (Player item in OtherPlayers)
            {
                BattleUI.instance.CreateAI(item);
                Debug.Log(item.Name);
            }

            //generate player's army card
            for (int i = 0; i < Player.BattleArmies.Count; i++)
            {
                BattleArmy item = Player.BattleArmies[i];
                Card card = new Card(Card.Types.eventCard, Career.none, new Effect(Effect.Types.createArmy, 1, i), 0, TargetType.cellEntity) { Owner = player };
                card.Effect.SetParam("name", item.Name);
                ArmyCardEntity.Create(card, item);
            }

            if (Game.Configuration.PvP)
            {
                enemy = new Player("Anexar", new LegionSet(Game.PlayerData.Legions[1], Game.PlayerData.EventCardPiles[0], Game.PlayerData.Leaders[0].UUID, " "), enemy.UUID);

                //generate player's army card
                for (int i = 0; i < Enemy.BattleArmies.Count; i++)
                {
                    BattleArmy item = Enemy.BattleArmies[i];
                    Card card = new Card(Card.Types.eventCard, Career.none, new Effect(Effect.Types.createArmy, 1, i), 0, TargetType.cellEntity) { Owner = enemy };
                    card.Effect.SetParam("name", item.Name);
                    ArmyCardEntity.Create(card, item);
                }
            }


            GenerateEnemy(1);
        }

        public void GenerateEnemy(int wave)
        {
            //generate first wave buildings
            foreach (BattleBuilding item in waveInfo[wave - 1].BattleBuildings)
            {
                buildings.Add(item);
                BuildingEntity.Create(item);
            }

            if (Game.Configuration.PvP)
            {
                return;
            }

            //generate first wave armies
            foreach (BattleArmy item in waveInfo[wave - 1].BattleArmies)
            {
                armies.Add(item);
                ArmyEntity.Create(item);
            }
        }


        /// <summary> 
        /// Start the Battle
        /// </summary>
        public void Start()
        {
            Round.TurnBegin();
            foreach (Player player in AllPlayers)
            {
                GetHandCard(player, Effect.Types.enterMove, 5);
                Round.CurrentPlayer.RefillAction();
            }
            Round.Normal();
            InNormal();
        }

        public void SetPlayer(LegionSet playerLegion)
        {
            player = new Player("Canute Svensson", playerLegion);
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
        public Stat InWinning() => stat = Stat.win;

        /// <summary> tell battle the battle is end </summary>
        public Stat InLosing() => stat = Stat.lose;

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
            NextPlayer();
        }

        /// <summary> 切换到下一个玩家 </summary>
        public void NextPlayer()
        {
            BattleUI.SetPlayerUI(Round.CurrentPlayer, false);
            Round.Next();
            BattleUI.SetPlayerUI(Round.CurrentPlayer, true);
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

            if (Player.IsInTurn)
            {
                BattleUI.SetDownBarsActive(true);
            }
        }

        #endregion

        /// <summary> 获取手牌 </summary>
        /// <param name="player"></param>
        /// <param name="count"></param>
        public void GetHandCard(Player player, int count)
        {
            Debug.Log(player.Name + ", " + count);
            for (int i = 0; i < count; i++)
            {
                Card card = centralDeck.DrawCard();
                card.Owner = player;
                CardEntity.Create(card);
            }
            Debug.Log(player.HandCard.Count);
        }

        public void GetHandCard(Player player, Effect.Types types, int count)
        {
            Debug.Log(player.Name + ", " + count);

            for (int i = 0; i < count; i++)
            {
                Card card = centralDeck.DrawCard(types);
                card.Owner = player;
                CardEntity.Create(card);
            }

            Debug.Log(player.HandCard.Count);
        }
        #region End of the Battle

        public void EndCheck()
        {
            if (stat == Stat.begin || Round.CurrentStat == Round.Stat.gameEnd)
            {
                return;
            }

            if (Enemy.BattleArmies.Count == 0)
            {
                if (Round.wave < waveInfo.Count)
                {
                    NextWave();
                }
                else
                {
                    Win();
                }
            }
            if (Player.BattleArmies.Count == 0)
            {
                Lost();
            }
        }

        private void NextWave()
        {
            Round.wave++;
            GenerateEnemy(round.wave);
        }

        public void Win()
        {
            InWinning();
            Round.GameEnd();
            Debug.Log("player win");
            foreach (ArmyItem item in Player.Legion.Armies)
            {
                item.AddFloatExp(prize.floatExp);
                Debug.Log("Add float exp into army");
            }

            BattleUI.EndUI.gameObject.SetActive(true);
        }

        public void Lost()
        {
            InLosing();
            Round.GameEnd();
            Debug.Log("player lost");
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
                Enemy = enemy.Clone() as Player,
                Player = Player.Clone() as Player,
                thirdParties = thirdParties.Clone(),
                waveInfo = waveInfo.Clone(),
                armies = new List<BattleArmy>(),
                buildings = new List<BattleBuilding>(),
                MapPrefab = MapPrefab,

            };
            return battleCopy;
        }

    }

    [Serializable]
    public class WaveInfo : ICloneable
    {
        [SerializeField] private List<BattleArmy> battleArmies;
        [SerializeField] private List<BattleBuilding> battleBuildings;

        public List<BattleArmy> BattleArmies { get => battleArmies; set => battleArmies = value; }
        public List<BattleBuilding> BattleBuildings { get => battleBuildings; set => battleBuildings = value; }

        public object Clone()
        {
            WaveInfo waveInfoCopy = new WaveInfo
            {
                battleArmies = battleArmies.Clone(),
                battleBuildings = battleBuildings.Clone()
            };
            return waveInfoCopy;
        }
    }

    [Serializable]
    public struct BattlePrize
    {
        public Currency goldPrize;
        public Currency popPrize;
        public int floatExp;

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(BattlePrize left, BattlePrize right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(BattlePrize left, BattlePrize right)
        {
            return !(left == right);
        }
    }


    public static class BattleAnimations
    {
        public static void AddToBattle(this Animator animator)
        {
            Game.CurrentBattle.OngoingAnimation.Add(animator);
            Game.CurrentBattle.InAnimation();
        }

        public static void RemoveFromBattle(this Animator animator)
        {
            Game.CurrentBattle.OngoingAnimation.Remove(animator);
            Game.CurrentBattle.TryEndAnimation();
        }

        public static bool IsDone(this Animator animator)
        {
            return animator.GetBool("isIdle");
        }

        public static bool TryRemoveFromBattle(this Animator animator)
        {
            bool isDone = animator.IsDone();
            if (isDone)
            {
                animator.RemoveFromBattle();
            }
            return isDone;
        }
    }
}
