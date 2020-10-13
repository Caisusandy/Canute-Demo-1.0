using Canute.BattleSystem;
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
        public ChapterTree.Chapters chapter;
        [Header("UI")]
        public Text title;
        public Text description;
        public LeaderScroll leaderScroll;

        public LSLegionSmallIcon selectingLegion;
        public ECPPileSmallIcon selectingPile;
        [Header("Button")]
        public Button startButton;
        public Button last;
        public Button next;


        private Canute.Legion legion;
        private Canute.EventCardPile pile;


        private LeaderItem LeaderItem => leaderScroll.SelectingLeader;
        public Canute.Legion Legion { get => legion; set => legion = value; }
        public Canute.EventCardPile Pile { get => pile; set => pile = value; }
        private LegionSet LegionSet => new LegionSet(Legion, Pile, LeaderItem.UUID, "Canute Svensson");
        private Level Level => GameData.Levels.GetLevel(levelName);
        private Canute.LevelTree.Chapter Chapter => GameData.Levels.GetChapter(chapter);


        private void Awake()
        {
            LSLegionSmallIcon.SelectEvent = SelectLegion;
            ECPPileSmallIcon.SelectEvent = SelectPile;
        }

        // Start is called before the first frame update
        void Start()
        {
            title.text = Level.Title;
            description.text = Level.Description;
            startButton.interactable = false;
            SetLevelChangeButton();
        }

        private void SetLevelChangeButton()
        {
            next.gameObject.SetActive(Level.IsPassed && Level.Next != null);
            last.gameObject.SetActive(Level.LastLevel != null);
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                DestroyImmediate(gameObject);
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
            Debug.Log(LegionSet.Leader?.Name);
            Game.LoadBattle(Level, LegionSet);
        }

        public void Next()
        {
            if (Level.IsPassed && Level.Next != null)
            {
                levelName = Level.nextLevel;
                Start();
            }
        }
        public void Last()
        {
            if (Level.LastLevel != null)
            {
                levelName = Level.lastLevel;
                Start();
            }
        }
    }
}