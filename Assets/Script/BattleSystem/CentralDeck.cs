
using Canute.Module;
using System;
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
                    Add(new Card(Card.Types.normal, career, new Effect(Effect.Types.enterAttack, 1), 3, TargetType.self | TargetType.armyEntity));
                }
                for (int j = 0; j < 12; j++)
                {
                    Add(new Card(Card.Types.normal, career, new Effect(Effect.Types.enterMove, 1), 2, TargetType.self | TargetType.armyEntity));
                }
            }
            for (int j = 0; j < 8; j++)
            {
                Add(new Card(Card.Types.centerEvent, GameData.Prototypes.GetEventCardPrototype("armySwitch")));
            }
            for (int j = 0; j < 8; j++)
            {
                Add(new Card(Card.Types.centerEvent, GameData.Prototypes.GetEventCardPrototype("drawCard")));
            }
            for (int j = 0; j < 8; j++)
            {
                Add(new Card(Card.Types.centerEvent, GameData.Prototypes.GetEventCardPrototype("cardMinusPoint")));
            }
            for (int j = 0; j < 8; j++)
            {
                Add(new Card(Card.Types.centerEvent, GameData.Prototypes.GetEventCardPrototype("confusion")));
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

        public Card DrawCard(Effect.Types types)
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
    }
}

