using Canute.BattleSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.LevelStart
{
    public class LeaderScroll : MonoBehaviour
    {
        public ScrollRect LeaderRect;
        public List<LeaderItem> LeaderItems => Game.PlayerData.Leaders;
        public LeaderItem selecting;
        public int selectingId;

        public float lastPos;

        [Header(" ")]
        public RectTransform selectPanel; //将ChoosePanel拖入
        public List<RectTransform> elements; //将Content中的若干个元素拖入 
        public RectTransform center; //将CenterToCompare空物体拖入

        public int distance; //相邻两个元素的距离，在Start方法计算  
        public bool dragging = false; //Element是否在被拖拽； 


        // Use this for initialization
        void Start()
        {
            GetAllLeaderCard();
        }

        private void GetAllLeaderCard()
        {
            foreach (RectTransform item in LeaderRect.content)
            {
                elements.Add(item);
            }
        }


        // Update is called once per frame
        void Update()
        {
            if (!dragging)
            {
                LerpEleToCenter();
            }

        }

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
            float distancePerLeader = 1f / (elements.Count - 1);
            for (int i = 0; i < elements.Count; i++)
            {
                float curDist = distancePerLeader * i;
                if (Mathf.Abs(x - curDist) < distancePerLeader / 2)
                {
                    selectingId = i;
                }
            }
        }

        void LerpEleToCenter()
        {
            Vector2 center = this.center.position;
            float dx = elements[selectingId].position.x - center.x;
            float newX = Mathf.Lerp(LeaderRect.content.position.x, LeaderRect.content.position.x - dx, Time.deltaTime * 20f); //使用Mathf.Lerp函数让数据的顺滑地变化

            Vector2 newPosition = new Vector2(newX, LeaderRect.content.position.y);//目标距离 
            selectPanel.position = newPosition;
        }

        public void StartDrag()
        {
            dragging = true;
        }

        public void EndDrap()
        {
            dragging = false;
        }
    }
}