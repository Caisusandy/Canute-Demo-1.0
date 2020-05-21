using Canute.BattleSystem;
using Canute.Languages;
using Canute.LevelTree;
using Canute.UI.Legion;
using Canute.UI.EventCardPile;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.LevelStart
{
    public class LevelStartPanel : MonoBehaviour
    {
        public string levelName;
        [Header("UI")]
        public Text title;
        public Text subtitle;
        public LeaderScroll leaderScroll;

        public LSLegionSmallIcon selectingLegion;
        public ECPPileSmallIcon selectingPile;
        [Header("Button")]
        public Button startButton;
        public Button last;
        public Button next;
        [Header("Prefab")]
        public GameObject leaderCard;


        private Canute.Legion legion;
        private Canute.EventCardPile pile;


        private LeaderItem LeaderItem => Game.PlayerData.Leaders[leaderScroll.selectingId];
        public Canute.Legion Legion { get => legion; set => legion = value; }
        public Canute.EventCardPile Pile { get => pile; set => pile = value; }
        private LegionSet LegionSet => new LegionSet(Legion, Pile, LeaderItem.UUID, "Canute Svensson");
        private Level Level => GameData.Chapters.ChapterTree.GetLevel(levelName);


        private void Awake()
        {
            LSLegionSmallIcon.SelectEvent = SelectLegion;
            ECPPileSmallIcon.SelectEvent = SelectPile;
        }

        // Start is called before the first frame update
        void Start()
        {
            LoadLeaders();
            leaderScroll.GetAllLeaderCard();

            title.text = Level.Lang("title");
            subtitle.text = Level.Lang("subtitle");
            startButton.interactable = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void LoadLeaders()
        {
            foreach (var item in Game.PlayerData.Leaders)
            {
                GameObject gameObject = Instantiate(this.leaderCard, leaderScroll.leaderRect.content);
                LeaderCardUI leaderCard = gameObject.GetComponent<LeaderCardUI>();
                leaderCard.Display(item);
            }
        }

        public void SelectLegion(int id)
        {
            Legion = Game.PlayerData.Legions[id];
            selectingLegion.ChangeLegion(id);
            UpdateStartButton();
        }

        public void SelectPile(int id)
        {
            Pile = Game.PlayerData.EventCardPiles[id];
            selectingPile.ChangePile(id);
            UpdateStartButton();
        }

        public void UpdateStartButton()
        {
            startButton.interactable = !(legion is null) && !(pile is null);
        }

        public void Close()
        {
            //gameObject.SetActive(false);
            //enabled = false;
            DestroyImmediate(gameObject);
        }


        public void GameStart()
        {
            Debug.Log(LegionSet.Leader.Name);
            Game.LoadBattle(Level, LegionSet);
        }
    }
}