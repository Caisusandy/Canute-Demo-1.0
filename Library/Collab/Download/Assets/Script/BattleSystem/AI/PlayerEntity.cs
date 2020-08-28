using Canute.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class PlayerEntity : Entity
    {
        public Player data;
        public override EntityData Data => data;
        public bool InPlayerTurn => Owner.IsInTurn;
        public List<CardEntity> Cards => CardEntity.GetHandCard(Owner);

        public static PlayerEntity Create(GameObject gameObject, Player player)
        {
            GameObject aiAnchor = new GameObject();
            aiAnchor.transform.SetParent(gameObject.transform);
            aiAnchor.name = player.Name;
            PlayerEntity ai = aiAnchor.AddComponent<PlayerEntity>();
            ai.data = player;

            return ai;
        }


        [Flags]
        public enum Personality
        {
            actualPlayer,
            dummy = 1,
        }
    }
}