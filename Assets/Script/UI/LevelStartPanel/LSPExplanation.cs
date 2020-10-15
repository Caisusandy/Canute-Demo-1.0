using UnityEngine;
using System.Collections;
using Canute.Module;
using Canute.StorySystem;
using Canute.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Canute.UI.LevelStart
{
    public class LSPExplanation : Icon
    {

        public override void OnMouseUp()
        {
            base.OnMouseUp();
            StoryDisplayer.LoadSceneIntro("SceneIntroLevelStartPanel");
        }
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Label.text = ("Explanation");
        }

    }
}
