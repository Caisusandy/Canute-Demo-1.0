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
        public static Config Configuration { get => config; set { config = value; } }
        public static Battle CurrentBattle { get => currentBattle; set { currentBattle = value; } }
        public static Level CurrentLevel { get => currentLevel; set { currentLevel = value; } }
        public static bool Initialized { get => initialized; set => initialized = value; }
        public static GameData GameData => GameData.instance;

        public static string ConfigPath => Application.persistentDataPath + "/Config.json";


        public static void ReadConfig()
        {
            string setting = File.ReadAllText(ConfigPath);
            Configuration = JsonUtility.FromJson<Config>(setting);
            initialized = true;
            Debug.Log("Configuration import complete");
            Debug.Log(Configuration.LastGame);

            if (GameData.BuildSetting.IsInDebugMode)
            {
                Testing.Fields.tArmies.AddRange(Testing.PrototypeLoader.LoadAllPrototype<Army>());
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





        [Serializable]
        public class Config
        {
            public UUID lastGame;

            [Header("Testing only")]
            [SerializeField] private bool debugMode = true;
            [SerializeField] private bool pvp = true;
            [Header("Allow Player Control")]
            [SerializeField] private float playCardDelay = 0.25f;
            [SerializeField] private bool showStory = true;
            [SerializeField] private LanguageSetting language = LanguageSetting.zh_cn;

            public bool IsInDebugMode => debugMode;
            public bool ShowStory => showStory;
            public bool PvP => pvp;
            public float PlayCardDelay { get => playCardDelay; set => playCardDelay = value; }
            public LanguageSetting Language { get => language; set => language = value; }
            public UUID LastGame { get => lastGame; set => lastGame = value; }
        }
    }
}