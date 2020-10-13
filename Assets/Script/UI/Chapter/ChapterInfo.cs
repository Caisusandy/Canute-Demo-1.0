using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.Assets.Script.UI.Chapter
{
    public class ChapterInfo : MonoBehaviour
    {
        public Text info;
        public Text des;
        public string chapterName;

        private void OnMouseEnter()
        {
            info.text = "Canute.LevelTree.Level.ChapterName.name".Replace("ChapterName", chapterName).Lang();
            des.text = "Canute.LevelTree.Level.ChapterName.description".Replace("ChapterName", chapterName).Lang();
        }

        private void OnMouseExit()
        {

            info.text = "";
            des.text = "";
        }
    }
}
