using Canute.Assets.Script.Module;
using Canute.StorySystem;
using Canute.Testing;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Canute.UI
{
    /// <summary>
    /// Server of Game
    /// </summary>
    public class GameServer : MonoBehaviour
    {
        public static GameServer instance;
        public GameData game;

        public List<ScriptableObject> loadingScriptableObject;

        private GameObject console;

        public void Awake()
        {
            instance = this;
            if (!Game.Initialized)
            {
                Game.ReadConfig();
                Languages.ForceLoadLang();
            }

            if (!GameBackgroundMusic.instance)
            {
                Instantiate(GameData.Prefabs.GameBackgroundMusicManager);
            }
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        // Start is called before the first frame update
        public void Start()
        {
            LoadSceneIntro();
        }

        private static void LoadSceneIntro()
        {
            string currentSceneName = SceneManager.GetActiveScene().name;

            if (currentSceneName == "Battle") { return; }
            if (currentSceneName == "Settings" || currentSceneName == "Loading" || currentSceneName == "Game Start") { return; }
            if (currentSceneName == "Equipment List" || currentSceneName == "Army List") { return; }

            if (!Game.PlayerData.GameSceneBeenTo.Contains(currentSceneName))
            {
                Debug.Log(currentSceneName);
                StoryDisplayer.LoadSceneIntro("SceneIntro" + currentSceneName);
                Game.PlayerData.GameSceneBeenTo.Add(currentSceneName);
                PlayerFile.SaveCurrentData();
            }
        }

        // Update is called once per frame
        void Update()
        {
            LoadSceneIntro();
            FunctionKey();
        }

        private void FunctionKey()
        {
            if (!Game.Configuration.IsDebugMode)
            {
                return;
            }
            if (Input.GetKeyDown(KeyCode.Slash) && Game.Configuration.IsDebugMode)
            {
                if (!console)
                {
                    Destroy(this.console);
                    var gameObject = Instantiate(GameData.Prefabs.Get("console"), transform);
                    var console = gameObject.GetComponent<Console>();
                    this.console = gameObject;
                }
                console.GetComponent<Console>().Open();
            }
        }
    }
}