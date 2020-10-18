using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.UI.Chapter
{
    public class ChapterUI : MonoBehaviour
    {
        public void DemoInfo()
        {
            InfoWindow.Create("Canute.Demo.Unfinished.info".Lang());
        }
    }
}
