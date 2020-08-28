using Canute.Languages;
using Canute.StorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.StoryRoom
{
    public class SRStoryBlockUI : MonoBehaviour
    {
        public string storyName;
        public Text text;
        public Image image;

        public Story story => Story.Get(storyName);

        public void Display(string storyName)
        {
            this.storyName = storyName;
            text.text = story.Lang("name");
        }

    }
}
