
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Canute.BattleSystem
{
    ///<summary> 中央牌堆 </summary>
    [Serializable]
    public class CentralDeck : DataList<Card>
    {
        public void Generated()
        {
            for (int i = 1; i <= 4; i++)
            {
                Career career = (Career)i;
                for (int j = 0; j < 12; j++)
                {
                    Add(new Card(Card.Types.normal, career, new Effect(Effect.Types.enterAttack, 1), 2, Card.TargetType.self | Card.TargetType.armyEntity));
                }
                for (int j = 0; j < 12; j++)
                {
                    Add(new Card(Card.Types.normal, career, new Effect(Effect.Types.enterMove, 1), 3, Card.TargetType.self | Card.TargetType.armyEntity));
                }
            }
            for (int j = 0; j < 8; j++)
            {
                Add(new Card(Card.Types.centerEvent, Career.none, new Effect(Effect.Types.armySwitch, 1), 1, Card.TargetType.self | Card.TargetType.armyEntity));
            }
            for (int j = 0; j < 8; j++)
            {
                Add(new Card(Card.Types.centerEvent, Career.none, new Effect(Effect.Types.drawCard, 1, 2), 1, Card.TargetType.self));
            }
            for (int j = 0; j < 8; j++)
            {
                Effect effect = new Effect(Effect.Types.addActionPoint, 1, -3);
                effect.SetSpecialName("cardMinusPoint");
                base.Add(new Card(Card.Types.centerEvent, Career.none, effect, 1, Card.TargetType.cardEntity));
            }
            for (int j = 0; j < 8; j++)
            {
                Effect effect = new Effect(PropertyType.attackRange | PropertyType.moveRange, BounesType.percentageIncrease, 1, -50, Effect.tc + ",1", Effect.statusAddingEffect + ",true", "onMove,true");
                effect.SetSpecialName("confusion");
                base.Add(new Card(Card.Types.centerEvent, Career.none, effect, 1, Card.TargetType.enemy | Card.TargetType.armyEntity));
            }
            Refresh();
        }

        public void Refresh()
        {
            for (int i = 0; i < 5 * Count; i++)
            {
                byte[] bytes = new byte[4];
                using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider()) { rng.GetBytes(bytes); }
                System.Random random = new System.Random(BitConverter.ToInt32(bytes, 0));
                int n1 = random.Next(0, Count);
                int n2 = random.Next(0, Count);
                Card c = this[n1];
                RemoveAt(n1);
                Insert(n2, c);
            }
        }

        public Card DrawCard()
        {
            if (Count == 0)
            {
                Generated();
            }
            Card card = this[Count - 1];
            Remove(card);
            return card;
        }

        private Card DrawCard(Effect.Types types)
        {
            if (Count == 0)
            {
                Generated();
            }
            for (int i = Count - 1; i > -1; i--)
            {
                Card card = this[i];
                if (card.Effect.Type == types)
                {
                    Remove(card);
                    return card;
                }
            }
            Generated();
            return DrawCard(types);
        }

        ///<summary> 获取手牌的方法 </summary>
        public void GetHandCard(Player player, int count)
        {
            Debug.Log(player.Name + ", " + count);
            for (int i = 0; i < count; i++)
            {
                Card card = DrawCard();
                card.Owner = player;
                player.AddHandCard(card);
            }
            Debug.Log(player.HandCard.Count);
        }

        ///<summary> 获取手牌的方法 </summary>
        public void GetHandCard(Player player, Effect.Types types, int count)
        {
            Debug.Log(player.Name + ", " + count);

            for (int i = 0; i < count; i++)
            {
                Card card = DrawCard(types);
                card.Owner = player;
                player.AddHandCard(card);
            }

            Debug.Log(player.HandCard.Count);
        }

    }
}

