using Canute.LevelTree;
using System;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public struct LevelInfo : INameable, IEquatable<LevelInfo>
    {
        [SerializeField] private string name;
        [SerializeField] private bool isPassed;

        public LevelInfo(string name, bool isPassed)
        {
            this.name = name;
            this.isPassed = isPassed;
        }

        public string Name => name;
        public bool IsPassed => isPassed;
        public Level Level => GameData.Levels.GetLevel(Name);

        public static implicit operator bool(LevelInfo levelInfo)
        {
            return !(levelInfo.Level is null);
        }

        public bool Equals(LevelInfo other)
        {
            return (other.name == name) ? (other.isPassed == isPassed) : false;
        }
    }
}