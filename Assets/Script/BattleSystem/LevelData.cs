using UnityEngine;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "LevelData", menuName = "Game Data/LevelData", order = 5)]
    public class LevelData : ScriptableObject, INameable
    {
        [SerializeField] protected string levelName;
        [SerializeField] protected Battle battle;

        public string Name => levelName;

        public Battle GetBattle()
        {
            Battle battleCopy = battle.Clone() as Battle;
            return battleCopy;
        }

    }

}
