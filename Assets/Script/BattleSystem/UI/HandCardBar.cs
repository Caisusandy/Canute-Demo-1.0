using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem.UI
{
    public class HandCardBar : BattleUIBase
    {
        public static List<HandCardBar> handCardBars = new List<HandCardBar>();

        public virtual List<CardEntity> Cards => Player?.Entity?.Cards;
        public bool isHidingCard = false;

        public override void Awake()
        {
            handCardBars.Add(this);
            base.Awake();
        }

        // Start is called before the first frame update
        public virtual void Start()
        {
        }

        // Update is called once per frame
        public virtual void Update()
        {

        }

        public IEnumerator Active()
        {
            while (true)
            {
                gameObject.SetActive(!(Player.IsInTurn || Game.CurrentBattle.CurrentStat == Battle.Stat.begin));
                yield return new WaitForEndOfFrame();
            }
        }

        public virtual void OnMouseDown()
        {
#if UNITY_EDITOR
            Game.CurrentBattle.GetHandCard(Game.CurrentBattle.Player, 1);
#endif
        }


        public virtual void HideCards(bool value)
        {
            if (isHidingCard == value)
            {
                if (value)
                {
                    CardEntity.HideCards(Cards, this);
                }

                return;
            }

            isHidingCard = value;

            if (value)
            {
                CardEntity.HideCards(Cards, this);
            }
            else
            {
                BattleUI.ArmyBar.Show();
                CardEntity.Reorganize(Cards, this);
            }
        }

        public void OnDisable()
        {
            HideCards(true);
        }

        public void OnEnable()
        {
            HideCards(false);
        }

        public void OnDestroy()
        {
            handCardBars.Remove(this);
        }

        public static HandCardBar GetHandCardBar(Player player)
        {
            foreach (var item in handCardBars)
            {
                if (item.Player == player)
                {
                    return item;
                }
            }
            return null;
        }
    }
}