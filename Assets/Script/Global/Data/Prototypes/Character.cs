using Canute.Module;
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
        public string RandomWordLine => ("Canute.Character." + name + ".WordLine." + UnityEngine.Random.Range(0, wordLineCount)).Lang();
        public string Name => name;
        public Sprite Icon => icon;
        public Sprite Portrait => portrait;

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