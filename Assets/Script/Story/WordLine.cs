﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.StorySystem
{
    /// <summary>
    /// 
    /// </summary>
   //plan: special speaker name can do something
    [Serializable]
    public struct WordLine : INameable, IEquatable<WordLine>
    {
        [SerializeField] private string speakerName;
        [SerializeField] private string id;
        [TextArea(5, 20), SerializeField] private string line;
        [SerializeField] private Sprite characterPortrait;
        [SerializeField] private Sprite conversationBG;
        [SerializeField] private StoryDisplayer.SpeakerStandPosition position;
        [SerializeField] private List<Selection> selections;
        [SerializeField] private string nextLineID;

        public string Name { get => id; set => id = value; }
        [Obsolete]
        public string ID { get => id; set => id = value; }
        public string SpeakerName { get => speakerName; set => speakerName = value; }
        public string Line { get => line; set => line = value; }
        public string NextLineID { get => nextLineID; set => nextLineID = value; }
        public bool HasSelection => selections is null ? false : selections.Count != 0;
        public StoryDisplayer.SpeakerStandPosition Position { get => position; set => position = value; }
        public Sprite CharacterPortrait { get => characterPortrait; set => characterPortrait = value; }
        [Obsolete]
        public List<Selection> Selections { get => selections; set => selections = value; }
        public Sprite ConversationBG { get => conversationBG; set => conversationBG = value; }


        public static WordLine Empty => new WordLine() { Name = string.Empty, Position = StoryDisplayer.SpeakerStandPosition.none, Line = string.Empty };


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

        [Obsolete]
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
        [Obsolete]
        public string toWordLineId;
        [Obsolete]
        public string selectionInfo;
    }
}