using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public enum Career
    {
        none,
        scholar,
        politician,
        general,
        merchant,
        any,
    }

    public interface ICareerLabled
    {
        Career Career { get; }
    }

    public static class Careers
    {
        public static List<Career> AllCareer => new List<Career> { Career.scholar, Career.politician, Career.general, Career.merchant };

        public static Career RestrainBy(this Career career)
        {
            switch (career)
            {
                case Career.scholar:
                    return Career.merchant;
                case Career.general:
                    return Career.scholar;
                case Career.politician:
                    return Career.general;
                case Career.merchant:
                    return Career.politician;
                default:
                    Debug.LogWarning("Not a Normal Career?");
                    return Career.none;
            }
        }

        public static Career RestrainTo(this Career career)
        {
            switch (career)
            {
                case Career.scholar:
                    return Career.general;
                case Career.general:
                    return Career.politician;
                case Career.politician:
                    return Career.merchant;
                case Career.merchant:
                    return Career.scholar;
                default:
                    Debug.LogWarning("Not a Normal Career?");
                    return Career.none;
            }
        }
    }
}