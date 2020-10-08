using Canute.BattleSystem;
using Canute.LevelTree;
using Canute.Module;
using Canute.Shops;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{
    /// <summary>
    /// 玩家数据
    /// </summary>
    //[CreateAssetMenu(fileName = "Game Data", menuName = "Game Data/Playexr Data", order = 2)]
    [Obsolete("Since scriptable Object cannot store anything in runtime, new storage system PlayerFile replaced it")]
    public class PlayerData : ScriptableObject
    {
        public WorldTime playerLastOperationTime;


        #region Curency
        [Header("Player Countable Item")]
        [SerializeField] private int gold;
        [SerializeField] private int manpower;
        [SerializeField] private int mantleAlloy;
        [Space]
        [SerializeField] private int mantleFluid;
        [SerializeField] private int aethium;

        public int Gold { get => gold; set => gold = value; }
        public int Manpower { get => manpower; set => manpower = value; }
        public int MantleAlloy { get => mantleAlloy; set => mantleAlloy = value; }
        public int MantleFluid { get => mantleFluid; set => mantleFluid = value; }
        public int Aethium { get => aethium; set => aethium = value; }

        public bool Spent(Currency.Type type, int amount)
        {
            switch (type)
            {
                case Currency.Type.fedgram:
                    if (Gold < amount)
                    {
                        Gold -= amount;
                        return true;
                    }
                    return false;
                case Currency.Type.manpower:
                    if (Manpower < amount)
                    {
                        Manpower -= amount;
                        return true;
                    }
                    return false;
                case Currency.Type.mantleAlloy:
                    if (MantleAlloy < amount)
                    {
                        MantleAlloy -= amount;
                        return true;
                    }
                    return false;
                case Currency.Type.aethium:
                    if (Aethium < amount)
                    {
                        Aethium -= amount;
                        return true;
                    }
                    return false;
                default:
                    return false;
            }
        }
        public bool Spent(Currency currency)
        {
            return Spent(currency.name, currency.count);
        }
        #endregion

        #region Legion
        [Header("Player Legions")]
        // 军团，用户默认军队设置储存
        // ......
        public List<Legion> Legions = new List<Legion>(3);
        #endregion

        #region Items
        [Header("Player Items")]
        [SerializeField] protected List<ArmyItem> armies = new List<ArmyItem>();
        [SerializeField] protected List<LeaderItem> leaders = new List<LeaderItem>();
        [SerializeField] protected List<EquipmentItem> equipments = new List<EquipmentItem>();
        [SerializeField] protected List<EventCardItem> eventCards = new List<EventCardItem>();

        public List<ArmyItem> Armies => armies;
        public List<LeaderItem> Leaders => leaders;
        public List<EquipmentItem> Equipments => equipments;
        public List<EventCardItem> EventCards => eventCards;
        #endregion

        #region Player's Chapter Progress
        [Header("Player's Chapter Progress")]
        [SerializeField] protected PlayerChapterTree gameProgree;
        public PlayerChapterTree PlayerChapterTreeStat { get => gameProgree; set => gameProgree = value; }
        #endregion

        #region Unlocked
        [Header("Statistic")]
        public GameStatistic gameStatistic;

        public List<string> ArmiesUnlocked => gameStatistic.armiesUnlocked;
        public CheckList ArmiesUnlockedList => new CheckList(GameData.Prototypes.TestingArmies, ArmiesUnlocked);
        public List<string> LeadersUnlocked => gameStatistic.leadersUnlocked;
        public CheckList LeadersUnlockedList => new CheckList(GameData.Prototypes.TestingEquipments, EquipmentsUnlocked);
        public List<string> EquipmentsUnlocked => gameStatistic.equipmentsUnlocked;
        public CheckList EquipmentsUnlockedList => new CheckList(GameData.Prototypes.TestingLeaders, LeadersUnlocked);
        public CheckList EventCardUnlocked => GetEventTree();


        public bool IsArmyUnlocked(string name)
        {
            return ArmiesUnlocked.Contains(name);
        }

        public bool IsArmiesUnlocked(string[] names)
        {
            foreach (string item in names)
            {
                if (!ArmiesUnlocked.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsLeaderUnlocked(string name)
        {
            return LeadersUnlocked.Contains(name);
        }

        public bool IsLeadersUnlocked(string[] names)
        {
            foreach (string item in names)
            {
                if (!LeadersUnlocked.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        public EventTree GetEventTree()
        {
            List<EventCard> Cards = new List<EventCard>();
            foreach (EventCardItem item in EventCards)
            {
                Cards.Add(GameData.Prototypes.GetEventCardPrototype(item.Name));
            }

            return new EventTree(Cards);
        }

        #endregion

        #region Find Player Item
        public bool HaveLeader(string name)
        {
            return !(GetLeaderItem(name) is null);
        }

        public LeaderItem GetLeaderItem(string name)
        {
            foreach (LeaderItem item in leaders)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }

            return null;
        }

        public List<LeaderItem> GetLeaderItems(string[] uuids)
        {
            List<LeaderItem> items = new List<LeaderItem>();
            foreach (string uuid in uuids)
            {
                LeaderItem item = GetLeaderItem(uuid);
                if (item is null)
                {
                    continue;
                }

                items.Add(item);
            }

            return items;
        }

        public LeaderItem GetLeaderItem(UUID uuid)
        {
            foreach (LeaderItem item in leaders)
            {
                if (item.UUID == uuid)
                {
                    return item;
                }
            }

            return null;
        }

        public List<LeaderItem> GetLeaderItems(UUID[] uuids)
        {
            List<LeaderItem> items = new List<LeaderItem>();
            foreach (UUID uuid in uuids)
            {
                LeaderItem item = GetLeaderItem(uuid);
                if (item is null)
                {
                    continue;
                }

                items.Add(item);
            }

            return items;
        }

        public ArmyItem GetArmyItem(UUID uuid)
        {
            foreach (ArmyItem item in armies)
            {
                if (item.UUID == uuid)
                {
                    return item;
                }
            }

            return null;
        }

        public List<ArmyItem> GetArmyItems(params UUID[] uuids)
        {
            List<ArmyItem> items = new List<ArmyItem>();
            foreach (UUID uuid in uuids)
            {
                ArmyItem item = GetArmyItem(uuid);
                if (item is null)
                {
                    continue;
                }

                items.Add(item);
            }

            return items;
        }

        public EquipmentItem GetEquipmentItem(UUID uuid)
        {
            foreach (EquipmentItem item in equipments)
            {
                if (item.UUID == uuid)
                {
                    return item;
                }
            }

            return null;
        }

        public List<EquipmentItem> GetEquipmentItems(UUID[] uuids)
        {
            List<EquipmentItem> items = new List<EquipmentItem>();
            foreach (UUID uuid in uuids)
            {
                EquipmentItem item = GetEquipmentItem(uuid);
                if (item is null)
                {
                    continue;
                }

                items.Add(item);
            }

            return items;
        }

        public EventCardItem GetEventCardItem(UUID uuid)
        {
            foreach (var item in eventCards)
            {
                if (item.UUID == uuid)
                {
                    return item;
                }
            }

            return null;
        }

        public List<EventCardItem> GetEventCardItems(UUID[] uuids)
        {
            List<EventCardItem> items = new List<EventCardItem>();
            foreach (UUID uuid in uuids)
            {
                EventCardItem item = GetEventCardItem(uuid);
                if (item is null)
                {
                    continue;
                }

                items.Add(item);
            }

            return items;
        }


        #endregion

        #region Give Player Item
        public void AddArmyItem(ArmyItem item)
        {
            if (!ArmiesUnlocked.Contains(item.Name))
            {
                ArmiesUnlocked.Add(item.Name);
            }

            armies.Add(item);
        }

        public void AddLeader(LeaderItem item)
        {
            leaders.Add(item);
        }

        public void AddEquipment(EquipmentItem item)
        {
            if (!EquipmentsUnlocked.Contains(item.Name))
            {
                EquipmentsUnlocked.Add(item.Name);
            }

            equipments.Add(item);
        }

        #endregion

        private PlayerData()
        {
        }


        //private void OnEnable()
        //{
        //    if (!GameData.PlayerDatas.Contains(this))
        //    {
        //        GameData.PlayerDatas.Add(this);
        //    }

        //    playerLastOperationTime = DateTime.UtcNow;
        //}
    }



    [Serializable]
    public class LevelInfo : INameable, IEquatable<LevelInfo>
    {
        [SerializeField] protected string name;
        [SerializeField] protected bool isPassed = true;

        public LevelInfo(string name, bool isPassed)
        {
            this.name = name;
            this.isPassed = isPassed;
        }
        public LevelInfo()
        {
        }

        public string Name => name;
        public bool IsPassed => isPassed;
        public Level Level => GameData.Levels.GetLevel(Name);

        public static implicit operator bool(LevelInfo levelInfo)
        {
            return !(levelInfo.Level is null);
        }

        public bool Equals(LevelInfo other)
        {
            return other.name == name ? (other.isPassed && isPassed) : false;
        }
    }

    [Serializable]
    public class PlayerChapterTree : DataList<LevelInfo> { }
}