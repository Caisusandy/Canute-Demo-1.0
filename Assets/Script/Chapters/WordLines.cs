using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.StorySystem
{
    [Serializable]
    public struct WordLines
    {
        [SerializeField] private string[] line;

        public static implicit operator WordLines(Story story)
        {
            List<string> strings = new List<string>();
            foreach (var item in story.WordLines)
            {
                strings.Add(item.Line);
            }
            return new WordLines() { line = strings.ToArray() };
        }
    }
}