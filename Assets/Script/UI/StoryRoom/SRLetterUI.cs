using Canute.StorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.StoryRoom
{
    public class SRLetterUI : MonoBehaviour
    {
        public bool initialized = true;
        public bool isReadyToDisplay;
        public string letterName;

        public Text displayingName;
        public Text author;
        public Image icon;

        public Letter letter => Letter.Get(letterName);

        public void Start()
        {
        }

        /// <summary>
        /// Load UI of Block
        /// </summary>
        /// <param name="letterName"></param>
        public void Display(string letterName)
        {
            initialized = true;
            this.letterName = letterName;
            displayingName.text = letter.Lang("name");
            author.text = letter.AuthorDisplayingName;
        }

        /// <summary>
        /// Open Letter
        /// </summary>
        public void ShowLetter()
        {
            LetterDisplayer.Load(letterName);
        }
    }
}
