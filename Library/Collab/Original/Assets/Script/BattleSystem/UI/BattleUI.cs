using System.Collections.Generic;
using UnityEngine;
using Canute.StorySystem;
using UnityEngine.UI;
using Canute.Languages;
using Canute.Testing;

namespace Canute.BattleSystem.UI
{
    public class BattleUI : BattleUIBase
    {
        public static BattleUI instance;

        public GameObject mapAnchor;
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
        public EndPanel endUI;
        public static EndPanel EndUI => instance.endUI;
        public HandCardBar handCardBar;
        public static HandCardBar ClientHandCardBar => instance.handCardBar;

        public static HandCardBar HandCardBar => Game.CurrentBattle.Enemy.IsInTurn ? EnemyHandCardBar : ClientHandCardBar;
        public HandCardBar enemyHandCardBar;
        public static HandCardBar EnemyHandCardBar => instance.enemyHandCardBar;
        public RightPanel enemyRightPanel;
        public static RightPanel EnemyRightPanel => instance.enemyRightPanel;
        [Header("AI")]
        public GameObject AIHolder;
        public List<PlayerEntity> playerEntities;

        [Header("Console")]
        public Console console;
        public static Console Console => instance.console;


        #region Anchors 
        public static Vector3 UndersideAnchor => instance.undersideAnchor.transform.position;
        #endregion



        public override void Awake()
        {
            if (instance != null)
            {
                Destroy(this);
            }
            instance = this;
            Instantiate(Game.CurrentBattle.MapPrefab, MapAnchor.transform);
        }

        // Start is called before the first frame update
        private void Start()
        {
            Debug.Log("Battle in prepare");
            Game.CurrentBattle.Prepare();

        }

        // Update is called once per frame
        private void Update()
        {
            if (GameData.BuildSetting.PvP)
            {
                EnemyHandCardBar.transform.parent.gameObject.SetActive(Battle.Enemy.IsInTurn);

                ClientHandCardBar.gameObject.SetActive(Player.IsInTurn);
                EnemyHandCardBar.gameObject.SetActive(Battle.Enemy.IsInTurn);
            }

            Game.CurrentBattle.EndCheck();
        }

        private void OnMouseDown()
        {
            Game.CurrentBattle.GetHandCard(Game.CurrentBattle.Player, 1);
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

        /// <summary>
        /// send game message, only when player is assigned to be the local player or did not assign the player can send message
        /// </summary>
        /// <param name="message"></param>
        /// <param name="player"></param>
        public static void SendMessage(string message, Player player = null, params string[] param)
        {
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

        public static void SendMessage(BattleEventError message, Player player = null, params string[] param) => SendMessage(message.Lang(), player, param);

        /// <summary>
        /// control player's down bars(army bar, hand card bar)
        /// </summary>
        /// <param name="value"></param>
        public static void SetDownBarsActive(bool value)
        {
            if (value) ArmyBar.Show();
            else ArmyBar.Hide();
            ClientHandCardBar.HideCards(!value);
        }

        /// <summary>
        /// set all ui in active
        /// </summary>
        /// <param name="value"></param>
        public static void SetUIActive(bool value)
        {
            SetUIInteractive(value);
            UICanvas.enabled = value;
            HighBar.enabled = value;
            RightPanel.enabled = value;
            LeftPanel.enabled = value;
            ArmyBar.enabled = value;
            PausePanel.enabled = value;
            ClientHandCardBar.enabled = value;
            EndUI.enabled = value;

            if (GameData.BuildSetting.PvP)
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
            Raycaster.enabled = value;
            Debug.Log(Raycaster.enabled);
        }

        public static void SetConsoleOpen(bool value)
        {
            Console.gameObject.SetActive(value);
        }

        public void OnDestroy()
        {
            instance = null;
        }
    }
}
