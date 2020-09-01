using Canute.BattleSystem;
using Canute.ExplorationSystem;
using Canute.Shops;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute
{
    public static class PlayerCountableItems
    {
        public const string a = "";
    }
    /// <summary>
    /// User Data
    /// </summary>
    [Serializable]
    public class Data : IUUIDLabeled
    {
        [SerializeField] private UUID uuid;
        [SerializeField] private WorldTime playerLastOperationTime;
        public UUID UUID { get => uuid; set => uuid = value; }
        public WorldTime PlayerLastOperationTime { get => playerLastOperationTime; set => playerLastOperationTime = value; }

        #region Curency 
        [SerializeField] private ShopInfo shopInfo;
        [SerializeField] private int federgram;
        [SerializeField] private int manpower;
        [SerializeField] private int mantleAlloy;
        [SerializeField] private int aethium;

        public ShopInfo ShopInfo { get => shopInfo; set => shopInfo = value; }
        public int Federgram { get => federgram; set => federgram = value; }
        public int Manpower { get => manpower; set => manpower = value; }
        public int MantleAlloy { get => mantleAlloy; set => mantleAlloy = value; }
        public int Aethium { get => aethium; set => aethium = value; }

        public bool Spent(Currency.Type type, int amount)
        {
            Debug.Log("try currency spent");
            switch (type)
            {
                case Currency.Type.fedgram:
                    if (Federgram >= amount)
                    {
                        Federgram -= amount;
                        break;
                    }
                    else return false;
                case Currency.Type.manpower:
                    if (Manpower >= amount)
                    {
                        Manpower -= amount;
                        break;
                    }
                    else return false;
                case Currency.Type.mantleAlloy:
                    if (MantleAlloy >= amount)
                    {
                        MantleAlloy -= amount;
                        break;
                    }
                    else return false;
                case Currency.Type.Aethium:
                    if (Aethium >= amount)
                    {
                        Aethium -= amount;
                        break;
                    }
                    else return false;
                default:
                    return false;
            }

            PlayerFile.SaveCurrentData(); Debug.Log("Currency Spent");
            return true;
        }

        public bool Spent(Currency currency)
        {
            return Spent(currency.name, currency.count);
        }

        public bool Spent(params Currency[] currency)
        {
            if (!CanSpent(currency))
            {
                return false;
            }
            foreach (var item in currency)
            {
                Spent(item);
            }
            return true;

        }
        public bool CanSpent(params Currency[] currency)
        {
            int federgram = 0, manpower = 0, mantleAlloy = 0, aethium = 0;

            foreach (var item in currency)
            {
                var amount = item.count;
                switch (item.name)
                {
                    case Currency.Type.fedgram:
                        federgram += amount;
                        break;
                    case Currency.Type.manpower:
                        manpower += amount;
                        break;
                    case Currency.Type.mantleAlloy:
                        manpower += amount;
                        break;
                    case Currency.Type.Aethium:
                        Aethium += amount;
                        break;
                    default:
                        return false;
                }
            }

            if (Federgram < federgram)
                return false;
            if (Manpower < manpower)
                return false;
            if (MantleAlloy < mantleAlloy)
                return false;
            if (Aethium < aethium)
                return false;

            return true;
        }

        public void Earned(Currency currency)
        {
            Earned(currency.name, currency.count);
        }

        public void Earned(Currency.Type type, int amount)
        {
            switch (type)
            {
                case Currency.Type.fedgram:
                    Federgram += amount;
                    break;
                case Currency.Type.manpower:
                    Manpower += amount;
                    break;
                case Currency.Type.mantleAlloy:
                    MantleAlloy += amount;
                    break;
                case Currency.Type.Aethium:
                    Aethium += amount;
                    break;
            }
            PlayerFile.SaveCurrentData();
        }
        #endregion

        #region Legion Sets
        // 军团，用户默认军队设置储存
        // ......
        [Header("Exca Team")]
        [SerializeField] private ExplorationTeam excaTeam;
        public ExplorationTeam ExplorationTeam { get => excaTeam; set => excaTeam = value; }
        [Header("Legion Sets")]
        [SerializeField] private List<Legion> legions = new List<Legion>(3);
        [SerializeField] private List<EventCardPile> eventCardPiles = new List<EventCardPile>(3);
        public List<Legion> Legions { get => legions; set => legions = value; }
        public List<EventCardPile> EventCardPiles { get => eventCardPiles; set => eventCardPiles = value; }
        #endregion

        #region Items
        [Header("Player Items")]
        [SerializeField] protected List<ArmyItem> armies = new List<ArmyItem>();
        [SerializeField] protected List<LeaderItem> leaders = new List<LeaderItem>();
        [SerializeField] protected List<EquipmentItem> equipments = new List<EquipmentItem>();
        [SerializeField] protected List<EventCardItem> eventCards = new List<EventCardItem>();
        [SerializeField] protected List<string> collectiveStoriesID = new List<string>();
        [SerializeField] protected ItemList countableItems = new ItemList();


        public List<ArmyItem> Armies { get => armies; set => armies = value; }
        public List<LeaderItem> Leaders { get => leaders; set => leaders = value; }
        public List<EquipmentItem> Equipments { get => equipments; set => equipments = value; }
        public List<EventCardItem> EventCards { get => eventCards; set => eventCards = value; }
        public List<string> CollectiveStoriesID { get => collectiveStoriesID; set => collectiveStoriesID = value; }
        public ItemList CountableItems { get => countableItems; set => countableItems = value; }

        #endregion

        #region Player's Chapter Progress
        [Header("Chapter Progress")]
        [SerializeField] protected PlayerChapterTree gameProgree;
        public PlayerChapterTree PlayerChapterTreeStat { get => gameProgree; set => gameProgree = value; }
        #endregion

        #region Unlocked
        [Header("staticstic")]
        [SerializeField] private GameStatistic gameStatistic;
        public GameStatistic Statistic { get => gameStatistic; set => gameStatistic = value; }

        public CheckList EventCardUnlocked => GetEventTree();

        public bool IsArmyUnlocked(string name)
        {
            return Statistic.ArmiesUnlocked.Contains(name);
        }

        public bool IsArmiesUnlocked(string[] names)
        {
            foreach (string item in names)
            {
                if (!Statistic.ArmiesUnlocked.Contains(item))
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsLeaderUnlocked(string name)
        {
            return Statistic.LeadersUnlocked.Contains(name);
        }

        public bool IsLeadersUnlocked(string[] names)
        {
            foreach (string item in names)
            {
                if (!Statistic.LeadersUnlocked.Contains(item))
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

        public LeaderItem GetLeaderItem(string name)
        {
            foreach (LeaderItem item in Leaders)
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
            foreach (LeaderItem item in Leaders)
            {
                if (item.UUID == uuid)
                {
                    return item;
                }
            }

            return null;
        }

        public List<LeaderItem> GetLeaderItems(params UUID[] uuids)
        {
            List<LeaderItem> items = new List<LeaderItem>();
            foreach (UUID uuid in uuids)
            {
                LeaderItem item = GetLeaderItem(uuid);
                items.Add(item);
            }

            return items;
        }

        public ArmyItem GetArmyItem(UUID uuid)
        {
            foreach (ArmyItem item in Armies)
            {
                if (item.UUID == uuid)
                {
                    return item;
                }
            }

            return ArmyItem.Empty;
        }

        public List<ArmyItem> GetArmyItems(params UUID[] uuids)
        {
            List<ArmyItem> items = new List<ArmyItem>();
            foreach (UUID uuid in uuids)
            {
                ArmyItem item = GetArmyItem(uuid);
                items.Add(item);
            }

            return items;
        }

        public EquipmentItem GetEquipmentItem(UUID uuid)
        {
            foreach (EquipmentItem item in Equipments)
            {
                if (item.UUID == uuid)
                {
                    return item;
                }
            }

            return null;
        }

        public List<EquipmentItem> GetEquipmentItems(params UUID[] uuids)
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
            foreach (var item in EventCards)
            {
                if (item.UUID == uuid)
                {
                    return item;
                }
            }

            return null;
        }

        public List<EventCardItem> GetEventCardItems(params UUID[] uuids)
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
            if (!Statistic.ArmiesUnlocked.Contains(item.Name))
            {
                Statistic.ArmiesUnlocked.Add(item.Name);
            }

            Armies.Add(item);
        }

        public void AddLeaderItem(LeaderItem item)
        {
            if (!Statistic.LeadersUnlocked.Contains(item.Name))
            {
                Statistic.LeadersUnlocked.Add(item.Name);
            }
            Leaders.Add(item);
        }

        public void AddEquipment(EquipmentItem item)
        {
            if (!Statistic.EquipmentsUnlocked.Contains(item.Name))
            {
                Statistic.EquipmentsUnlocked.Add(item.Name);
            }
            if (!Statistic.EquipmentsUnlocked.Contains(item.Name))
            {
                Statistic.EquipmentsUnlocked.Add(item.Name);
            }

            Equipments.Add(item);
        }

        #endregion

        public void ClearInvalidInfo()
        {
            armies = armies.Where((item) => item).ToList();
            leaders = leaders.Where((item) => item).ToList();
            equipments = equipments.Where((item) => item).ToList();
        }

        public Data()
        {
            uuid = Guid.NewGuid();
        }

        public Data(Guid guid)
        {
            uuid = guid;
            shopInfo = new ShopInfo();
            Legions = new List<Legion>() { new Legion(), new Legion(), new Legion() };
            EventCardPiles = new List<EventCardPile>() { new EventCardPile(), new EventCardPile(), new EventCardPile() };
            armies = new List<ArmyItem>();
            leaders = new List<LeaderItem>();
            equipments = new List<EquipmentItem>();
            eventCards = new List<EventCardItem>();

            gameProgree = new PlayerChapterTree();
            gameStatistic = new GameStatistic();

            excaTeam = new ExplorationTeam();

            for (int i = 0; i < 6; i++)
            {
                AddArmyItem(new ArmyItem(GameData.Prototypes.GetArmyPrototype("Basic Infantry")));
            }
            AddLeaderItem(new LeaderItem(GameData.Prototypes.GetLeaderPrototype("Canute Svensson")));
        }
    }
}