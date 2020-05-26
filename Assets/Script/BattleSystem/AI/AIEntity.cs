using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem.AI
{
    public class AIEntity : PlayerEntity
    {
        public List<Entity> targets = new List<Entity>();
        public bool isInAction;

        public Entity Target { get => targets.Count == 1 ? targets[0] : null; set => targets = new List<Entity> { value }; }

        public override void Start()
        {
            if (Game.Configuration.PvP)
            {
                enabled = false;
            }
        }

        public override void Update()
        {
            if (InPlayerTurn)
            {
                Run();
            }
        }

        public virtual void Run()
        {
            if (isInAction)
            {
                return;
            }
            Action(EndTurn);
        }

        /// <summary>
        /// perform an action in coroutine
        /// </summary>
        /// <param name="action"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public override Coroutine Action(Func<object[], IEnumerator> action, params object[] param)
        {
            if (isInAction)
            {
                return null;
            }
            return base.Action(action, param);
        }


        /// <summary>
        /// executor of action, only use in Action(Func, params[])
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        protected override IEnumerator Action(IEnumerator enumerator)
        {
            Debug.Log("action start");
            isInAction = true;
            yield return enumerator;
            isInAction = false;
        }

        public virtual IEnumerator EndTurn(params object[] param)
        {
            yield return Sleep(1);
            Game.CurrentBattle.EndTurn();
            yield return true;
        }

        public virtual IEnumerator PlayCard(Card card)
        {
            if (!Owner.HandCard.Contains(card))
            {
                yield return false;
            }

            Effect effect = card.Effect;
            effect.Target = Target;

            yield return Sleep(1);

            bool success = effect.Execute();
            if (success)
            {
                Owner.RemoveHandCard(card);
            }
            yield return success;
        }

        public static new AIEntity Create(GameObject gameObject, Player player)
        {
            GameObject aiAnchor = new GameObject();
            aiAnchor.transform.SetParent(gameObject.transform);
            aiAnchor.name = player.Name;
            AIEntity ai = aiAnchor.AddComponent<AIEntity>();
            ai.data = player;

            return ai;
        }

        public enum PersonalityType
        {
            none,
        }


    }
}