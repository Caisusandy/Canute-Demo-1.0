using System.IO;
using UnityEngine;

namespace Canute.StorySystem
{
    [CreateAssetMenu(fileName = "story", menuName = "Game Data/Story/Story Container")]
    public class StoryContainer : ScriptableObject, INameable
    {
        public Language language;
        public Story story;

        public string Name => story.Name;

        public static implicit operator Story(StoryContainer container)
        {
            return container ? container.story : Story.Empty;
        }

        [ContextMenu("Add To Story Pack")]
        public void AddToStoryPack()
        {
            if (language == GameData.Stories.Language) if (!GameData.Stories.StoryTree.Contains(this)) GameData.Stories.StoryTree.Add(this);
        }
        [ContextMenu("Auto Fill Id")]
        public void AutoFillId()
        {
            for (int i = 0; i < story.WordLines.Length; i++)
            {
                var temp = story.WordLines[i];
                temp.ID = story.Name + "." + (i + 1);
                story.WordLines[i] = temp;
            }
        }
        [ContextMenu("Export")]
        public void Export()
        {
            string json = JsonUtility.ToJson(story);
            if (!File.Exists(Application.persistentDataPath + "/Stories"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Stories");
            }
            Debug.Log(Application.persistentDataPath + "/Stories");
            File.WriteAllText(Application.persistentDataPath + "/Stories/" + story.Name + ".json", json);
        }
        [ContextMenu("Export Story Only")]
        public void ExportStoryOnly()
        {
            string json = JsonUtility.ToJson((WordLines)story);
            if (!File.Exists(Application.persistentDataPath + "/Stories"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Stories");
            }
            Debug.Log(Application.persistentDataPath + "/Stories");
            File.WriteAllText(Application.persistentDataPath + "/Stories/" + story.Name + "_line.json", json);
        }
    }
}