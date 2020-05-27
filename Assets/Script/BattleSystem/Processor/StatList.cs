using System;
using System.Collections.Generic;
using UnityEngine;
namespace Canute.BattleSystem
{
    [Serializable]
    public class StatusList : DataList<Status>
    {
        /// <summary>
        /// add a status
        /// </summary>
        /// <param name="item"></param>
        public override void Add(Status item)
        {
            if (item.Effect.Type == Effect.Types.none)
            {
                return;
            }
            //Debug.Log("add " + item.ToString());
            foreach (var stat in this)
            {
                if (stat.SimilarTo(item))
                {
                    bool sucess = stat.Merge(item);
                    if (!sucess)
                    {
                        Debug.LogWarning("Trying to merge 2 status result a failure " + stat.ToString() + "\n" + item.ToString());
                    }
                    return;
                }
            }
            base.Add(item);
            ClearInvalid();
        }

        /// <summary>
        /// Clear the status list
        /// <para>: will not clear permanent status and resonance</para>
        /// </summary>
        public override void Clear()
        {
            for (int i = Count - 1; i >= 0; i--)
            {
                if (this[i].IsPermanentStatus)
                {
                    continue;
                }
                if (this[i].IsResonance)
                {
                    continue;
                }
                RemoveAt(i);
            }
        }

        /// <summary>
        /// Clear the status list
        /// <para>: clear permanent status and resonance depend on <paramref name="clearPermanent"/></para>
        /// </summary>
        /// <param name="clearPermanent"></param>
        public void Clear(bool clearPermanent = false)
        {
            if (clearPermanent)
            {
                base.Clear();
            }
            else
            {
                for (int i = Count - 1; i >= 0; i--)
                {
                    if (this[i].IsPermanentStatus)
                    {
                        continue;
                    }
                    if (this[i].IsResonance)
                    {
                        continue;
                    }
                    RemoveAt(i);
                }
            }
        }

        public bool HasStatus(Effect.Types types)
        {
            foreach (var item in this)
            {
                if (item.Effect.Type == types)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasStatus(Effect.Types types, params Arg[] args)
        {
            foreach (var item in this)
            {
                if (item.Effect.Type == types)
                {
                    foreach (var arg in args)
                    {
                        if (item.Effect[arg.Key] != arg.Value)
                        {
                            goto next;
                        }
                    }
                    return true;
                }
                next:
                continue;
            }
            return false;
        }

        /// <summary>
        /// Get all status that have <paramref name="types"/>
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public List<Status> GetAllStatus(Effect.Types types)
        {
            StatusList statuses = new StatusList();
            foreach (var item in this)
            {
                if (item.Effect.Type == types)
                {
                    statuses.Add(item);
                }
            }
            return statuses;
        }

        /// <summary>
        /// Get all status that have <paramref name="conditions"/>
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public List<Status> GetAllStatus(TriggerCondition.Conditions conditions)
        {
            List<Status> stats = new List<Status>();
            for (int i = 0; i < Count; i++)
            {
                Status item = this[i];
                if (item.TriggerConditions?.HasCondition(conditions) == true)
                {
                    stats.Add(item);
                }
            }
            return stats;
        }

        /// <summary>
        /// Get all status that have <paramref name="types"/> and <paramref name="args"/>
        /// </summary> 
        /// <param name="types"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<Status> GetAllStatus(Effect.Types types, params Arg[] args)
        {
            List<Status> statuses = new List<Status>();
            foreach (var item in this)
            {
                if (item.Effect.Type == types)
                {
                    foreach (var arg in args)
                    {
                        if (item.Effect[arg.Key] != arg.Value)
                        {
                            goto next;
                        }
                    }
                    statuses.Add(item);
                }
                next:
                continue;
            }
            return statuses;
        }

        /// <summary>
        /// Get all status that have <paramref name="types"/>, <paramref name="conditions"/>, and <paramref name="args"/>
        /// </summary> 
        /// <param name="types"></param>
        /// <param name="conditions"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public List<Status> GetAllStatus(Effect.Types types, TriggerCondition.Conditions conditions, params Arg[] args)
        {
            List<Status> statuses = new List<Status>();
            foreach (var item in this)
            {
                if (item.TriggerConditions?.HasCondition(conditions) != true)
                {
                    continue;
                }

                if (item.Effect.Type == types)
                {
                    foreach (var arg in args)
                    {
                        if (item.Effect[arg.Key] != arg.Value)
                        {
                            goto next;
                        }
                    }
                    statuses.Add(item);
                }
                next:
                continue;
            }
            return statuses;
        }

        /// <summary>
        /// Get the first status that have <paramref name="types"/>
        /// </summary>
        /// <param name="types"></param>
        /// <returns></returns>
        public Status GetStatus(Effect.Types types)
        {
            foreach (var item in this)
            {
                if (item.Effect.Type == types)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the first status that have <paramref name="conditions"/>
        /// </summary>
        /// <param name="conditions"></param>
        /// <returns></returns>
        public Status GetStatus(TriggerCondition.Conditions conditions)
        {
            for (int i = 0; i < Count; i++)
            {
                Status item = this[i];
                if (item.TriggerConditions?.HasCondition(conditions) == true)
                {
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// Get the first status that have <paramref name="types"/> and <paramref name="args"/>
        /// </summary> 
        /// <param name="types"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Status GetStatus(Effect.Types types, params Arg[] args)
        {
            foreach (var item in this)
            {
                if (item.Effect.Type == types)
                {
                    foreach (var arg in args)
                    {
                        if (item.Effect[arg.Key] != arg.Value)
                        {
                            goto next;
                        }
                    }
                    return item;
                }
                next:
                continue;
            }
            return null;
        }

        /// <summary>
        /// Get the first status that have <paramref name="types"/>, <paramref name="conditions"/>, and <paramref name="args"/>
        /// </summary> 
        /// <param name="types"></param>
        /// <param name="conditions"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public Status GetStatus(Effect.Types types, TriggerCondition.Conditions conditions, params Arg[] args)
        {
            List<Status> statuses = new List<Status>();
            foreach (var item in this)
            {
                if (item.TriggerConditions?.HasCondition(conditions) != true)
                {
                    continue;
                }

                if (item.Effect.Type == types)
                {
                    foreach (var arg in args)
                    {
                        if (item.Effect[arg.Key] != arg.Value)
                        {
                            goto next;
                        }
                    }
                    return item;
                }
                next:
                continue;
            }
            return null;
        }

        /// <summary>
        /// Get all permanent status (the effect that will not be cleared)
        /// </summary>
        /// <returns></returns>
        public List<Status> GetAllPermanentStatus()
        {
            List<Status> pStats = new StatusList();
            foreach (Status item in this)
            {
                if (item.IsPermanentStatus)
                {
                    pStats.Add(item);
                }
            }
            return pStats;
        }

        /// <summary>
        /// Get all temporary status (the effect that can be cleared)
        /// </summary>
        /// <returns></returns>
        public List<Status> GetAllTemporaryStatus()
        {
            List<Status> pStats = new StatusList();
            foreach (Status item in this)
            {
                if (!item.IsPermanentStatus)
                {
                    pStats.Add(item);
                }
            }
            return pStats;
        }

        /// <summary>
        /// Get all status(tag)
        /// </summary>
        /// <returns></returns>
        public List<Status> GetAllTags() => GetAllStatus(Effect.Types.tag);

        /// <summary>
        /// Get all status(event)
        /// </summary>
        /// <returns></returns>
        public List<Status> GetAllEvent() => GetAllStatus(Effect.Types.@event);

        /// <summary>
        /// Return the first status(tag) in the list that matches <paramref name="args"/>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Status GetTag(params Arg[] args)
        {
            foreach (var item in GetAllTags())
            {
                bool match = true;
                foreach (var arg in args)
                {
                    if (!item.Effect.Args.HasParam(arg.Key, arg.Value))
                    {
                        match = false;
                    }
                }
                if (match)
                {
                    return item;
                }
            }
            return default;
        }

        /// <summary>
        /// Return the first status(event) in the list that matches <paramref name="args"/>
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public Status GetEvent(params Arg[] args)
        {
            foreach (var item in GetAllEvent())
            {
                bool match = true;
                foreach (Arg arg in args)
                {
                    if (!item.Effect.Args.HasParam(arg.Key, arg.Value))
                    {
                        match = false;
                    }
                }
                if (match)
                {
                    return item;
                }
            }
            return default;
        }

        public void ClearInvalid()
        {
            //Debug.Log("Cleaning useless status");
            for (int i = Count - 1; i > -1; i--)
            {
                if (this[i].IsResonance)
                {
                    continue;
                }
                else if (this[i].NoMore)
                {
                    Debug.LogWarning("remove status:\n" + this[i].ToString());
                    RemoveAt(i);
                }
                else
                {
                    //effect is valid
                    //Debug.Log(this[i].ToString());
                }
            }
        }

        public bool HasCondition(TriggerCondition.Conditions conditions)
        {
            return !(GetAllStatus(conditions) is null);
        }


        public static implicit operator StatusList(List<Status> stats)
        {
            return new StatusList(stats);
        }

        public static implicit operator List<Status>(StatusList stats)
        {
            return stats.list;
        }

        public StatusList(IEnumerable<Status> stats)
        {
            list = new List<Status>(stats);
        }

        public StatusList() { }

        public object Clone()
        {
            return new StatusList(((IEnumerable<Status>)this).Clone());
        }
    }
}