using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    public interface IStatusContainer : INameable
    {
        /// <summary>
        /// Status in the container
        /// </summary>
        StatusList StatList { get; }
        /// <summary>
        /// Get all Status that effect a container directly (readonly)
        /// </summary>
        /// <returns></returns>
        StatusList GetAllStatus();
    }


    public static class Statuses
    {
        #region Trigger

        /// <summary> Trigger all effect that fit in a specific condition </summary>
        /// <param name="conditions"></param>
        public static void TriggerOf(this TriggerCondition.Conditions conditions)
        {
            foreach (IStatusContainer container in Game.CurrentBattle.StatusContainers)
            {
                StatusList stats = container.StatList.GetAllStatus(conditions);
                for (int i = stats.Count - 1; i >= 0; i--)
                {
                    Status stat = stats[i];

                    if (!stat.TriggerConditions.CanTrigger())
                    {
                        continue;
                    }
                    else if (stat.Type == Status.StatType.delay && stat.TurnCount != 1)
                    {
                        continue;
                    }
                    else
                    {
                        stat.Execute();
                    }
                }
                container.StatList.ClearInvalid();
            }
        }

        /// <summary>
        /// Try to trigger all status
        /// </summary>
        public static void TryTriggerAll()
        {
            foreach (IStatusContainer statusContainer in Game.CurrentBattle.StatusContainers)
            {
                statusContainer.TryTriggerAll();
            }
        }



        /// <summary>
        /// trigger with a reference effect
        /// attack triggers(4), move, entity arrive/left, adding status, play card 
        /// </summary>
        /// <param name="statusContainer"></param>
        /// <param name="condition"></param>
        /// <param name="effect"></param>
        public static void TriggerOf(this IStatusContainer statusContainer, TriggerCondition.Conditions condition, ref Effect effect)
        {
            StatusList stats = statusContainer.StatList?.GetAllStatus(condition);
            Debug.Log(condition.ToString() + " triggered. " + statusContainer.Name + "; status count:" + stats.Count);

            if (stats is null) return;

            for (int i = stats.Count - 1; i >= 0; i--)
            {
                Status item = stats[i];
                if (!item.TriggerConditions.CanTrigger(effect))
                {
                    Debug.LogWarning("An status cannot be triggered: " + item); continue;
                }
                else if (item.Type == Status.StatType.delay && item.TurnCount != 1) continue;

                item.Execute(ref effect);
            }
            statusContainer.StatList.ClearInvalid();
        }

        /// <summary>
        /// trigger without a reference effect (turn begin, turn end)
        /// </summary>
        /// <param name="statusContainer"></param>
        /// <param name="condition"></param>
        public static void TriggerOf(this IStatusContainer statusContainer, TriggerCondition.Conditions condition)
        {
            StatusList stats = statusContainer.StatList?.GetAllStatus(condition);
            Debug.Log(statusContainer.Name + " trigger status when" + condition.ToString() + "; status count =" + stats.Count);

            if (stats is null) return;

            for (int i = stats.Count - 1; i >= 0; i--)
            {
                Status item = stats[i];
                if (!item.TriggerConditions.CanTrigger())
                {
                    Debug.LogWarning("An status cannot be triggered: " + item); continue;
                }
                else if (item.Type == Status.StatType.delay && item.TurnCount != 1) continue;

                item.Execute();
            }
            statusContainer.StatList.ClearInvalid();
        }

        /// <summary> try trigger all status </summary> <param name="statusContainer"></param>
        public static void TryTriggerAll(this IStatusContainer statusContainer)
        {
            StatusList stats = statusContainer.StatList;
            for (int i = stats.Count - 1; i >= 0; i--)
            {
                Status stat = stats[i];

                if (!stat.TriggerConditions.CanTrigger())
                {
                    continue;
                }
                else if (stat.Type == Status.StatType.delay && stat.TurnCount != 1)
                {
                    continue;
                }
                stat.Execute();
                //Debug.Log(item.Type + " is not available");

            }
            statusContainer.StatList.ClearInvalid();
        }

        #endregion

        #region 回合计算Stat自减 
        /// <summary>
        /// Let all status that work to the player/ object that player own -1 turn (if is base on turn)
        /// </summary>
        /// <param name="player"></param>
        public static void StatTurnDecay(this Player player)
        {
            (player as IStatusContainer).TurnDecay();

            foreach (BattleArmy item in player.BattleArmies)
            {
                item.TurnDecay();
            }

            foreach (BattleBuilding item in player.Buildings)
            {
                item.TurnDecay();
            }
        }


        public static void TurnDecay(this IStatusContainer container)
        {
            foreach (Status stat in container.StatList)
            {
                if (stat.IsBaseOnTurn || stat.IsDualBase || stat.Type == Status.StatType.delay)
                {
                    stat.TurnCount--;
                }
            }
            container.StatList.ClearInvalid();
        }
        #endregion
    }
}