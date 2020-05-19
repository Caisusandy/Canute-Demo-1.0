﻿using Canute.LevelTree;
using UnityEngine;

namespace Canute.UI.LevelStart
{
    [CreateAssetMenu(fileName = "Level Start Panel", menuName = "UI/Level Start Panel", order = 1)]
    public class Levels : ScriptableObject
    {
        public static Levels instance;

        public GameObject levelStartPanel;

        public static void OpenLevelPanel(Level level, Transform parent)
        {
            var obj = Instantiate(instance.levelStartPanel, parent);
            var panel = obj.GetComponent<LevelStartPanel>();
            panel.levelName = level.Name;
        }

        public void OpenLevelPanel(Transform parent)
        {
            var obj = Instantiate(levelStartPanel, parent);
            var panel = obj.GetComponent<LevelStartPanel>();
            panel.levelName = GameData.Chapters.ChapterTree.GetLevel("TestingLevel").Name;
        }

        public void OnEnable()
        {
            instance = this;
        }

    }
}