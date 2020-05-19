using Canute.BattleSystem.AI;
using Canute.BattleSystem.UI;
using UnityEngine;

namespace Canute.BattleSystem
{
    /// <summary>
    /// card entity of AI's card
    /// </summary>
    public class NonControllingCardEntity : CardEntity
    {
        public override void Select() { }

        public override void Unselect() { }

        public override void Highlight() { }

        public override void Unhighlight() { }

        public override bool ToggleSelect() { return false; }

        public override void Update() { }

        public static CardEntity Create(Card card, Player player)
        {
            AIEntity aI = player.AI;
            NonControllingCardEntity cardEntity;
            card.Owner = player;
            GameObject gameObject = new GameObject("Card", typeof(NonControllingCardEntity));
            gameObject.transform.SetParent(aI.transform);
            cardEntity = gameObject.GetComponent<NonControllingCardEntity>();
            cardEntity.data = card;
            cards.Add(cardEntity);

            //Reorganize(player.Entity.cards, BattleUI.HandCardBar);
            return cardEntity;
        }

        public override void Destroy()
        {
            Owner.RemoveHandCard(data);
            cards.Remove(this);
            //Destroy(this); 
            Destroy(gameObject);
        }
    }
}