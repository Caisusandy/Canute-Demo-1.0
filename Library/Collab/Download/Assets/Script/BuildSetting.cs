using Canute.Languages;
using System;
using UnityEngine;


namespace Canute
{
    [Serializable]
    [CreateAssetMenu(fileName = "Player Setting", menuName = "Game Data/Player Setting", order = 1)]
    public class BuildSetting : ScriptableObject
    {
        public static BuildSetting instance;
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
    }
}
