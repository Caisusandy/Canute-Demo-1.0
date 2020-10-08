using Canute.BattleSystem;
using System;
using Canute.StorySystem;

namespace Canute.LevelTree
{
    [Serializable]
    public class Level : INameable, IEquatable<Level>
    {
        public string backgroundStoryName;
        public string lastLevel;
        public LevelData data;
        public string nextLevel;
        public string endStoryName;

        public string Name => data.Exist()?.Name ?? "Empty";
        public string Title => this.Lang("title");
        public string Subtitle => this.Lang("subtitle");
        public LevelData Data => data;
        public Story BackgroundStory => Story.Get(backgroundStoryName);
        public Story EndStory => Story.Get(endStoryName);
        public Level LastLevel => GameData.Levels.GetLevel(lastLevel);
        public Level Next => GameData.Levels.GetLevel(nextLevel);
        public bool IsPassed { get { if (Name == "Empty") return true; return Game.PlayerData.PlayerChapterTreeStat.Get(Name)?.IsPassed == true; } }


        public bool OpenBGStory()
        {
            if (!Game.Configuration.ShowStory)
            {
                return false;
            }

            if (BackgroundStory)
            {
                StoryDisplayer.Load(BackgroundStory);
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
                StoryDisplayer.Load(EndStory);
                return true;
            }
            else return false;
        }

        public void Pass()
        {
            Game.PlayerData.PlayerChapterTreeStat.Add(this);
        }

        public void NotPass()
        {
            if (Game.PlayerData.PlayerChapterTreeStat.Contains(new LevelInfo(Name, true)))
                return;
            if (!Game.PlayerData.PlayerChapterTreeStat.Contains(new LevelInfo(Name, false)))
                Game.PlayerData.PlayerChapterTreeStat.Add(new LevelInfo(Name, false));
        }

        public bool Equals(Level level)
        {
            return data == level?.data;
        }

        public static implicit operator LevelInfo(Level level)
        {
            return new LevelInfo(level.Name, level.IsPassed);
        }

    }
}
