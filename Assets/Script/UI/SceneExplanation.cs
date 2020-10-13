using Canute.Module;
using Canute.StorySystem;
using Canute.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Canute.Assets.Script.UI
{
    public class SceneExplanation : Icon
    {
        private static string SceneName => SceneManager.GetActiveScene().name;

        public override void OnMouseUp()
        {
            base.OnMouseUp();
            StoryDisplayer.LoadSceneIntro("SceneIntro" + SceneName);
        }
        public override void DisplayInfo()
        {
            base.DisplayInfo();
            Label.text = ("Canute." + SceneName + ".Explanation").Lang();
        }

    }
}
