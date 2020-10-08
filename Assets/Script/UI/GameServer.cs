using Canute.Assets.Script.Module;
using Canute.Testing;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
            //if (instance) { Destroy(this); return; }
            //else instance = this;

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

        }

        // Update is called once per frame
        void Update()
        {
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