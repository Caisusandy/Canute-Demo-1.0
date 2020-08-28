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
            if (InPlayerTurn) Run();
        }

        public void Run()
        {
            if (isInAction) return;

            Debug.Log(Personality.HasFlag(Personality.dummy));
            Debug.Log(Personality);
            if (Personality.HasFlag(Personality.dummy))
            {
                Debug.Log("Dummy");
                foreach (var item in Owner.BattleArmies)
                {
                    switch (item.Autonomous)
                    {
                        case BattleableEntityData.AutonomousType.none:
                        case BattleableEntityData.AutonomousType.idle:
                            Debug.Log("No action require");
                            break;
                        case BattleableEntityData.AutonomousType.attack:
                            AIAction.ArmyAttack(item.Entity);
                            break;
                        case BattleableEntityData.AutonomousType.patrol:
                            AIAction.ArmyPatrol(item.Entity);
                            break;
                        default:
                            break;
                    }
                }
            }
            else
            {

            }
            Action(EndTurn);
        }

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