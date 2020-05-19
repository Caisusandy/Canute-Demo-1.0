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

        public List<Resonance> GetResonance(Army.Types types, int count)
        {
            List<Resonance> resoncances = new List<Resonance>();
            foreach (var item in resoncancePairs)
            {
                if (item.Types != types || item.count > count)
                {
                    continue;
                }
                resoncances.Add(item.resonance);
            }
            return resoncances;
        }

        [Serializable]
        class ResonanceList : DataList<ResoncancePair>
        {
            public ResoncancePair Get(Army.Types types)
            {
                return Get(types.ToString());
            }

        }

    }
}

