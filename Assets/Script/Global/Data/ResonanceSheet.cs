using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "Resonance Sheet", menuName = "Game Data/Resonance/Resonance Sheet")]
    public class ResonanceSheet : ScriptableObject
    {
        [SerializeField] private ResonanceList resoncancePairs;
        private ResonanceList ResoncancePairs { get => resoncancePairs; set => resoncancePairs = value; }

        public List<ResonancePair> GetResonance(Army.Types types, int count)
        {
            List<ResonancePair> resoncances = new List<ResonancePair>();
            foreach (var item in resoncancePairs)
            {
                if (item.ArmyType != types || item.Count > count)
                {
                    continue;
                }
                resoncances.Add(item);
            }
            return resoncances;
        }

        [Serializable]
        class ResonanceList : DataList<ResonancePair>
        {
            public ResonancePair Get(Army.Types types)
            {
                return Get(types.ToString());
            }

        }

    }
}

