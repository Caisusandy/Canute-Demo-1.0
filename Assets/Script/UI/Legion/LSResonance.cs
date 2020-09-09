using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.UI.Legion
{
    public class LSResonance : MonoBehaviour
    {
        public GameObject prefab;
        public GameObject anchor;

        public void LoadResonance()
        {
            Clear();
            var resonanceInfo = Resonance.GetResonance(LSLegionDisplay.instance.Legion.Armies);
            foreach (var item in resonanceInfo)
            {
                var resonances = GameData.ResonanceSheet.GetResonance(item.Key, item.Value);
                foreach (var resonance in resonances)
                {
                    var go = Instantiate(prefab, anchor.transform).GetComponent<LSResonanceInfo>();
                    go.ResonancePair = resonance;
                    go.Display();
                }
            }
        }

        public void Clear()
        {
            foreach (Transform resonance in anchor.transform)
            {
                Destroy(resonance.gameObject);
            }
        }
    }
}
