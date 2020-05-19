
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
                    Add(new Card(Card.Types.normal, career, new Effect(Effect.Types.enterAttack, 1), 3, TargetType.self | TargetType.armyEntity));
                }
                for (int j = 0; j < 12; j++)
                {
                    Add(new Card(Card.Types.normal, career, new Effect(Effect.Types.enterMove, 1), 2, TargetType.self | TargetType.armyEntity));
                }
            }
            for (int j = 0; j < 8; j++)
            {
                Add(new Card(GameData.Prototypes.GetEventCardPrototype("armySwitch")));
                //Add(new Card(Card.Types.centerEvent, Career.none, new Effect(Effect.Types.armySwitch, 1), 1, TargetType.self | TargetType.armyEntity));
            }
            for (int j = 0; j < 8; j++)
            {
                Add(new Card(GameData.Prototypes.GetEventCardPrototype("drawCard")));
                //Add(new Card(Card.Types.centerEvent, Career.none, new Effect(Effect.Types.drawCard, 1, 2), 1, TargetType.self));
            }
            for (int j = 0; j < 8; j++)
            {
                Add(new Card(GameData.Prototypes.GetEventCardPrototype("cardMinusPoint")));
                //Effect effect = new Effect(Effect.Types.addActionPoint, 1, -3);
                //effect.SetSpecialName("cardMinusPoint");
                //base.Add(new Card(Card.Types.centerEvent, Career.none, effect, 1, TargetType.cardEntity));
            }
            for (int j = 0; j < 8; j++)
            {
                Add(new Card(GameData.Prototypes.GetEventCardPrototype("confusion")));
                //Effect effect = new Effect(PropertyType.attackRange | PropertyType.moveRange, BounesType.percentage, 1, -50, Effect.tc + ",1", Effect.statusAddingEffect + ",true", "onMove,true");
                //effect.SetSpecialName("confusion");
                //base.Add(new Card(Card.Types.centerEvent, Career.none, effect, 1, TargetType.enemy | TargetType.armyEntity));
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

