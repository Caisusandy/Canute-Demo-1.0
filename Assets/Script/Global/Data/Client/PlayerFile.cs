using Canute.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Canute
{
    public static class PlayerFile
    {
        private static Data data;

        public static string DataPath => Application.persistentDataPath + "/Saves/";

        public static Data Data { get => GetData(); private set { data = value; Game.Configuration.LastGame = value.UUID; Game.SaveConfig(); GameData.instance.data = value; } }

        private static Data GetData()
        {
            return data ?? ContinueLastSaved();
        }

        /// <summary>
        /// save current player file
        /// </summary>
        public static bool SaveCurrentData()
        {
            Data.LastOperationTime = DateTime.Now;
            string json = JsonUtility.ToJson(Data);
            string filePath = DataPath + data.UUID;
            string savePath = filePath + "/Data.json";

            if (!Directory.Exists(DataPath + data.UUID))
            {
                Directory.CreateDirectory(filePath);
            }

            try
            {
                File.WriteAllText(savePath, json, Encoding.UTF8);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }
        /// <summary>
        /// save player file
        /// </summary>
        public static bool SaveData(Data data)
        {
            data.LastOperationTime = DateTime.Now;
            string json = JsonUtility.ToJson(data);
            string filePath = DataPath + data.UUID;
            string savePath = filePath + "/Data.json";

            if (!Directory.Exists(DataPath + data.UUID))
            {
                Directory.CreateDirectory(filePath);
            }

            try
            {
                File.WriteAllText(savePath, json, Encoding.UTF8);
                Debug.Log("Saved!");
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                return false;
            }
        }

        /// <summary>
        /// load data from local storage
        /// </summary>
        /// <param name="uuid">name of the folder</param>
        /// <returns></returns>
        public static bool LoadData(UUID uuid)
        {
            string path = DataPath + uuid + "/Data.json";

            if (!File.Exists(path))
            {
                Debug.Log("Save not found: " + path);
                return false;
            }

            string json = File.ReadAllText(path, Encoding.UTF8);
            var data = JsonUtility.FromJson<Data>(json);
            if (data is null) { return false; };
            if (((DateTime)data.LastOperationTime).ToUniversalTime() > DateTime.UtcNow)
            {
                var info = InfoWindow.Create(GameServer.instance.transform, "This player file seems to have an inappropriate time! Tried to reload this in " + ((int)(data.LastOperationTime - DateTime.UtcNow).TotalHours) + " hours");
                UnityEngine.Object.Destroy(info.GetComponent<GraphicRaycaster>());
                UnityEngine.Object.Destroy(info.GetComponent<Canvas>());
                return false;

            }
            Data = data;

            Data.RemoveInvalid();
            Game.Configuration.LastGame = Data.UUID;
            Game.SaveConfig();
            return true;
        }

        /// <summary>
        /// create a new player file
        /// </summary>
        /// <returns></returns>
        public static Data CreateNewPlayerFile()
        {
            Debug.Log("try create player file");
            Data = new Data(Guid.NewGuid());
            string filePath = DataPath + data.UUID;
            Directory.CreateDirectory(filePath);
            Debug.Log(filePath);
            File.Create(filePath + "/Data.json").Dispose();

            SaveCurrentData();
            return data;
        }

        public static Data ContinueLastSaved()
        {
            bool result = LoadData(Game.Configuration.LastGame);
            if (result)
                return data;
            else
                SceneControl.GotoSceneImmediate(MainScene.gameStart);

            return null;
        }

        public static List<Save> GetAllSaves()
        {
            List<Save> saves = new List<Save>();
            if (!Directory.Exists(DataPath))
            {
                Directory.CreateDirectory(DataPath);
                return saves;
            }

            string[] folders = Directory.GetDirectories(DataPath);
            foreach (var item in folders)
            {
                string path = item + "/Data.json";
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    Save save = JsonUtility.FromJson<Save>(json);
                    save.filePath = path;
                    saves.Add(save);
                }
            }
            Debug.Log("Current saves count : " + saves.Count);
            saves.Sort();
            saves.Reverse();
            return saves;
        }
    }

    [Serializable]
    public class Save : IUUIDLabeled, IComparable<Save>, IComparable<Data>
    {
        public UUID uuid;
        public WorldTime playerLastOperationTime;
        public int federgram;
        public int manpower;
        public int mantleAlloy;
        public int aethium;
        public PlayerChapterTree gameProgress;

        public string filePath;

        public DateTime LastOperationTime => playerLastOperationTime;
        public UUID UUID { get => uuid; set => uuid = value; }


        public Data GetData()
        {
            string path = PlayerFile.DataPath + uuid + "/Data.json";
            if (!File.Exists(path))
            {
                Debug.Log("Save not found");
                return null;
            }
            string json = File.ReadAllText(path, Encoding.UTF8);
            return JsonUtility.FromJson<Data>(json);
        }
        public int CompareTo(Save other)
        {
            if (LastOperationTime > other.LastOperationTime) { return 1; }
            if (LastOperationTime < other.LastOperationTime) { return -1; }
            return 0;
        }

        public int CompareTo(Data other)
        {
            if (LastOperationTime > other.LastOperationTime) { return 1; }
            if (LastOperationTime < other.LastOperationTime) { return -1; }
            return 0;
        }

        public string NextLevelName()
        {
            foreach (var item in GameData.Levels.ChapterTree.CurrentStory)
            {
                if (gameProgress.Contains(item))
                {
                    continue;
                }
                return item.Name;
            }
            return string.Empty;
        }
    }
}