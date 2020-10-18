using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canute.UI.SupplyTeam
{
    [Obsolete]
    public class STExplanation : Icon
    {

        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Label.text = "Canute.SupplyTeam.Explanation".Lang();
        }

        public override void HideInfo()
        {
            base.HideInfo();
        }
    }
}
