using System.IO;
using UnityEngine;

namespace Canute.StorySystem
{
    [CreateAssetMenu(fileName = "letter", menuName = "Game Data/Story/Letter Container")]
    public class LetterContainer : ScriptableObject, INameable
    {
        public Letter letter;

        public string Name => letter.Name;

        public static implicit operator Letter(LetterContainer container)
        {
            return container ? container.letter : Letter.Empty;
        }

        [ContextMenu("Export")]
        public void Export()
        {
            string json = JsonUtility.ToJson(letter);
            if (!File.Exists(Application.persistentDataPath + "/Stories"))
            {
                Directory.CreateDirectory(Application.persistentDataPath + "/Stories");
            }
            Debug.Log(Application.persistentDataPath + "/Stories");
            File.WriteAllText(Application.persistentDataPath + "/Stories/letter" + letter.Name + ".json", json);
        }
    }
}