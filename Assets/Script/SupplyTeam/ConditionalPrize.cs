using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public class ConditionalPrize : Prize
    {
        [SerializeField] private List<string> leaderRequirement;
        [SerializeField] private string armyRequirement;
        [SerializeField] private TimeInterval time;

        public ConditionalPrize() { }
        public ConditionalPrize(string name, int count, Item.Type type, List<string> leaderRequirement, string armyRequirement, TimeInterval time)
        {
            this.name = name;
            this.count = count;
            this.type = type;
            this.leaderRequirement = leaderRequirement;
            this.armyRequirement = armyRequirement;
            this.time = time;
        }

        [Obsolete]
        public new ConditionalPrize Clone()
        {
            return new ConditionalPrize(name, count, type, leaderRequirement.Clone(), armyRequirement, time);
        }

        [Obsolete]
        public List<string> LeaderRequirement { get => leaderRequirement; set => leaderRequirement = value; }
        [Obsolete]
        public string ArmyRequirement { get => armyRequirement; set => armyRequirement = value; }
        [Obsolete]
        public TimeInterval Time { get => time; set => time = value; }

    }
}