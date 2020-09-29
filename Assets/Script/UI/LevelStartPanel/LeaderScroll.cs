using Canute.BattleSystem;
using Canute.UI.LevelStart;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public delegate void LeaderSelection(LeaderItem leaderItem);
    public class LeaderScroll : MonoBehaviour
    {
        public static LeaderSelection SelectEvent;
        public static IEnumerable<LeaderItem> notShowingLeader;
        public static List<LeaderItem> leaders;
        public static LeaderScroll instance;

        [Header("Prefab")]
        public GameObject leaderCard;
        public ScrollRect leaderRect;

        public int selectingId;
        public float lastPos;

        [Header(" ")]
        public Transform selectPanel; //将ChoosePanel拖入
        public List<Transform> elements; //将Content中的若干个元素拖入 
        public RectTransform center; //将CenterToCompare空物体拖入

        public bool dragging = false; //Element是否在被拖拽； 


        public Text bonusInfo;
        public Text skillInfo;


        public List<LeaderItem> LeaderItems => Game.PlayerData.Leaders;
        public LeaderItem SelectingLeader => GetSelectingLeader();

        public int SelectingId { get => selectingId; set => selectingId = value; }

        private void Awake()
        {
            instance = this;
            LoadLeaders();
            GetAllLeaderCard();
        }

        void Start()
        {

        }

        void Update()
        {
            if (!dragging)
            {
                LerpEleToCenter();
            }

        }

        public void OnDestroy()
        {
            instance = null;
            notShowingLeader = null;
        }

        #region Scrolling
        //+1 using in editor
        public void Scrolling(Vector2 vector2)
        {
            float curPos = vector2.x;
            if (Mathf.Abs(curPos - lastPos) > 0.005)
            {
                lastPos = curPos;
                return;
            }
            else
            {
                EndScroll(curPos);
            }
        }

        public void EndScroll(float x)
        {
            float distancePerLeader = 1f / (selectPanel.childCount - 1);
            for (int i = 0; i < selectPanel.childCount; i++)
            {
                float curDist = distancePerLeader * i;
                if (Mathf.Abs(x - curDist) < distancePerLeader / 2)
                {
                    SelectingId = i;
                    ShowLeaderInfo();
                }
            }
        }

        void LerpEleToCenter()
        {
            Vector2 center = this.center.position;
            float dx = selectPanel.GetChild(SelectingId).position.x - center.x;
            float newX = Mathf.Lerp(leaderRect.content.position.x, leaderRect.content.position.x - dx, Time.deltaTime * 20f); //使用Mathf.Lerp函数让数据的顺滑地变化

            Vector2 newPosition = new Vector2(newX, leaderRect.content.position.y);//目标距离 
            selectPanel.position = newPosition;
        }

        //+1 using in editor
        public void StartDrag()
        {
            dragging = true;
        }

        //+1 using in editor
        public void EndDrag()
        {
            dragging = false;
        }

        #endregion


        private LeaderItem GetSelectingLeader()
        {
            if (SelectingId > 0)
            {
                return leaders[SelectingId - 1];
            }
            else return LeaderItem.Empty;
        }

        public void GetAllLeaderCard()
        {
            foreach (Transform item in leaderRect.content)
            {
                elements.Add(item);
            }
        }

        public void LoadLeaders()
        {
            leaders = Game.PlayerData.Leaders;
            if (!(notShowingLeader is null))
            {
                leaders = leaders.Except(notShowingLeader).ToList();
            }

            GameObject firstG = Instantiate(this.leaderCard, leaderRect.content);
            LeaderCardUI first = firstG.GetComponent<LeaderCardUI>();
            first.Display(LeaderItem.Empty);

            foreach (var item in leaders)
            {
                GameObject gameObject = Instantiate(this.leaderCard, leaderRect.content);
                LeaderCardUI leaderCard = gameObject.GetComponent<LeaderCardUI>();
                leaderCard.Display(item);
            }
        }

        public void Select()
        {
            SelectEvent?.Invoke(SelectingLeader);
        }

        public void Close()
        {
            SelectEvent?.Invoke(LeaderItem.Empty);
        }

        public static void OpenLeaderScroll()
        {
            Instantiate(GameData.Prefabs.Get("leaderScroll"));
        }

        public static void CloseLeaderScroll()
        {
            Destroy(instance.gameObject);
        }

        public void ShowLeaderInfo()
        {
            bonusInfo.text = "";
            var text = SelectingLeader.Bonus.ToArray().Lang();
            if (text.Length > 1)
                bonusInfo.text = text.Remove(text.Length - 1);
            if (skillInfo)
            {
                skillInfo.text = "";//SelectingLeader.
            }
        }
    }
}