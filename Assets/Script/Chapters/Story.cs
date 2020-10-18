using System;
using UnityEngine;

namespace Canute.StorySystem
{

    [Serializable]
    public struct Story : INameable, IEquatable<Story>
    {
        public static Story Empty => new Story();

        [SerializeField] private string id;
        [SerializeField] private StoryType type;
        [SerializeField] private WordLine[] wordLines;

        public Story(string id, params WordLine[] wordLines) : this()
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
            if (CurrentLine.NextLineID != string.Empty)
            {
                if (CurrentLine.NextLineID == "End")
                {
                    return WordLine.Empty;
                }
                else
                {
                    CurrentLine = Find(CurrentLine.NextLineID);
                    return CurrentLine;
                }
            }
            int nextIndex = Array.IndexOf(WordLines, CurrentLine) + 1;

            //Debug.Log(nextIndex);
            if (nextIndex < WordLines.Length)
            {
                CurrentLine = WordLines[nextIndex];
                return CurrentLine;
            }
            else
            {
                return WordLine.Empty;
            }
        }

        public WordLine Find(string id)
        {
            foreach (var item in WordLines)
            {
                if (item.ID == id) { return item; }
            }
            return WordLine.Empty;
        }

        public WordLine ContinueFrom(string id)
        {
            CurrentLine = Find(id);
            return CurrentLine;
        }

        public bool Equals(Story story)
        {
            return id == story.id;
        }

        public override bool Equals(object obj)
        {
            return obj is Story story && Equals(story);
        }

        public override int GetHashCode()
        {
            return id.GetHashCode();
        }

        public override string ToString()
        {
            return id;
        }


        public static implicit operator bool(Story story)
        {
            if (story.Name == "Empty") return false;
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

        public static Story Get(string id)
        {
            return GameData.Stories.Get(id);
        }
    }

    [Serializable]
    public struct Letter : INameable
    {
        public static Letter Empty => new Letter();

        [SerializeField] private string id;
        [SerializeField] private string author;
        [SerializeField] private string title;
        [SerializeField] private Sprite background;
        [SerializeField, TextArea(3, 50)] private string text;

        public string Name => id;
        public string Text { get => text; set => text = value; }
        public string Author { get => author; set => author = value; }
        public string Title { get => title; set => title = value; }
        public string AuthorDisplayingName => ("Canute.Leader." + Author + ".name").Lang();
        public Sprite Background { get => background; set => background = value; }

        public bool Equals(Letter letter)
        {
            return id == letter.id;
        }

        public override bool Equals(object obj)
        {
            return obj is Letter story && Equals(story);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Letter left, Letter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Letter left, Letter right)
        {
            return !(left == right);
        }
        public static Letter Get(string id)
        {
            return GameData.Stories.Letters.Get(id);
        }
    }

    public enum StoryType
    {
        normalStory,
        dailyConversation,
        selectionBase,
    }
}