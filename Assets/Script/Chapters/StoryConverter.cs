using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.StorySystem
{
    [CreateAssetMenu(fileName = "Story Converter", menuName = "Game Data/Story/Converter")]
    public class StoryConverter : ScriptableObject, INameable
    {
        public string storyName;
        public List<string> charaterNames;
        [TextArea(5, 50)]
        public string input;
        public StoryContainer output;

        public string Name => storyName;

        [ContextMenu("Convert Story")]
        public void ConvertStory()
        {
            List<WordLine> wordLines = new List<WordLine>();
            var lines = input.Split('\r', '\n').ToList();
            for (int i = lines.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(lines[i]))
                {
                    lines.RemoveAt(i);
                }
            }

            for (int i = 0; i < lines.Count; i++)
            {
                WordLine wordLine = new WordLine();
                for (int j = i; j < lines.Count; j++)
                {
                    string line = lines[j];
                    Debug.Log(line);
                    if (charaterNames.Where((name) => { return name.StartsWith(line); }).Count() > 0)
                    {
                        if (string.IsNullOrEmpty(wordLine.SpeakerName))
                            wordLine.SpeakerName = line;
                        else break;
                    }
                    else
                    {
                        wordLine.Line += line + "\n";
                        i++;
                    }
                }
                Debug.Log("WordLine End:" + i);
                wordLines.Add(wordLine);
            }

            var story = new Story(storyName, wordLines.ToArray());
            output.story = story;
        }
    }
}