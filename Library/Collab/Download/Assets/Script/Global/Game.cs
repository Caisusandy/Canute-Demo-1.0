using Canute.BattleSystem;
using Canute.Languages;
using Canute.LevelTree;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Canute
{
    public static class Game
    {
        private static Battle currentBattle;
        private static Level currentLevel;
        private static Config config;
        private static bool initialized = false;


        /// <summary> 玩家数据 </summary>
        public static Data PlayerData => PlayerFile.Data;
        public static GameData GameData => GameData.instance;
        public static Config Configuration { get => config; set { config = value; } }
        public static Battle CurrentBattle { get => currentBattle; set { currentBattle = value; } }
        public static Level CurrentLevel { get => currentLevel; set { currentLevel = value; } }
        public static bool Initialized { get => initialized; set => initialized = value; }
        public static LanguageName Language => Configuration is null ? LanguageName.en_us : Configuration.Language;

        public static string ConfigPath => Application.persistentDataPath + "/Config.json";


        public static void ReadConfig()
        {
            string settingPath = File.ReadAllText(ConfigPath);
            Configuration = JsonUtility.FromJson<Config>(settingPath);
            initialized = true;

            Debug.Log("Configuration import complete");
            Debug.Log(Configuration.LastGame);

            ModuleLoad();
        }

        private static void ModuleLoad()
        {
            if (Game.Configuration.IsDebugMode)
            {
                Testing.Fields.tArmies.AddRange(Testing.PrototypeLoader.LoadAllUserPrototype<Army>());
                Testing.Fields.tBuildingList.AddRange(Testing.PrototypeLoader.LoadAllUserPrototype<Building>());
                Testing.Fields.tEventCards.AddRange(Testing.PrototypeLoader.LoadAllUserPrototype<EventCard>());
                Testing.Fields.tEquipments.AddRange(Testing.PrototypeLoader.LoadAllUserPrototype<Equipment>());
                Testing.Fields.tLeaders.AddRange(Testing.PrototypeLoader.LoadAllUserPrototype<Leader>());

                Testing.Fields.tArmies.AddRange(Testing.PrototypeLoader.LoadAllDefaultPrototype<Army>());
                Testing.Fields.tBuildingList.AddRange(Testing.PrototypeLoader.LoadAllDefaultPrototype<Building>());
                Testing.Fields.tEventCards.AddRange(Testing.PrototypeLoader.LoadAllDefaultPrototype<EventCard>());
                Testing.Fields.tEquipments.AddRange(Testing.PrototypeLoader.LoadAllDefaultPrototype<Equipment>());
                Testing.Fields.tLeaders.AddRange(Testing.PrototypeLoader.LoadAllDefaultPrototype<Leader>());
            }
        }

        public static void SaveConfig()
        {
            string json = JsonUtility.ToJson(Configuration);
            File.WriteAllText(ConfigPath, json, Encoding.UTF8);
            Debug.Log("Configuration saved");
        }

        public static void LoadBattle(Level level, LegionSet legionSet)
        {
            Debug.Log(legionSet);
            currentLevel = level;
            currentBattle = level.Data.GetBattle();
            currentBattle.SetPlayer(legionSet);
            SceneControl.GotoScene(MainScene.battle);
            level.OpenBGStory();
        }


        public static void ClearBattle()
        {
            Entity.Initialize();
            currentBattle = null;
        }






    }

    [Serializable]
    public class Config : ICloneable
    {
        public static Config instance { get => Game.Configuration; set => Game.Configuration = value; }

        [SerializeField] private UUID lastGame;

        [Header("Testing only")]
        [SerializeField] private bool debugMode = true;
        [SerializeField] private bool pvp = true;
        [SerializeField] private bool playerAutoSwitch = true;

        [Header("Allow Player Control")]
        [SerializeField] private float playCardDelay = 0.25f;
        [SerializeField] private bool showStory = true;
        [SerializeField] private LanguageName language = LanguageName.zh_cn;

        public UUID LastGame { get => lastGame; set { lastGame = value; Game.SaveConfig(); } }
        public bool IsDebugMode { get => debugMode; set { debugMode = value; Game.SaveConfig(); } }
        public bool PvP { get => pvp; set { pvp = value; Game.SaveConfig(); } }
        public bool PlayerAutoSwitch { get => playerAutoSwitch; set { playerAutoSwitch = value; Game.SaveConfig(); } }
        public float PlayCardDelay { get => playCardDelay; set { playCardDelay = value; Game.SaveConfig(); } }
        public bool ShowStory { get => showStory; set { showStory = value; Game.SaveConfig(); } }
        public LanguageName Language { get => language; set { language = value; Game.SaveConfig(); } }

        private Config() { }

        public Config Clone()
        {
            return MemberwiseClone() as Config;
        }

        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}