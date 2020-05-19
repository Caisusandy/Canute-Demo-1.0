using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.StorySystem
{
    [Serializable]
    public struct WordLine : INameable, IEquatable<WordLine>
    {
        [SerializeField] private string id;
        [SerializeField] private string speakerName;
        [TextArea, SerializeField] private string line;
        [SerializeField] private Sprite characterPortrait;
        [SerializeField] private StoryDisplayer.SpeakerStandPosition position;
        [SerializeField] private List<Selection> selections;

        public string Name { get => id; set => id = value; }
        public string SpeakerName { get => speakerName; set => speakerName = value; }
        public string Line { get => line; set => line = value; }
        public bool HasSelection => selections is null ? false : selections.Count != 0;
        public StoryDisplayer.SpeakerStandPosition Position { get => position; set => position = value; }
        public Sprite CharacterPortrait { get => characterPortrait; set => characterPortrait = value; }
        public List<Selection> Selections { get => selections; set => selections = value; }


        public static WordLine Empty => new WordLine() { Name = string.Empty, Position = StoryDisplayer.SpeakerStandPosition.none, Line = string.Empty };


        public WordLine(string id, string speakerName, StoryDisplayer.SpeakerStandPosition position, string line, Sprite characterSprite)
        {
            this.id = id;
            this.speakerName = speakerName;
            this.position = position;
            this.line = line;
            this.characterPortrait = characterSprite;
            selections = new List<Selection>();
        }

        public bool Equals(WordLine obj)
        {
            return Name == obj.Name && Position == obj.Position && Line == obj.Line;
        }

        public override bool Equals(object obj)
        {
            return obj is WordLine ? Equals((WordLine)obj) : false;
        }

        public static bool operator ==(WordLine left, WordLine right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(WordLine left, WordLine right)
        {
            return !(left == right);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }

    [Serializable]
    public struct Selection
    {
        public string toWordLineId;
        public string selectionInfo;
    }
}