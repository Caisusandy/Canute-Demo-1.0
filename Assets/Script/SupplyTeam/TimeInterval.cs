using System;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public struct TimeInterval
    {
        [SerializeField] private int hours;
        [SerializeField] private int minutes;
        [SerializeField] private int seconds;

        public TimeInterval(int hours, int minutes, int seconds)
        {
            this.hours = hours;
            this.minutes = minutes;
            this.seconds = seconds;
        }

        public int Hours { get => hours; set => hours = value; }
        public int Minutes { get => minutes; set => minutes = value; }
        public int Seconds { get => seconds; set => seconds = value; }

        // [Obsolete]
        public static implicit operator TimeSpan(TimeInterval timeInterval)
        {
            return new TimeSpan(timeInterval.Hours, timeInterval.Minutes, timeInterval.Seconds);
        }

        // [Obsolete]
        public static implicit operator TimeInterval(TimeSpan timeInterval)
        {
            return new TimeInterval(timeInterval.Hours, timeInterval.Minutes, timeInterval.Seconds);
        }
    }
}