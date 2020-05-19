using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Canute.BattleSystem;

namespace Canute.UI
{
    public class GameStartMenu : MonoBehaviour
    {
        public static GameStartMenu instance;
        public Button continueButton;
        public Button newGameButton;
        public Button savesButton;
        public Button settingsButton;
        public Button creditButton;
        [Header("")]
        public Text versionDisplayer;
        [Header("")]
        public GameObject savesMenu;
        private void Awake()
        {
            instance = this;
        }

        // Use this for initialization
        void Start()
        {
            if (string.IsNullOrEmpty(Game.Configuration.LastGame))
            {
                DestroyImmediate(continueButton.gameObject);
            }
            if (PlayerFile.GetAllSaves().Count == 0)
            {
                DestroyImmediate(savesButton.gameObject);
            }
            versionDisplayer.text = GameData.Version.ToString();
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Continue()
        {
            PlayerFile.ContinueLastSaved();
            SceneControl.GotoScene(MainScene.mainHall);
        }

        public void NewGame()
        {
            PlayerFile.CreateNewPlayerFile();
            SceneControl.GotoScene(MainScene.mainHall);
            /*
             *
             * 
             */
        }

        public void OpenSavesMenu()
        {
            savesMenu.SetActive(true);
        }

        public void CloseSavesMenu()
        {
            savesMenu.SetActive(false);
        }

        public void Credit()
        {

        }
    }
}