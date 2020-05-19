using System;
using UnityEngine;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "NewResoncancePair", menuName = "Game Data/Resonance/ResoncancePair")]
    [Serializable]
    public class ResoncancePair : ScriptableObject, INameable
    {
        public Army.Types Types;
        public int count;
        public HalfResonance resonance;

        public string Name => Types.ToString();
    }
}

