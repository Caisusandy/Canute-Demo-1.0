using Canute.LanguageSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.StoryRoom
{
    public class StoryRoomUI : MonoBehaviour
    {
        public static StoryRoomUI instance;

        public enum StoryRoomType
        {
            letters,
            blocks,
        }

        public StoryRoomType roomType;
        public GameObject storyBlockUIPrefab;
        public GameObject letterUIPrefab;
        public Transform spawnAnchor;
        public GameObject slot;

        public List<GameObject> blocks;

        public void Awake()
        {
            instance = this;
        }

        public void Start()
        {
            Display(StoryRoomType.blocks);
        }

        private void Display(StoryRoomType roomType)
        {
            switch (roomType)
            {
                case StoryRoomType.letters:
                    DisplayLetters();
                    break;
                case StoryRoomType.blocks:
                    DisplayBlock();
                    break;
                default:
                    break;
            }
        }

        public void ClearDisplay()
        {
            foreach (var item in blocks)
            {
                Destroy(item);
            }
            blocks.Clear();
        }

        public void DisplayBlock()
        {
            if (roomType == StoryRoomType.blocks)
                return;

            ClearDisplay();
            roomType = StoryRoomType.blocks;
            spawnAnchor.gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(150, 150);
            spawnAnchor.gameObject.GetComponent<GridLayoutGroup>().constraintCount = 5;
            foreach (var item in Game.PlayerData.CollectionStoriesID)
            {
                var block = Instantiate(storyBlockUIPrefab, spawnAnchor).GetComponent<SRStoryBlockUI>();
                block.Display(item);
                blocks.Add(block.gameObject);
            }
            Debug.Log("load block complete");
        }

        public void DisplayLetters()
        {
            if (roomType == StoryRoomType.letters)
                return;

            ClearDisplay();
            roomType = StoryRoomType.letters;
            spawnAnchor.gameObject.GetComponent<GridLayoutGroup>().cellSize = new Vector2(250, 180);
            spawnAnchor.gameObject.GetComponent<GridLayoutGroup>().constraintCount = 3;
            foreach (var item in Game.PlayerData.CollectionLetterID)
            {
                var block = Instantiate(letterUIPrefab, spawnAnchor).GetComponent<SRLetterUI>();
                block.Display(item);
                blocks.Add(block.gameObject);
            }
            Debug.Log("load letters complete");
        }
    }
}
