using Canute.BattleSystem;
using Canute.Shops;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Canute
{
    public static class PlayerFile
    {
        private static Data data;

        public static string DataPath => Application.persistentDataPath + "/Saves/";

        public static Data Data { get => GetData(); private set { data = value; Game.Configuration.LastGame = value.uuid; Game.SaveConfig(); GameData.instance.data = value; } }

        public static Data GetData()
        {
            return data ?? ContinueLastSaved();
        }

        /// <summary>
        /// save current player file
        /// </summary>
        public static bool SaveCurrentData()
        {
            Data.playerLastOperationTime = DateTime.UtcNow;
            string json = JsonUtility.ToJson(Data);
            string filePath = DataPath + data.uuid;
            string savePath = filePath + "/Data.json";

            if (!Directory.Exists(DataPath + data.uuid))
            {
                Directory.CreateDirectory(filePath);
            }
            try
            {
                File.WriteAllText(savePath, json, Encoding.UTF8);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }
        /// <summary>
        /// save player file
        /// </summary>
        public static bool SaveData(Data data)
        {
            data.playerLastOperationTime = DateTime.UtcNow;
            string json = JsonUtility.ToJson(data);
            string filePath = DataPath + data.uuid;
            string savePath = filePath + "/Data.json";

            if (!Directory.Exists(DataPath + data.uuid))
            {
                Directory.CreateDirectory(filePath);
            }

            try
            {
                File.WriteAllText(savePath, json, Encoding.UTF8);
                Debug.Log("Saved!");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        /// <summary>
        /// load data from local storage
        /// </summary>
        /// <param name="uuid">name of the folder</param>
        /// <returns></returns>
        public static bool LoadData(UUID uuid)
        {
            string path = DataPath + uuid + "/Data.json";

            if (!File.Exists(path))
            {
                Debug.Log("Save not found: " + path);
                return false;
            }

            string json = File.ReadAllText(path, Encoding.UTF8);
            Data = JsonUtility.FromJson<Data>(json); ;
            if (Data is null)
                return false;

            return true;
        }

        /// <summary>
        /// create a new player file
        /// </summary>
        /// <returns></returns>
        public static Data CreateNewPlayerFile()
        {
            Debug.Log("try create player file");
            Data = new Data();
            string filePath = DataPath + data.uuid;
            Directory.CreateDirectory(filePath);
            Debug.Log(filePath);
            File.Create(filePath + "/Data.json").Dispose();

            SaveCurrentData();
            return data;
        }

        public static Data ContinueLastSaved()
        {
            bool result = LoadData(Game.Configuration.LastGame);
            return result ? data : null;
        }

        public static List<Save> GetAllSaves()
        {
            List<Save> saves = new List<Save>();
            string[] folders = Directory.GetDirectories(DataPath);
            foreach (var item in folders)
            {
                string path = item + "/Data.json";
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    Save save = JsonUtility.FromJson<Save>(json);
                    save.filePath = path;
                    saves.Add(save);
                }
            }
            Debug.Log("Current saves count : " + saves.Count);
            return saves;
        }
    }



    [Serializable]
    public class Data : IUUIDLabeled
    {
        public UUID uuid;
        public WorldTime playerLastOperationTime;

        #region Curency 
        [SerializeField] private int federgram;
        [SerializeField] private int manpower;
        [SerializeField] private int mantleAlloy;
        [SerializeField] private int mantleFluid;
        [SerializeField] private int aethium;

        public int Federgram { get => federgram; set => federgram = value; }
        public int Manpower { get => manpower; set => manpower = value; }
        public int MantleAlloy { get => mantleAlloy; set => mantleAlloy = value; }
        public int MantleFluid { get => mantleFluid; set => mantleFluid = value; }
        public int Aethium { get => aethium; set => aethium = value; }

        public bool Spent(Currency.Type type, int amount)
        {
            switch (type)
            {
                case Currency.Type.federgram:
                    if (Federgram < amount)
                    {
                        Federgram -= amount;
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
                case Currency.Type.MantleFluid:
                    if (MantleFluid < amount)
                    {
                        MantleFluid -= amount;
                        return true;
                    }
                    return false;
                case Currency.Type.Aethium:
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
            return Spent(currency.type, currency.count);
        }
        #endregion

        #region LegionSets
        // 军团，用户默认军队设置储存
        // ......
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

        public List<ArmyItem> Armies { get => armies; set => armies = value; }
        public List<LeaderItem> Leaders { get => leaders; set => leaders = value; }
        public List<EquipmentItem> Equipments { get => equipments; set => equipments = value; }
        public List<EventCardItem> EventCards { get => eventCards; set => eventCards = value; }

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

        public UUID UUID { get => uuid; set => uuid = value; }

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
        public bool HaveLeader(string name)
        {
            return !(GetLeaderItem(name) is null);
        }

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
            foreach (var item in EventCards)
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
            if (!Statistic.ArmiesUnlocked.Contains(item.Name))
            {
                Statistic.ArmiesUnlocked.Add(item.Name);
            }

            Armies.Add(item);
        }

        public void AddLeader(LeaderItem item)
        {
            Leaders.Add(item);
        }

        public void AddEquipment(EquipmentItem item)
        {
            if (!Statistic.EquipmentsUnlocked.Contains(item.Name))
            {
                Statistic.EquipmentsUnlocked.Add(item.Name);
            }

            Equipments.Add(item);
        }

        #endregion

        public Data() { uuid = new UUID(); }
    }

    [Serializable]
    public class Save : IUUIDLabeled
    {
        public UUID uuid;
        public WorldTime playerLastOperationTime;
        public string filePath;

        public UUID UUID { get => uuid; set => uuid = value; }

        public Data GetData()
        {
            string path = PlayerFile.DataPath + uuid + "/Data.json";
            if (!File.Exists(path))
            {
                Debug.Log("Save not found");
                return null;
            }
            string json = File.ReadAllText(path, Encoding.UTF8);
            return JsonUtility.FromJson<Data>(json);
        }
    }
}