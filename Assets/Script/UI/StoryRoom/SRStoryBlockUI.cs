using Canute.StorySystem;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.StoryRoom
{
    public class SRStoryBlockUI : MonoBehaviour
    {
        public bool initialized = true;
        public bool isReadyToDisplay;
        public string storyName;

        public Text displayingName;
        public Image icon;

        public Story story => Story.Get(storyName);

        public void Start()
        {
        }

        /// <summary>
        /// Load UI of Block
        /// </summary>
        /// <param name="storyName"></param>
        public void Display(string storyName)
        {
            initialized = true;
            this.storyName = storyName;
            displayingName.text = story.Lang("name");
        }

        private void OnMouseDown()
        {
            Canvas canvas = gameObject.AddComponent<Canvas>();
            canvas.overrideSorting = true;
            canvas.sortingLayerName = "UI";
            canvas.sortingOrder = 2;
            StoryRoomUI.instance.spawnAnchor.parent.parent.GetComponent<ScrollRect>().enabled = false;
        }

        /// <summary>
        /// when mouse is draging it
        /// </summary>
        private void OnMouseDrag()
        {
            Vector3 vector3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            vector3.z = transform.position.z;
            transform.position = vector3;

            float magnitude = (transform.position - StoryRoomUI.instance.slot.transform.position).magnitude;
            isReadyToDisplay = magnitude < 2.5 ? true : false;
        }

        private void OnMouseUp()
        {
            if (isReadyToDisplay)
            {
                ShowStory();
            }

            StoryRoomUI.instance.spawnAnchor.parent.parent.GetComponent<ScrollRect>().enabled = true;
            StoryRoomUI.instance.spawnAnchor.GetComponent<GridLayoutGroup>().enabled = false;
            StoryRoomUI.instance.spawnAnchor.GetComponent<GridLayoutGroup>().enabled = true;
            isReadyToDisplay = false;
            Destroy(GetComponent<Canvas>());
        }


        /// <summary>
        /// Open Story
        /// </summary>
        public void ShowStory()
        {
            StoryDisplayer.Load(story);
        }
    }
}
