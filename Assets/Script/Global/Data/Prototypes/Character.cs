using Canute.Module;
using Canute.StorySystem;
using System;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public class Character : INameable
    {
        [SerializeField] protected string name;
        [SerializeField] protected int wordLineCount;
        [SerializeField] protected Sprite icon;
        [SerializeField] protected Sprite portrait;

        public string DisplayingName => this.Lang("name");
        public Story SupplyTeamComeBackItem => new Story("", new WordLine() { SpeakerName = DisplayingName, CharacterPortrait = Portrait, Line = this.Lang("supplyTeamComeBackWithItem"), Position = StoryDisplayer.SpeakerStandPosition.middle }) { Type = StoryType.dailyConversation };
        public Story SupplyTeamComeBackLeader => new Story("", new WordLine() { SpeakerName = DisplayingName, CharacterPortrait = Portrait, Line = this.Lang("supplyTeamComeBackWithLeader"), Position = StoryDisplayer.SpeakerStandPosition.middle }) { Type = StoryType.dailyConversation };
        public Story SupplyTeamComeBackStory => new Story("", new WordLine() { SpeakerName = DisplayingName, CharacterPortrait = Portrait, Line = this.Lang("supplyTeamComeBackWithStory"), Position = StoryDisplayer.SpeakerStandPosition.middle }) { Type = StoryType.dailyConversation };
        public Story SupplyTeamComeBackLetter => new Story("", new WordLine() { SpeakerName = DisplayingName, CharacterPortrait = Portrait, Line = this.Lang("supplyTeamComeBackWithLetter"), Position = StoryDisplayer.SpeakerStandPosition.middle }) { Type = StoryType.dailyConversation };
        public string Name => name;
        public Sprite Icon => icon;
        public Sprite Portrait => portrait;



        public string GetRandomWordLine()
        {
            return ("Canute.Character." + name + ".WordLine." + UnityEngine.Random.Range(0, wordLineCount)).Lang();
        }



        public static implicit operator bool(Character character)
        {
            if (character is null)
            {
                return false;
            }
            else if (character.name is null)
            {
                return false;
            }
            else if (string.IsNullOrEmpty(character.name))
            {
                return false;
            }
            else if (character.name == "Empty")
            {
                return false;
            }
            return true;
        }
        public static Character Get(string name)
        {
            return GameData.Prototypes.GetCharacter(name);
        }
    }

    [Serializable]
    public class CharacterContainerList : DataList<CharacterContainer>
    {

    }

}