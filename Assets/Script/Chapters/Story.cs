using System;
using UnityEngine;

namespace Canute.StorySystem
{
    [Serializable]
    public struct Story : INameable, IEquatable<Story>
    {
        public static Story Empty => new Story();

        [SerializeField] private string id;
        [SerializeField] private WordLine[] wordLines;

        public Story(string id, WordLine[] wordLines) : this()
        {
            this.id = id;
            this.wordLines = wordLines;
        }

        public int LineCount => wordLines.Length;
        public string Name => id;
        public WordLine CurrentLine { get; set; }
        public WordLine First { get { CurrentLine = wordLines[0]; return wordLines[0]; } }

        public WordLine Next(int id = -1)
        {
            int nextIndex = id == -1 ? Array.IndexOf(wordLines, CurrentLine) + 1 : id;
            //Debug.Log(nextIndex);
            if (nextIndex < wordLines.Length)
            {
                CurrentLine = wordLines[nextIndex];
                return wordLines[nextIndex];
            }
            else
            {
                return WordLine.Empty;
            }
        }

        public WordLine Next()
        {
            int nextIndex = Array.IndexOf(wordLines, CurrentLine) + 1;
            //Debug.Log(nextIndex);
            if (nextIndex < wordLines.Length)
            {
                CurrentLine = wordLines[nextIndex];
                return wordLines[nextIndex];
            }
            else
            {
                return WordLine.Empty;
            }
        }

        public override bool Equals(object obj)
        {
            return obj is Story story && Equals(story);
        }

        public bool Equals(Story story)
        {
            return id == story.id;
        }

        public static implicit operator bool(Story story)
        {
            return !story.Equals(Empty);
        }

        public static bool operator ==(Story left, Story right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Story left, Story right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }
    }
}