using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using Canute.BattleSystem;
using Canute.Testing;

namespace Canute.UI
{
    public class GameStartMenu : MonoBehaviour
    {
        public const string currentFirstChat = "theLastDayAtFantarium";
        public static GameStartMenu instance;
        public Button continueButton;
        public Button newGameButton;
        public Button savesButton;
        public Button settingsButton;
        public Button creditButton;
        public Button exitButton;
        [Header("")]
        public Text versionDisplayer;
        [Header("")]
        public GameObject savesMenu;
        [Header("")]
        public AudioSource audioSource;
        public AudioSource preAudioSource;
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
            audioSource.volume = Game.Configuration.Volume;
            preAudioSource.volume = Game.Configuration.Volume * 5;
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
            Game.LoadBattle(GameData.Levels.GetLevel("Tutorial"), new LegionSet(Game.PlayerData.Legions[0], Game.PlayerData.EventCardPiles[0], Game.PlayerData.Leaders[0].UUID, Game.PlayerData.Leaders[0].Name));
        }

        public void OpenSavesMenu()
        {
            savesMenu.SetActive(true);
        }

        public void CloseSavesMenu()
        {
            savesMenu.SetActive(false);
        }

        public void ExitGame()
        {
            ConfirmWindow.CreateConfirmWindow(() => Application.Quit(), "Canute.UI.Start.Quit.Check".Lang());
        }

        public void Credit()
        {
            var info = "Wendell Cai (Leader, programmer, artist, story, soundtrack)," +
                "\n Kira Wang (artist)," +
                "\n Adelle Alexander (story), " +
                "\nand Gia Khanh (programmer)" +
                "\nAdvisor: David Rios" +
                "\nThis is a demo version of the game.";
            var a = InfoWindow.Create(info);
            a.transform.SetParent(transform.parent);
            Destroy(a.gameObject.GetComponent<GraphicRaycaster>());
            Destroy(a.gameObject.GetComponent<Canvas>());
        }
    }
}