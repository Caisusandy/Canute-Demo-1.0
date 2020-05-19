using Canute.BattleSystem;
using Canute.Languages;
using Canute.LevelTree;
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
        public Button startButton;
        public Button last;
        public Button next;
        [Header("Prefab")]
        public GameObject leaderCard;


        private Legion legion;

        private Canute.EventCardPile eventCardItems = new Canute.EventCardPile();
        private LeaderItem LeaderItem => Game.PlayerData.Leaders[leaderScroll.selectingId];
        private LegionSet LegionSet => new LegionSet(legion, eventCardItems, LeaderItem.UUID, "Canute Svensson");
        private LevelTree.Level Level => GameData.Chapters.ChapterTree.GetLevel(levelName);

        private void Awake()
        {
            LSLegionSmallIcon.SelectEvent = SelectLegion;
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
            legion = Game.PlayerData.Legions[id];
            selectingLegion.ChangeLegion(id);
            startButton.interactable = true;
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