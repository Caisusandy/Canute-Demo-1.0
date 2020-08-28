using System.IO;
using UnityEngine;

namespace Canute.StorySystem
{
    [CreateAssetMenu(fileName = "story", menuName = "Game Data/Story/Story Container")]
    public class StoryContainer : ScriptableObject, INameable
    {
        public Story story;

        public string Name => story.Name;

        public static implicit operator Story(StoryContainer container)
        {
            return container.story;
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