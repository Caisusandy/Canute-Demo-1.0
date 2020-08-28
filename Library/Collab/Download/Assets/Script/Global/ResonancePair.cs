using System;
using UnityEngine;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "NewResoncancePair", menuName = "Game Data/Resonance/ResoncancePair")]
    [Serializable]
    public class ResonancePair : ScriptableObject, INameable
    {
        [SerializeField] private Army.Types type;
        [SerializeField] private int count;
        [SerializeField] private Resonance.ResonanceTarget target;
        [SerializeField] private HalfResonance resonance;

        public string Name => ArmyType.ToString();

        public Army.Types ArmyType { get => type; set => type = value; }
        public int Count { get => count; set => count = value; }
        public Resonance.ResonanceTarget Target { get => target; set => target = value; }
        public HalfResonance Resonance { get => resonance; set => resonance = value; }
    }
}

