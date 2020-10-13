using System;
using System.Linq;
using UnityEngine;
using Canute.Module;
using System.Collections.Generic;

namespace Canute.LevelTree
{
    [CreateAssetMenu(fileName = "Chapter Tree", menuName = "Game Data/Chapter Tree", order = 6)]
    public class Chapters : ScriptableObject
    {
        #region Chapters
        [SerializeField] protected Level tutorial;
        [SerializeField] protected ChapterTree chapterTree;
        [SerializeField] protected Chapter Extra = new Chapter();

        public ChapterTree ChapterTree { get => chapterTree; set => chapterTree = value; }

        public Level GetLevel(string name)
        {
            if (name == "Empty") { return null; }
            if (name == "Tutorial") { return tutorial; }

            foreach (Level item in ChapterTree.Levels)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            foreach (Level item in Extra)
            {
                if (item.Name == name)
                {
                    return item;
                }
            }
            return null;
        }

        public Level GetNext()
        {
            foreach (Level item in ChapterTree.CurrentStory)
            {
                if (!item.IsPassed)
                {
                    return item;
                }
            }
            return null;
        }

        public Chapter GetChapter(ChapterTree.Chapters chapter)
        {
            switch (chapter)
            {
                case ChapterTree.Chapters.None: return null;
                case ChapterTree.Chapters.CH1: return chapterTree.Chapter1;
                case ChapterTree.Chapters.CH2: return chapterTree.Chapter2;
                case ChapterTree.Chapters.CH3: return chapterTree.Chapter3;
                case ChapterTree.Chapters.CH4: return chapterTree.Chapter4;
                case ChapterTree.Chapters.CH5: return chapterTree.Chapter5;
                case ChapterTree.Chapters.CH6: return chapterTree.Chapter6;
                case ChapterTree.Chapters.CH7: return chapterTree.Chapter7;
                case ChapterTree.Chapters.Extra: return null;
                default: return null;
            }
        }

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
            CH7,
            Extra,
        }

        public Chapter Chapter1;
        public Chapter Chapter2;
        public Chapter Chapter3;
        public Chapter Chapter4;
        public Chapter Chapter5;
        public Chapter Chapter6;
        public Chapter Chapter7;
        public Chapter Extra;

        public ChapterTree()
        {
            Chapter1 = new Chapter();
            Chapter2 = new Chapter();
            Chapter3 = new Chapter();
            Chapter4 = new Chapter();
            Chapter5 = new Chapter();
            Chapter6 = new Chapter();
            Chapter7 = new Chapter();
            Extra = new Chapter();
        }

        public IEnumerable<Level> Levels => Chapter1.Union(Chapter2).Union(Chapter3).Union(Chapter4).Union(Chapter5).Union(Chapter6).Union(Chapter7).Union(Extra);

        public IEnumerable<Level> CurrentStory => Chapter7;

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
    [Serializable]
    public class LetterTree : DataList<LetterContainer>
    {

    }
}