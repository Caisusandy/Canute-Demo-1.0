using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.UI.Levels
{
    public class ChapterButtons : MonoBehaviour
    {
        public ChapterButtonPair[] ChapterButtonPair;

        public void Awake()
        {
            foreach (var item in ChapterButtonPair)
            {
                bool open = GameData.Levels.GetLevel(item.levelName)?.LastLevel?.IsPassed == true;
                open |= Game.PlayerData.PlayerChapterTreeStat.Get(item.levelName).IsPassed == true;
                item.button.interactable = open;
            }
        }
    }
}
