using UnityEngine;

namespace Canute
{
    [CreateAssetMenu(fileName = "Character", menuName = "Prototype/Character")]
    public class CharacterContainer : ScriptableObject, INameable
    {
        public Character character;
        public string Name => character.Name;
    }

}