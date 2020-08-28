using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.Testing
{
    public class BuildTest : MonoBehaviour
    {
        public InputField aethium;
        public InputField mantleAlloy;
        public InputField federgram;
        public InputField manpower;

        public int common = 0;
        public int rare = 0;
        public int epic = 0;
        public int legend = 0;

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        //public void Build()
        //{
        //    foreach (int item in new int[1000])
        //    {

        //        var a = Shops.Build.BuildPrize(uint.Parse(aethium.text), uint.Parse(mantleAlloy.text), uint.Parse(manpower.text), uint.Parse(federgram.text));
        //        switch (a.rarity)
        //        {
        //            case Rarity.common:
        //                common++;
        //                break;
        //            case Rarity.rare:
        //                rare++;
        //                break;
        //            case Rarity.epic:
        //                epic++;
        //                break;
        //            case Rarity.legendary:
        //                legend++;
        //                break;
        //            default:
        //                break;
        //        }

        //    }
        //    Debug.Log("C " + common + "; R " + rare + "; E " + epic + "; L" + legend);
        //    var total = common + rare + epic + legend;
        //    Debug.Log("C " + common * 1f / total + "; R " + rare * 1f / total + "; E " + epic * 1f / total + "; L" + legend * 1f / total);

        //}
    }
}