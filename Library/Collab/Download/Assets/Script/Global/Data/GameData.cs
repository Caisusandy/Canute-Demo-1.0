using Canute.BattleSystem;
using Canute.LevelTree;
using Canute.Shops;
using Canute.StorySystem;
using Canute.Technologies;
using Canute.Languages;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using System.IO;
using System;
using Canute.UI.LevelStart;

namespace Canute
{

    /// <summary>
    /// 游戏
    /// </summary>
    //[CreateAssetMenu(fileName = "Game Data", menuName = "Game Data/Main", order = 1)]
    public class GameData : ScriptableObject
    {
        public static GameData instance;
        [Header("Version")]
        [SerializeField] protected CanuteVersion version;
        //[Header("Current Player Data")]
        //public PlayerData playerData;
        //public string lastSaveName;
        [Header("Game Module")]
        [SerializeField] protected PrototypeFactory prototypeFactory;
        [SerializeField] protected Prefabs entityPrefabs;
        [SerializeField] protected SpriteLoader spriteLoader;
        [SerializeField] protected Technology technology;
        [SerializeField] protected Shop shop;
        [SerializeField] protected Levels levelStart;
        [SerializeField] protected Chapters chapters;
        [SerializeField] protected Stories stories;
        [SerializeField] protected Language language;
        [SerializeField] protected ResonanceSheet resonanceSheet;
        [Header("Story Packs")]
        [SerializeField] protected StoryPacks storyPacks;

        [ContextMenuItem("Save", "SavePlayerFile")]
        [Header("Player File (please save after edit)")]
        public Data data;


        /// <summary> 原型工厂，访问所有游戏内物品的原型 </summary>
        public static PrototypeFactory Prototypes => instance.prototypeFactory;
        /// <summary> 游戏内物品的内置件 </summary>
        public static Prefabs Prefabs => instance.entityPrefabs;
        /// <summary> 章节书 </summary>
        public static Chapters Chapters => instance.chapters;
        /// <summary> 科技系统 </summary>
        public static Technology Technology => instance.technology;
        /// <summary> 商店状态 </summary>
        public static Shop Shop => instance.shop;
        /// <summary> Sprite获取器 </summary>
        public static SpriteLoader SpriteLoader => instance.spriteLoader;
        /// <summary> 语言包 </summary>
        public static Language Language => instance.language;
        /// <summary> 故事 </summary>
        public static Stories Stories => instance.stories;
        /// <summary> 目前进行的战斗 </summary>
        //public static Battle CurrentBattle => instance.currentBattle;
        /// <summary> </summary>
        //public static Level CurrentLevel => instance.currentLevel;
        /// <summary> </summary>
        //public static List<PlayerData> PlayerDatas => instance.playerDatas;

        ///<summary>   </summary>
        public static CanuteVersion Version => instance.version;

        public static ResonanceSheet ResonanceSheet => instance.resonanceSheet;

        public static Levels LevelStart => instance.levelStart;

        private GameData()
        {
            instance = this; //这么干可以使得没有地方可以再生一个这个class（非静态的唯一实例）
        }

        public void OnEnable()//当被加载时
        {
            instance = this;
            Language.ForceLoadLang();
            stories = storyPacks.Get(Game.Language.ToString());
        }

        public void SavePlayerFile()
        {
            PlayerFile.SaveData(data);
        }



    }
}