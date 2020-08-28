using Canute.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.UI.StoryRoom
{
    public class StoryRoomUI : MonoBehaviour
    {
        public GameObject storyBlockUIPrefab;
        public Transform spawnAnchor;

        public List<SRStoryBlockUI> blocks;

        public void DisplayBlock()
        {
            foreach (var item in Game.PlayerData.CollectiveStoriesID)
            {
                var block = Instantiate(storyBlockUIPrefab, spawnAnchor).GetComponent<SRStoryBlockUI>();
                block.Display(item);
            }
        }

    }
}
