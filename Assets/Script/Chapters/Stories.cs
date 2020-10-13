using Canute.LanguageSystem;
using Canute.Module;
using System;
using System.IO;
using System.Text;
using UnityEngine;

namespace Canute.StorySystem
{
    [CreateAssetMenu(fileName = "Stories", menuName = "Game Data/Stories", order = 6)]
    public class Stories : ScriptableObject, INameable
    {
        [SerializeField] protected Language language;
        [ContextMenuItem("Save", "SaveStory")]
        [SerializeField] protected StoryTree storyTree;
        [SerializeField] protected StoryTree uiEventStoryTree;
        [SerializeField] protected LetterTree letter;

        public string Name => language.ToString();
        public StoryTree StoryTree { get => storyTree; set => storyTree = value; }
        public StoryTree UIEventStoryTree { get => uiEventStoryTree; set => uiEventStoryTree = value; }
        public LetterTree Letters { get => letter; set => letter = value; }
        public Language Language { get => language; set => language = value; }
        public string DataPath => Application.persistentDataPath + "/Stories/";

        public Story Get(string name)
        {
            Story story = StoryTree.Get(name);
            if (story) return story;
            story = UIEventStoryTree.Get(name);
            if (story) return story;
            return Story.Empty;
        }


        /// <summary>
        /// save player file
        /// </summary>
        [ContextMenu("Save All Story")]
        public bool SaveStory()
        {
            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }

            foreach (var item in StoryTree)
            {
                WordLines wordLines = item.story;
                string json = JsonUtility.ToJson(wordLines);
                string filePath = DataPath + item.Name;
                string savePath = filePath + ".json";

                try
                {
                    File.WriteAllText(savePath, json, Encoding.UTF8);
                    Debug.Log("Saved!");
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return false;
                }
            }
            foreach (var item in UIEventStoryTree)
            {
                WordLines wordLines = item.story;
                string json = JsonUtility.ToJson(wordLines);
                string filePath = DataPath + "UI_" + item.Name;
                string savePath = filePath + ".json";

                try
                {
                    File.WriteAllText(savePath, json, Encoding.UTF8);
                    Debug.Log("Saved!");
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message);
                    return false;
                }
            }
            return true;
        }
    }

    [Serializable]
    public class StoryPacks : DataList<Stories> { }
}
