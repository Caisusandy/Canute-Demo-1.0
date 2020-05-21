using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Canute.UI
{
    public class GameScene : MonoBehaviour
    {
        public GameData game;
        public List<ScriptableObject> loadingScriptableObject;

        public void Awake()
        {
            if (!Game.Initialized)
            {
                Game.ReadConfig();
            }
        }

        // Start is called before the first frame update
        public void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}