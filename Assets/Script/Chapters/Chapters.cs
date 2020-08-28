using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Canute.Module;
using Canute.StorySystem;

namespace Canute.LevelTree
{
    [CreateAssetMenu(fileName = "Chapter Tree", menuName = "Game Data/Chapter Tree", order = 6)]
    public class Chapters : ScriptableObject
    {
        #region Chapters
        [SerializeField] protected ChapterTree chapterTree;

        public ChapterTree ChapterTree { get => chapterTree; set => chapterTree = value; }

        public void OnValidate()
        {

        }

        #endregion
    }

    [Serializable]
    public class ChapterTree
    {
        public enum Chapters
        {
            None,
            CH1,
            CH2,
            CH3,
            CH4,
            CH5,
            CH6,
            Extra,
        }

        public Chapter Chapter1;
        public Chapter Chapter2;
        public Chapter Chapter3;
        public Chapter Chapter4;
        public Chapter Chapter5;
        public Chapter Chapter6;
        public Chapter Extra;

        public ChapterTree()
        {
            Chapter1 = new Chapter();
            Chapter2 = new Chapter();
            Chapter3 = new Chapter();
            Chapter4 = new Chapter();
            Chapter5 = new Chapter();
            Chapter6 = new Chapter();
            Extra = new Chapter();
        }

        public DataList<Level> Levels => Chapter1.Union(Chapter2).Union(Chapter3).Union(Chapter4).Union(Chapter5).Union(Chapter6).Union(Extra).ToDataList();

        public Level GetLevel(string name)
        {
            foreach (Level item in Levels)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }
    }

    [Serializable]
    public class Level : INameable, IEquatable<Level>
    {
        public LevelData data;
        public string backgroundStoryName;
        public string endStoryName;
        public string nextLevel;

        public string Name => Data.Name;
        public LevelData Data => data;
        public Story BackgroundStory => Story.Get(backgroundStoryName);
        public Story EndStory => Story.Get(endStoryName);
        public Level Next => GameData.Chapters.ChapterTree.GetLevel(nextLevel);


        public bool OpenBGStory()
        {
            if (!Game.Configuration.ShowStory)
            {
                return false;
            }

            if (BackgroundStory)
            {
                StorySystem.StoryDisplayer.Load(BackgroundStory);
                return true;
            }
            else return false;
        }

        public bool OpenEndStory()
        {
            if (!Game.Configuration.ShowStory)
            {
                return false;
            }

            if (EndStory)
            {
                StorySystem.StoryDisplayer.Load(EndStory);
                return true;
            }
            else return false;
        }

        public bool Equals(Level level)
        {
            return data == level?.data;
        }
    }


    [Serializable]
    public class Chapter : DataList<Level> { }
}

namespace Canute.StorySystem
{
    [Serializable]
    public class StoryTree : DataList<StoryContainer>
    {

    }
}