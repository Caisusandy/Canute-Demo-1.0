using Canute.Languages;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Canute.StorySystem
{
    [CreateAssetMenu(fileName = "Stories", menuName = "Game Data/Stories", order = 6)]
    public class Stories : ScriptableObject, INameable
    {
        [SerializeField] protected LanguageName language;
        [ContextMenuItem("Save", "SaveStory")]
        [SerializeField] protected StoryTree storyTree;

        public string Name => language.ToString();
        public StoryTree StoryTree { get => storyTree; set => storyTree = value; }
        public LanguageName Language { get => language; set => language = value; }
        public string DataPath => Application.persistentDataPath + "/Stories/";

        public void OnValidate()
        {

        }

        /// <summary>
        /// save player file
        /// </summary>
        public bool SaveStory()
        {
            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
            }

            foreach (var item in StoryTree)
            {
                string json = JsonUtility.ToJson(item.story);
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
            return true;
        }
    }

    [Serializable]
    public class StoryPacks : DataList<Stories> { }
}
