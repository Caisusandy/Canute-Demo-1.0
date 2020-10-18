using Canute.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canute.Module
{
    [Obsolete]
    class AutoLabel : Icon
    {
        public string key;
        [Obsolete]
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Label.text = key.Lang();
        }
    }
}
