using Canute.BattleSystem.UI;
using Canute.Module;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Motion = Canute.Module.Motion;

namespace Canute.BattleSystem
{
    public class CardEntity : InteractableEntity
    {
        public const float DistanceFromDeckParam = 1;

        public bool wasSelected;
        public bool isDraging;
        public bool isOnTrashCan;

        [Header("Entity Data")]
        public Card data;
        public override EntityData Data => data;
        [Header("Control/Display")]
        public CardCollider cardCollider;
        public Image careerPicture;
        public Text infoDisplayer;
        public Text actionPointDisplayer;
        public int id;


        #region Entity Status 
        private static InteractableEntity selectingEntity;
        private static CardEntity selectingCard;


        public static List<CardEntity> cards = new List<CardEntity>();
        public static float times = 0;

        public static bool IsDelayEnded => times > PlayCardDelay;
        public static float PlayCardDelay => Game.Configuration.PlayCardDelay;

        public static InteractableEntity SelectingEntity { get => selectingEntity; set { selectingEntity = value; } }
        public static CardEntity SelectingCard { get => selectingCard; set { selectingCard = value; } }
        #endregion


        public override void Start()
        {
        }

        public override void Update()
        {
            Display();
            DragCardDeleyAdd();
            //if (Game.CurrentBattle.Round.CurrentStat != Round.Stat.gameStart)
            //    if (!isDraging && !GetComponent<Motion>())
            //    {
            //        Idle(0);
            //    }
        }

        #region Mouse operation
        public override void OnMouseDown()
        {
            if (BattleUI.Raycaster.enabled == false)
            {
                return;
            }

            Debug.Log("touch card");
            if (!isSelected)
            {
                Select();
                wasSelected = false;
            }
            else
            {
                wasSelected = true;
            }
        }

        public override void OnMouseDrag()
        {
            if (BattleUI.Raycaster.enabled == false)
            {
                return;
            }

            isDraging = true;
            TryDrag();
        }

        public override void OnMouseUp()
        {
            if (BattleUI.Raycaster.enabled == false)
            {
                return;
            }

            isDraging = false;
            if (wasSelected) Unselect();
            else Reorganize(Owner.Entity.Cards, BattleUI.HandCardBar);
            cardCollider.last = null;
            TriggerSelectEvent(IsSelected);
            DetermineAction();
        }


        #endregion

        #region Entity respond to player action
        public virtual void Display()
        {
            if (isDraging && SelectingEntity)
            {
                if (SelectingEntity is IBattleableEntity && SelectingEntity.Owner == Owner)
                {
                    if (data.Effect.Type == Effect.Types.enterMove)
                    {
                        data.Effect.Parameter = (SelectingEntity as IBattleableEntity).Data.Properties.MoveRange;
                    }
                }
            }

            int layer = !IsSelected ? id * 2 : 1000;

            infoDisplayer.text = data.Effect.GetDisplayingName();
            infoDisplayer.canvas.sortingLayerName = "Card";
            infoDisplayer.canvas.sortingOrder = layer + 1;

            actionPointDisplayer.text = data.ActionPoint.ToString();
            actionPointDisplayer.canvas.sortingLayerName = "Card";
            actionPointDisplayer.canvas.sortingOrder = layer + 1;

            careerPicture.sprite = GameData.SpriteLoader.Get(SpriteAtlases.careerIcon, data.Career.ToString());
            GetComponent<Image>().canvas.sortingLayerName = "Card";
            GetComponent<Image>().canvas.sortingOrder = layer;

        }

        public override bool ToggleSelect()
        {
            base.ToggleSelect();
            if (!IsSelected)
            {
                Reorganize(Owner.Entity.Cards, BattleUI.HandCardBar);
            }

            return IsSelected;
        }

        public override void Select()
        {
            base.Select();
            SelectingCard.Exist()?.Unselect();
            SelectingCard = this;
            Reorganize(Owner?.Entity.Cards, BattleUI.HandCardBar);
        }

        public override void Unselect()
        {
            base.Unselect();
            if (SelectingEntity != this) SelectingEntity.Exist()?.Unselect();
            //if (SelectingCard != this) SelectingCard.Exist()?.Unhighlight();
            SelectingCard = null;
            Reorganize(Owner?.Entity.Cards, BattleUI.HandCardBar);
        }

        public override void Highlight()
        {
            base.Highlight();
        }

        public override void Unhighlight()
        {
            base.Unhighlight();
        }

        /// <summary>
        /// Determine the action player trying to do
        /// </summary>
        private void DetermineAction()
        {
            if (isOnTrashCan)
            {
                ThrowAway();
            }
            else
            {
                TryPlayCard();
            }
        }

        /// <summary>
        /// Try drag the card away
        /// </summary>
        private bool TryDrag()
        {
            if (IsSelected)
            {
                Drag();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Drag the card
        /// </summary>
        private void Drag()
        {
            if (GetComponent<Motion>())
                Destroy(GetComponent<Motion>());
            //if (Game.CurrentBattle.Round.CurrentStat != Round.Stat.gameStart)
            //    InPerformingAnimation();
            transform.position = Control.UserInputPosition + new Vector3(0, 0, 10);
            Debug.Log(transform.position);
            Debug.Log(Control.UserInputPosition);
        }

        private void DragCardDeleyAdd()
        {
            times += isDraging ? Time.deltaTime : 0;
        }

        private void TimeDeleyReset()
        {
            times = 0;
        }

        /// <summary>
        /// Throw this card away
        /// </summary>
        private void ThrowAway()
        {
            Destroy();
            Game.CurrentBattle.Player.ActionPoint++;
            /* */
        }

        /// <summary>
        /// try to play card
        /// </summary>
        /// <returns>status of player play card successfully</returns>
        public bool TryPlayCard()
        {
            Debug.Log("try play card");
            if (!Game.CurrentBattle.IsFreeTime)
            {
                Debug.Log("Card did not played: Not a free time to play card, there is at least an action in process");
                TimeDeleyReset();
                return false;
            }
            if (SelectingEntity == null)
            {
                BattleUI.SendMessage("Card did not played: no selecting Entity");
                TimeDeleyReset();
                return false;
            }
            //Debug.Log(data.ActionPoint + ", player has " + Owner.ActionPoint);
            if (!IsDelayEnded)
            {
                Debug.Log("Card did not played: not enough time deley to play card");
                TimeDeleyReset();
                return false;
            }
            if (!data.IsValidTarget(SelectingEntity))
            {
                BattleUI.SendMessage("Card did not played: not a valid target");
                TimeDeleyReset();
                return false;
            }

            PlayCard();
            TimeDeleyReset();
            return true;
        }


        /// <summary>
        /// When player's action is determine to play his selecting card
        /// </summary>
        public virtual void PlayCard()
        {
            Debug.Log("play card " + data);

            data.Effect.Source = this;
            data.Effect.Target = SelectingEntity;
            bool sucess = data.Play();

            if (sucess)
            {
                Destroy();
            }

            OnMapEntity.SelectingEntity.Exist()?.Unselect();
            /* */
        }

        #endregion 

        #region Card Positions 
        /// <summary> 重整卡牌实体的位置 </summary>
        public static void Reorganize(List<CardEntity> cardEntities, HandCardBar handCardBar)
        {
            if (cardEntities is null)
            {
                return;
            }

            if (handCardBar.isHidingCard)
            {
                Debug.Log("cannot reorganize card: cards are hiding");
                HideCards(cardEntities, handCardBar);
                return;
            }
            //Debug.Log("start reorganize card");
            OrderRearrange(cardEntities);
            ReassignPosition(cardEntities);
        }

        ///<summary>序列整理(不调整位置)</summary>
        private static void OrderRearrange(List<CardEntity> cardEntities)
        {
            ;
            for (int j = 0; j < cardEntities.Count - 1; j++)
            {
                for (int i = 0; i < cardEntities.Count - 1; i++)
                {
                    CardEntity CardEntity = cardEntities[i];
                    Card Card = cardEntities[i].data;
                    if (Card.Type > cardEntities[i + 1].data.Type)
                    {
                        cardEntities.RemoveAt(i);
                        cardEntities.Insert(i + 1, CardEntity);
                    }
                    else if (Card.Type == cardEntities[i + 1].data.Type)
                    {
                        if (Card.Career > cardEntities[i + 1].data.Career)
                        {
                            cardEntities.RemoveAt(i);
                            cardEntities.Insert(i + 1, CardEntity);
                        }
                    }
                }
            }

            foreach (CardEntity card in cardEntities)
            {
                card.id = cardEntities.IndexOf(card);
            }
            foreach (CardEntity card in cardEntities)
            {
                card.transform.SetSiblingIndex(card.id);
            }
        }

        private static Vector3 GetAngle(List<CardEntity> cardEntities, CardEntity Card)
        {
            float AnglePerCard = 2;
            if (cardEntities.Count > 7)
            {
                AnglePerCard = 40 / (cardEntities.Count + 1);
            }

            float cardcount = cardEntities.Count;
            float StartingAngle = 90 + (cardcount - 1) * AnglePerCard / 2;
            float angle = (StartingAngle - Card.id * AnglePerCard);
            return new Vector3(0, 0, angle - 90);
        }

        private static Vector3 GetPosition(List<CardEntity> cardEntities, CardEntity card)
        {
            float AnglePerCard = 20;
            if (cardEntities.Count > 4)
            {
                AnglePerCard = 100 / (cardEntities.Count + 1);
            }

            int cardcount = cardEntities.Count;
            float StartingAngle = 90 + (cardcount - 1) * AnglePerCard / 2;
            float angle = StartingAngle - card.id * AnglePerCard;
            float degreeX = Mathf.Cos(angle * Mathf.Deg2Rad) * DistanceFromDeckParam * 6.25f;
            float degreeY = Mathf.Sin(angle * Mathf.Deg2Rad) * DistanceFromDeckParam * 1.2f;

            return new Vector3(degreeX, degreeY) * (card.IsSelected ? 1.25f : 1f);

        }

        private static void ReassignPosition(List<CardEntity> cardEntities)
        {
            OrderRearrange(cardEntities);
            foreach (CardEntity cardEntity in cardEntities)
            {
                Rotation.SetRotation(cardEntity.gameObject, GetAngle(cardEntities, cardEntity));
                Motion.SetMotion(cardEntity.gameObject, GetPosition(cardEntities, cardEntity), Space.Self, null, true);
            }
        }

        public static void HideCards(List<CardEntity> cardEntities, HandCardBar handCardBar)
        {
            if (cardEntities is null)
            {
                return;
            }
            foreach (var cardEntity in cardEntities)
            {
                if (cardEntity.IsSelected)
                {
                    cardEntity.Unselect();
                }
                Motion.SetMotion(cardEntity.gameObject, handCardBar.transform.position - new Vector3(0, 3, 0), null, true);
            }
        }


        #endregion

        public static CardEntity Create(Card card)
        {
            HandCardBar handCardBar = HandCardBar.GetHandCardBar(card.Owner);
            if (handCardBar is null)
            {
                return NonControllingCardEntity.Create(card, card.Owner);
            }

            CardEntity cardEntity;
            GameObject cardObject;
            GameObject cardPrefab = card.Prefab;

            cardObject = Instantiate(cardPrefab, handCardBar.transform);
            cardEntity = cardObject.GetComponent<CardEntity>();

            cards.Add(cardEntity);

            cardEntity.data = card;
            cardObject.name = "Card";
            cardObject.transform.SetParent(handCardBar.transform);
            cardObject.transform.Find("Text").GetComponent<Canvas>().overrideSorting = true;

            cardEntity.Owner.AddHandCard(card);
            Reorganize(card.Owner.Entity.Cards, BattleUI.HandCardBar);
            return cardEntity;
        }

        public static List<CardEntity> GetHandCard(Player player)
        {
            List<CardEntity> cardEntities = new List<CardEntity>();

            foreach (var item in cards)
            {
                if (item.Owner.UUID == player.UUID)
                {
                    cardEntities.Add(item);
                }
            }

            return cardEntities;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override void Destroy()
        {
            cards.Remove(this);
            Owner.RemoveHandCard(data);

            Reorganize(Owner.Entity.Cards, BattleUI.HandCardBar);
            Animator.RemoveFromBattle();

            base.Destroy();
        }
    }
}