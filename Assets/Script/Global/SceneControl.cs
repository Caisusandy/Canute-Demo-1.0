using Canute.LevelTree;
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
        gameStart,
        playerArmyList,
    }

    [CreateAssetMenu(fileName = "Scene Control", menuName = "Game Data/Scene Control", order = 1)]
    public class SceneControl : ScriptableObject
    {
        public const string storyDisplayer = "StoryLineLoader";
        public const string main = "Main";
        public const string legionSetting = "Legion";
        public const string battleUI = "BattleUI";
        public const string gameStart = "Game Start";
        public const string armyList = "Army List";


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
                    ans = battleUI;
                    break;
                case MainScene.StoryDisplayer:
                    ans = storyDisplayer;
                    break;
                case MainScene.gameStart:
                    ans = gameStart;
                    break;
                case MainScene.playerArmyList:
                    ans = armyList;
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


        [Temporary]
        public void OpenLegionSetting()
        {
            GotoScene(MainScene.legionSetting);
        }

        [Temporary]
        public void OpenMain()
        {
            SceneManager.LoadScene("Main");
        }

    }
}