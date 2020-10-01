﻿using Canute.LevelTree;
using Canute.UI;
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Canute
{
    public enum MainScene
    {
        mainHall,
        legionSetting,
        battle,
        StoryDisplayer,
        letterDisplayer,
        gameStart,
        playerArmyList,
        playerLeaderList,
        playerEquipmentList,
    }

    [CreateAssetMenu(fileName = "Scene Control", menuName = "Game Data/Scene Control", order = 1)]
    public class SceneControl : ScriptableObject
    {
        public const string storyDisplayer = "StoryLineLoader";
        public const string letterDisplayer = "LetterLineLoader";
        public const string main = "Main";
        public const string legionSetting = "Legion";
        public const string battle = "Battle";
        public const string gameStart = "Game Start";
        public const string armyList = "Army List";
        public const string EquipmentList = "Equipment List";
        public const string leaderList = "Leader List";

        public static string lastScene;

        public static string GetName(MainScene scene)
        {
            string ans = "";
            switch (scene)
            {
                case MainScene.mainHall:
                    ans = main;
                    break;
                case MainScene.legionSetting:
                    ans = legionSetting;
                    break;
                case MainScene.battle:
                    ans = battle;
                    break;
                case MainScene.StoryDisplayer:
                    ans = storyDisplayer;
                    break;
                case MainScene.letterDisplayer:
                    ans = letterDisplayer;
                    break;
                case MainScene.gameStart:
                    ans = gameStart;
                    break;
                case MainScene.playerArmyList:
                    ans = armyList;
                    break;
                case MainScene.playerLeaderList:
                    ans = leaderList;
                    break;
                case MainScene.playerEquipmentList:
                    ans = EquipmentList;
                    break;
                default:
                    break;
            }
            return ans;
        }

        public static void GotoScene(MainScene scene) => SceneJumper.Goto(GetName(scene));

        public static void GotoSceneImmediate(MainScene scene) => SceneManager.LoadScene(GetName(scene));

        public static void AddScene(MainScene scene) => SceneJumper.Add(GetName(scene));

        public static void RemoveScene(MainScene scene) => SceneJumper.Remove(GetName(scene));


        /// <summary>
        /// (used in UnityEditor) Goto another scene
        /// </summary>
        /// <param name="ans"></param>
        public void GotoScene(string ans) => SceneJumper.Goto(ans);

        /// <summary>
        /// (used in UnityEditor) Goto another scene
        /// </summary>
        /// 
        /// <param name="ans"></param>
        public void GotoSceneImmediate(string ans) => SceneManager.LoadScene(ans);



        [Temporary]
        public void OpenMain()
        {
            SceneManager.LoadScene("Main");
        }

    }
}