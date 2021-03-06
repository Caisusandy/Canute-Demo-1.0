﻿using Canute.LevelTree;
using Canute.UI;
using System;
using System.Collections.Generic;
using System.Linq;
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
        settings,
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
        public const string equipmentList = "Equipment List";
        public const string leaderList = "Leader List";
        public const string settings = "Settings";

        public static List<string> lastScene = new List<string>();

        private static string GetName(MainScene scene)
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
                    ans = equipmentList;
                    break;
                case MainScene.settings:
                    ans = settings;
                    break;
                default:
                    break;
            }
            return ans;
        }

        public static void GotoScene(MainScene scene)
        {
            lastScene.Add(SceneManager.GetActiveScene().name);
            SceneJumper.Goto(GetName(scene));
        }

        public static void GotoSceneImmediate(MainScene scene)
        {
            lastScene.Add(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(GetName(scene));
        }

        public static void AddScene(MainScene scene) => SceneJumper.Add(GetName(scene));

        public static void RemoveScene(MainScene scene) => SceneJumper.Remove(GetName(scene));

        //public static void GotoLastScene()
        //{
        //    if (lastScene.Count > 0)
        //    {
        //        GotoScene(lastScene.Last());
        //    }
        //}

        //public static void GotoLastSceneImmediate()
        //{
        //    if (lastScene.Count > 0)
        //    {
        //        GotoSceneImmediate(lastScene.Last());
        //    }
        //}

        public void GotoLastScene()
        {
            if (SceneManager.sceneCount > 1)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                return;
            }
            else if (lastScene.Count > 0)
            {
                GotoScene(lastScene.Last());
            }
            else GotoScene(MainScene.gameStart);
        }

        public void GotoLastSceneImmediate()
        {
            if (SceneManager.sceneCount > 1)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                return;
            }
            else if (lastScene.Count > 0)
            {
                GotoSceneImmediate(lastScene.Last());
            }
            else GotoScene(MainScene.gameStart);
        }



        public static void GotoLast()
        {
            if (SceneManager.sceneCount > 1)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                return;
            }
            else if (lastScene.Count > 0)
            {
                Goto(lastScene.Last());
            }
            else GotoScene(MainScene.gameStart);
        }

        public static void GotoLastImmediate()
        {
            if (SceneManager.sceneCount > 1)
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
                return;
            }
            else if (lastScene.Count > 0)
            {
                GotoImmediate(lastScene.Last());
            }
            else GotoScene(MainScene.gameStart);
        }


        /// <summary>
        /// (used in UnityEditor) Goto another scene
        /// </summary>
        /// <param name="ans"></param>
        public static void Goto(string ans)
        {
            lastScene.Add(SceneManager.GetActiveScene().name);
            SceneJumper.Goto(ans);
        }

        /// <summary>
        /// (used in UnityEditor) Goto another scene
        /// </summary>
        /// 
        /// <param name="ans"></param>
        public static void GotoImmediate(string ans)
        {
            lastScene.Add(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(ans);
        }

        /// <summary>
        /// (used in UnityEditor) Goto another scene
        /// </summary>
        /// <param name="ans"></param>
        public void GotoScene(string ans)
        {
            lastScene.Add(SceneManager.GetActiveScene().name);
            SceneJumper.Goto(ans);
        }

        /// <summary>
        /// (used in UnityEditor) Goto another scene
        /// </summary>
        /// 
        /// <param name="ans"></param>
        public void GotoSceneImmediate(string ans)
        {
            lastScene.Add(SceneManager.GetActiveScene().name);
            SceneManager.LoadScene(ans);
        }

        [Temporary]
        public void OpenMain()
        {
            SceneManager.LoadScene("Main");
        }

    }
}