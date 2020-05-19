using System;
using System.Collections.Generic;
using UnityEngine;
namespace Canute.BattleSystem
{
    [Serializable]
    public class StatusList : DataList<Status>
    {
        public StatusList GetByCondition(TriggerCondition.Conditions conditions)
        {
            ClearInvalid();
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

        public override void Add(Status item)
        {
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
        }

        /// <summary>
        /// will not clear permanent status and resonance
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

        public StatusList GetStatus(Effect.Types types)
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

        public List<Status> GetStatus(Effect.Types types, params Arg[] args)
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

        public List<Status> GetAllTags()
        {
            List<Status> list = new StatusList();
            foreach (var item in this)
            {
                if (item.Effect.Type == Effect.Types.tag)
                {
                    list.Add(item);
                }
            }
            return list;
        }

        public void TotalClear()
        {
            base.Clear();
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
                    Debug.Log(this[i].ToString());
                }
            }
        }

        public bool HasCondition(TriggerCondition.Conditions conditions)
        {
            return !(GetByCondition(conditions) is null);
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