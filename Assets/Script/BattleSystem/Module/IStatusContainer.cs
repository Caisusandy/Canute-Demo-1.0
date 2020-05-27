using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    public interface IStatusContainer : INameable
    {
        StatusList StatList { get; }
        StatusList GetAllStatus();
    }


    public static class StatusContainer
    {
        #region Trigger

        /// <summary> Trigger all effect that fit in a specific condition </summary>
        /// <param name="conditions"></param>
        public static void Trigger(this TriggerCondition.Conditions conditions)
        {
            foreach (IStatusContainer container in Game.CurrentBattle.StatusContainers)
            {
                StatusList stats = container.StatList.GetAllStatus(conditions);
                for (int i = stats.Count - 1; i >= 0; i--)
                {
                    Status stat = stats[i];

                    if (stat.TriggerConditions.CanTrigger())
                    {
                        stat.Execute();
                    }
                    else
                    {
                        //Debug.Log(item.Type + " is not available");
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

        public static void Trigger(this IStatusContainer statusContainer, TriggerCondition.Conditions condition, ref Effect effect)
        {
            StatusList stats = statusContainer.StatList?.GetAllStatus(condition);
            Debug.Log(condition.ToString() + " triggered. " + statusContainer.Name + "; status count:" + stats.Count);

            if (stats is null)
            {
                return;
            }

            for (int i = stats.Count - 1; i >= 0; i--)
            {
                Status item = stats[i];

                if (!item.TriggerConditions.CanTrigger(effect))
                {
                    Debug.LogWarning("An status cannot be triggered: " + item);
                    continue;
                }
                item.Execute(ref effect);
            }

            statusContainer.StatList.ClearInvalid();
        }

        public static void Trigger(this IStatusContainer statusContainer, TriggerCondition.Conditions condition)
        {
            StatusList stats = statusContainer.StatList?.GetAllStatus(condition);
            Debug.Log(statusContainer.Name + " triggered its effect in condition of " + condition.ToString() + "; status count:" + stats.Count);

            if (stats is null)
            {
                return;
            }

            for (int i = stats.Count - 1; i >= 0; i--)
            {
                Status item = stats[i];

                if (!item.TriggerConditions.CanTrigger())
                {
                    Debug.LogWarning("An status cannot be triggered: " + item);
                    continue;
                }
                item.Execute();
            }

            statusContainer.StatList.ClearInvalid();
        }

        /// <summary>
        /// try trigger all status
        /// </summary>
        /// <param name="statusContainer"></param>
        public static void TryTriggerAll(this IStatusContainer statusContainer)
        {
            StatusList stats = statusContainer.StatList;
            for (int i = stats.Count - 1; i >= 0; i--)
            {
                Status stat = stats[i];

                if (stat.TriggerConditions.CanTrigger())
                {
                    stat.Execute();
                    continue;
                }
                //Debug.Log(item.Type + " is not available");

            }
            statusContainer.StatList.ClearInvalid();
        }


        public static List<IStatusContainer> ToStatusContainers<T>(this IEnumerable<T> ts) where T : IStatusContainer
        {
            List<IStatusContainer> statusContainers = new List<IStatusContainer>();
            foreach (var item in ts)
            {
                statusContainers.Add(item);
            }
            return statusContainers;
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
                if (stat.IsBaseOnTurn || stat.IsDualBase)
                {
                    stat.TurnCount--;
                }
            }
            container.StatList.ClearInvalid();
        }
        #endregion
    }
}