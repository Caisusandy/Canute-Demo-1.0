using System.Collections.Generic;
using System.Linq;

namespace Canute
{
    public static class Hallway
    {
        public static List<Character> GetPlayersTeam()
        {
            return Game.PlayerData.Leaders.Where((l) => l.Prototype.HasAssociateCharacter).Select((l) => l.Prototype.Character).ToList();
        }

        public static List<Character> CameOut()
        {
            List<Character> characters = new List<Character>();
            characters.Add(Character.Get("Finn Herman"));

            for (int i = 0; i < 3; i++)
            {
                var possible = GetPlayersTeam().Except(characters).ToArray();
                if (possible.Length == 0) break;
                characters.Add(possible[UnityEngine.Random.Range(0, possible.Length)]);
            }

            return characters;
        }
    }
}
