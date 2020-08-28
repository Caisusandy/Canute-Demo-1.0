using System;
using System.Collections.Generic;
using UnityEngine;
namespace Canute.BattleSystem
{
    [Serializable]
    public class StatList : DataList<Status>
    {
        public List<Status> GetByCondition(TriggerCondition.Conditions conditions)
        {
            Clean();
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

        public bool HasCondition(TriggerCondition.Conditions conditions)
        {
            return !(GetByCondition(conditions) is null);
        }

        public void TotalClear()
        {
            base.Clear();
        }

        public void Clean()
        {
            //Debug.Log("Cleaning useless status");
            for (int i = Count - 1; i > -1; i--)
            {
                if (this[i].IsResonance)
                {
                    continue;
                }
                else if (this[i].NoMore || !this[i].IsValid)
                {
                    Debug.Log(this[i].NoMore);
                    Debug.Log(!this[i].IsValid);
                    Debug.Log("remove status:\n" + this[i].ToString());
                    RemoveAt(i);
                }
                else
                {
                    Debug.Log(this[i].ToString());
                }
            }
        }

        public static implicit operator StatList(List<Status> stats)
        {
            return new StatList(stats);
        }

        public static implicit operator List<Status>(StatList stats)
        {
            return stats.list;
        }

        public StatList(IEnumerable<Status> stats)
        {
            list = new List<Status>(stats);
        }

        public StatList() { }

        public StatList GetAllPermanentStatus()
        {
            StatList pStats = new StatList();
            foreach (Status item in this)
            {
                if (item.IsPermanentStatus)
                {
                    pStats.Add(item);
                }
            }
            return pStats;
        }

        public StatList GetAllTemporaryStatus()
        {
            StatList pStats = new StatList();
            foreach (Status item in this)
            {
                if (!item.IsPermanentStatus)
                {
                    pStats.Add(item);
                }
            }
            return pStats;
        }

        public object Clone()
        {
            return new StatList(((IEnumerable<Status>)this).Clone());
        }
    }
}