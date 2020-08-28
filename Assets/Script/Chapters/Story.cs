﻿using System;
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
    [Serializable]
    public struct Story : INameable, IEquatable<Story>
    {
        public static Story Empty => new Story();

        [SerializeField] private string id;
        [SerializeField] private StoryType type;
        [SerializeField] private WordLine[] wordLines;

        public Story(string id, WordLine[] wordLines) : this()
        {
            this.id = id;
            this.WordLines = wordLines;
        }

        public int LineCount => WordLines.Length;
        public string Name => id;
        public StoryType Type { get => type; set => type = value; }
        public WordLine CurrentLine { get; set; }
        public WordLine First { get { CurrentLine = WordLines[0]; return WordLines[0]; } }
        public WordLine[] WordLines { get => wordLines; set => wordLines = value; }

        public WordLine Next(int id = -1)
        {
            int nextIndex = id == -1 ? Array.IndexOf(WordLines, CurrentLine) + 1 : id;
            //Debug.Log(nextIndex);
            if (nextIndex < WordLines.Length)
            {
                CurrentLine = WordLines[nextIndex];
                return WordLines[nextIndex];
            }
            else
            {
                return WordLine.Empty;
            }
        }

        public WordLine Next()
        {
            int nextIndex = Array.IndexOf(WordLines, CurrentLine) + 1;
            //Debug.Log(nextIndex);
            if (nextIndex < WordLines.Length)
            {
                CurrentLine = WordLines[nextIndex];
                return WordLines[nextIndex];
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

        public static Story Get(string id)
        {
            return GameData.Stories.StoryTree.Get(id);
        }
    }

    public enum StoryType
    {
        normalStory,
        dailyConversation,
        selectionBase,
    }
}