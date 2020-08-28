using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem.AI
{
    public class AIEntity : PlayerEntity
    {
        public List<Entity> targets = new List<Entity>();
        public Dictionary<string, object> AIMindStorage = new Dictionary<string, object>();
        public bool isInAction;


        public Entity Target { get => targets.Count == 1 ? targets[0] : null; set => targets = new List<Entity> { value }; }
        public new Personality Personality => Owner.Personality;


        public override void Start()
        {
            if (Game.Configuration.PvP)
                enabled = false;

        }

        public override void Update()
        {

        }

        public IEnumerator Run(object[] vs)
        {
            //Debug.LogError(Game.CurrentBattle.CurrentStat);
            //Debug.LogError(Game.CurrentBattle.Round.CurrentStat);
            Debug.LogError("执行");

            if (Game.CurrentBattle.HasAnimation) yield return WaitForAnimation();

            //Debug.Log(Personality.HasFlag(Personality.dummy));
            //Debug.Log(Personality);

            if (Personality.HasFlag(Personality.dummy))
            {
                yield return DummyAction();
            }


            yield return Action(EndTurn);
        }

        private IEnumerator DummyAction()
        {
            Debug.LogError("执行DummyAction");
            isInAction = true;
            foreach (var item in Owner.BattleArmies)
            {
                yield return WaitForAnimation();
                Debug.Log("Dummy");
                switch (item.Autonomous)
                {
                    case BattleEntityData.AutonomousType.none:
                    case BattleEntityData.AutonomousType.idle:
                        Debug.Log("No action require");
                        break;
                    case BattleEntityData.AutonomousType.attack:
                        AIAction.ArmyAttack(item.Entity);
                        break;
                    case BattleEntityData.AutonomousType.patrol:
                        AIAction.ArmyPatrol(item.Entity);
                        break;
                    default:
                        break;
                }
            }
            yield return null;
        }

        protected IEnumerator WaitForAnimation()
        {
            while (Game.CurrentBattle.HasAnimation)
            {
                yield return new WaitForFixedUpdate();
            }
        }

        public virtual IEnumerator EndTurn(params object[] param)
        {
            Debug.LogError("开始回合结束");
            yield return Sleep(1);
            while (Game.CurrentBattle.HasAnimation)
            {
                yield return new WaitForSeconds(0.1f);
            }
            bool notEnd;

            do
            {
                notEnd = !Owner.TryEndTurn();
                yield return new WaitForSeconds(0.1f);
            } while (notEnd);

            yield return Sleep(1);
            isInAction = false;
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

            bool success = effect.Execute(true);
            if (success)
            {
                Owner.RemoveHandCard(card);
                card.Entity.Destroy();
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



    }

}