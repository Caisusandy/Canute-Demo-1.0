using Canute.LanguageSystem;
using System;
using UnityEngine;


namespace Canute
{
    [Serializable]
    [Obsolete]
    [CreateAssetMenu(fileName = "Player Setting", menuName = "Other/[Obsolete] Player Setting", order = 1)]
    public class BuildSetting : ScriptableObject
    {
        [Obsolete]
        public static BuildSetting instance;
        [Header("Testing only")]
        [SerializeField] private bool debugMode = true;
        [SerializeField] private bool pvp = true;
        [SerializeField] private bool playerAutoSwitch = true;

        [Header("Allow Player Control")]
        [SerializeField] private float playCardDelay = 0.25f;
        [SerializeField] private bool showStory = true;

        [Obsolete]
        public bool IsInDebugMode => debugMode;
        [Obsolete]
        public bool ShowStory => showStory;
        [Obsolete]
        public bool PvP => pvp;
        [Obsolete]
        public bool PlayerAutoSwitch => playerAutoSwitch;
        [Obsolete]
        public float PlayCardDelay { get => playCardDelay; set => playCardDelay = value; }
    }
}
