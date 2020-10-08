using System.Collections.Generic;
using UnityEngine;
using Canute.StorySystem;
using UnityEngine.UI;
using Canute.Testing;
using System.Collections;
using System;

namespace Canute.BattleSystem.UI
{
    public class BattleUI : BattleUIBase
    {
        public GameObject mapAnchor;
        public Battle battle;


        public static BattleUI instance;
        public static IWindow currentWindow;
        public static GameObject MapAnchor => instance.mapAnchor;
        public static GraphicRaycaster Raycaster => instance.GetComponent<GraphicRaycaster>();
        public static Canvas UICanvas => instance.GetComponent<Canvas>();

        [Header("anchors")]
        public GameObject undersideAnchor;
        public GameObject messageAnchor;

        [Header("UI components")]
        public HighBar highBar;
        public static HighBar HighBar => instance.highBar;

        public RightPanel rightPanel;
        public static RightPanel RightPanel => instance.rightPanel;

        public LeftPanel leftPanel;
        public static LeftPanel LeftPanel => instance.leftPanel;

        public ArmyBar armyBar;
        public static ArmyBar ArmyBar => instance.armyBar;

        public PausePanel pausePanel;
        public static PausePanel PausePanel => instance.pausePanel;


        public ResultUI endUI;
        public static ResultUI EndUI => instance.endUI;

        public Image fadeOutImage;
        public static Image FadeOutImage => instance.fadeOutImage;

        public HandCardBar handCardBar;
        public static HandCardBar HandCardBar => Game.CurrentBattle.Enemy.IsInTurn ? EnemyHandCardBar : ClientHandCardBar;
        #region HandCard Bar
        public static HandCardBar ClientHandCardBar => instance.handCardBar;
        public HandCardBar enemyHandCardBar;
        public static HandCardBar EnemyHandCardBar => instance.enemyHandCardBar;

        #endregion
        public RightPanel enemyRightPanel;
        public static RightPanel EnemyRightPanel => instance.enemyRightPanel;
        [Header("AI")]
        public GameObject AIHolder;
        public List<PlayerEntity> playerEntities;

        [Header("Debug")]
        public GameDebug debug;
        public static GameDebug DebugWindow => instance.debug;
        [Header("Camera")]
        public Camera secondCamera;


        #region Anchors 
        public static Vector3 UndersideAnchor => instance.undersideAnchor.transform.position;
        #endregion




        IEnumerator InstantiateMap()
        {
            yield return Instantiate(Game.CurrentBattle.MapPrefab, MapAnchor.transform);
            yield return null;
        }

        public override void Awake()
        {
            if (Game.CurrentBattle == null)
            {
                SceneControl.GotoScene(MainScene.mainHall);
                return;
            }
            if (instance != null)
            {
                Destroy(instance);
            }
            instance = this;
            FadeOutImage.enabled = true;
            var c = FadeOutImage.color;
            c.a = 1;
            FadeOutImage.color = c;
            StartCoroutine(InstantiateMap());
        }

        // Start is called before the first frame update
        private void Start()
        {
            Debug.Log("Battle in prepare");
            Game.CurrentBattle.Prepare();
            battle = Game.CurrentBattle;
            if (Game.CurrentBattle.AvoidPlayerLegion)
            {
                Game.CurrentBattle.Start();
            }
        }

        // Update is called once per frame
        private void Update()
        {
            Game.CurrentBattle.EndCheck();
        }

        /// <summary>
        /// create an AIentity
        /// </summary>
        /// <param name="player"></param>
        public void CreateAI(Player player)
        {
            PlayerEntity aI = AI.AIEntity.Create(AIHolder, player);
            Entity.entities.Add(aI);
            playerEntities.Add(aI);
        }

        /// <summary>
        /// create a playerEntity
        /// </summary>
        /// <param name="player"></param>
        public void CreatePlayerEntity(Player player)
        {
            PlayerEntity aI = PlayerEntity.Create(AIHolder, player);
            Entity.entities.Add(aI);
            playerEntities.Add(aI);
        }

        #region Messenger

        /// <summary>
        /// send game message, only when player is assigned to be the local player or did not assign the player can send message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="player"></param>
        public static void SendMessage(string message, Player player = null, params string[] param)
        {
            if (!instance)
            {
                return;
            }

            Debug.Log(message);

            if (player != Game.CurrentBattle.Player && !(player is null))
            {
                return;
            }

            GameMessage messageDisplayer = GameMessage.GetNewMessage();
            messageDisplayer.line = message;
            messageDisplayer.transform.SetParent(instance.messageAnchor.transform);
            messageDisplayer.transform.localScale = Vector3.one;
            messageDisplayer.transform.localPosition = Vector3.zero;
        }

        /// <summary>
        /// send game message, only when player is assigned to be the local player or did not assign the player can send message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="color"></param>
        /// <param name="player"></param>
        public static void SendMessage(string message, Color color, Player player = null, params string[] param)
        {
            if (!instance)
            {
                return;
            }

            Debug.Log(message);
            if (player != Game.CurrentBattle.Player && !(player is null))
            {
                return;
            }

            GameMessage messageDisplayer = GameMessage.GetNewMessage();
            messageDisplayer.line = message;
            messageDisplayer.transform.SetParent(instance.messageAnchor.transform);
            messageDisplayer.transform.localScale = Vector3.one;
            messageDisplayer.transform.localPosition = Vector3.zero;
            messageDisplayer.text.color = color;
        }

        public static void SendMessage(BattleEventError message, Player player = null, params string[] param) => SendMessage(message.Lang(), Color.white, player, param);

        public static void SendMessage(BattleEvent message, Color color, Player player = null, params string[] param) => SendMessage(message.Lang(), color, player, param);


        #endregion

        /// <summary>
        /// control player's down bars(army bar, hand card bar)
        /// </summary>
        /// <param name="value"></param>
        public static void SetPlayerDownBarsActive(bool value)
        {
            if (!instance)
            {
                return;
            }

            if (value) ArmyBar.Show();
            else ArmyBar.Hide();
            HandCardBar.GetHandCardBar(instance.Player).HideCards(!value);
        }

        public static void ChangePlayerUI(Player player, bool value)
        {
            if (Game.Configuration.PvP)
            {
                HandCardBar handCardBar = HandCardBar.GetHandCardBar(player);
                if (!handCardBar)
                {
                    return;
                }

                handCardBar.enabled = value;
                handCardBar.gameObject.SetActive(value);
                handCardBar.transform.parent.gameObject.SetActive(value);
            }
            ChangeCameraDisplay();
        }

        public static void ChangeCameraDisplay()
        {
            if (Game.Configuration.PlayerAutoSwitch)
            {
                instance.secondCamera.targetDisplay = Game.CurrentBattle.Enemy.IsInTurn ? 0 : 1;
                Camera.main.targetDisplay = Game.CurrentBattle.Player.IsInTurn ? 0 : 1;
            }
        }

        public static void ShowEndUI()
        {
            instance.StartCoroutine(instance.ReadyShowEndUI());
        }

        public static Coroutine FadeOutBattle()
        {
            return instance.StartCoroutine(instance.FadeOut());
        }

        public static Coroutine FadeInBattle()
        {
            return instance.StartCoroutine(instance.FadeIn());
        }

        public IEnumerator ReadyShowEndUI()
        {
            while (StoryDisplayer.currentStory || LetterDisplayer.instance)
            {
                yield return new WaitForFixedUpdate();
            }

            EndUI.gameObject.SetActive(true);
            yield return null;
        }

        public IEnumerator FadeOut()
        {
            fadeOutImage.enabled = true;
            Color color = fadeOutImage.color;
            color.a = 0;
            fadeOutImage.color = color;

            while (true)
            {
                color = fadeOutImage.color;
                color.a += Time.deltaTime;
                fadeOutImage.color = color;
                if (fadeOutImage.color.a > 1) { yield break; }
                else { yield return new WaitForEndOfFrame(); }
            }
        }

        public IEnumerator FadeIn()
        {
            fadeOutImage.enabled = true;
            Color color = fadeOutImage.color;
            color.a = 1;
            fadeOutImage.color = color;

            while (true)
            {
                color = fadeOutImage.color;
                color.a -= Time.deltaTime;
                fadeOutImage.color = color;
                if (fadeOutImage.color.a < 0) { break; }
                else { yield return new WaitForEndOfFrame(); }
            }
            fadeOutImage.enabled = false;
        }

        #region Battle UI Control

        /// <summary>
        /// set all ui in active
        /// </summary>
        /// <param name="value"></param>
        [Obsolete]
        public static void SetUIActive(bool value)
        {
            if (!instance)
            {
                return;
            }

            SetUIInteractive(value);
            UICanvas.enabled = value;
            HighBar.enabled = value;
            RightPanel.enabled = value;
            LeftPanel.enabled = value;
            ArmyBar.enabled = value;
            PausePanel.enabled = value;
            ClientHandCardBar.enabled = value;

            if (Game.Configuration.PvP)
            {
                EnemyRightPanel.enabled = value;
                EnemyHandCardBar.enabled = value;
            }

            foreach (Transform item in instance.transform)
            {
                item.gameObject.SetActive(value);
            }
        }

        public static void SetUIInteractive(bool value)
        {
            if (!instance) return;
            Raycaster.enabled = value;
            //Debug.Log("Raycaster turn on: " + Raycaster.enabled);
        }

        public static void SetUICanvasActive(bool value)
        {
            if (!instance) return;
            Raycaster.enabled = value;
            UICanvas.enabled = value;
        }

        public static void ToggleUICanvas()
        {
            Raycaster.enabled = !UICanvas.enabled;
            UICanvas.enabled = !UICanvas.enabled;
        }
        #endregion

        #region Window

        /// <summary>
        /// open a window in the battle
        /// </summary>
        /// <param name="window"></param>
        public static void ToggleWindow(IWindow window)
        {
            if (!window.enabled)
            {
                currentWindow = window;
                currentWindow.Open();
                SetUIInteractive(false);
            }
            //if the window component is not enabled
            else
            {
                window.Close();
                SetUIInteractive(true);
                currentWindow = null;
            }
        }

        /// <summary>
        /// close current window
        /// </summary>
        public static void CloseCurrentWindow()
        {
            currentWindow?.Close();
            SetUIInteractive(true);
            currentWindow = null;
        }

        #endregion

        public void OnDestroy()
        {
            instance = null;
        }
    }
}
