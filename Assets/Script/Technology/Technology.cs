using Canute.BattleSystem;
using Canute.Module;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.Technologies
{
    [Serializable]
    [CreateAssetMenu(fileName = "Technology", menuName = "Game Data/Technology", order = 6)]
    public class Technology : ScriptableObject
    {
        public static Technology instance;

        #region TechnologyInfo

        [SerializeField] protected ArmyTree prototypeArmyTree;
        public ArmyTree PrototypeArmyTree => prototypeArmyTree;

        #endregion
    }

    [Serializable]
    public class ArmyTree : DataList<ArmyTreeInfo>
    {
        public ArmyTreeInfo this[Army index] => Get(index.Name);

        public static List<string> PlayerUnlocked => Game.PlayerData.Statistic.ArmiesUnlocked;

        public List<ArmyTreeInfo> PossibleNextArmies()
        {
            List<ArmyTreeInfo> possibleArmies = new List<ArmyTreeInfo>();
            foreach (ArmyTreeInfo item in list)
            {
                if (PlayerUnlocked.Contains(item.Name))
                {
                    continue;
                }
                List<ArmyTreeInfo> pairs = this.Finds(item.Requires.ToArray());
                if (HadPlayerUnlocked(pairs))
                {
                    possibleArmies.Add(item);
                }
            }
            return possibleArmies;
        }

        private static bool HadPlayerUnlocked(List<ArmyTreeInfo> pairs)
        {
            foreach (ArmyTreeInfo item in pairs)
            {
                if (PlayerUnlocked.Contains(item.Name))
                {
                    return true;
                }
            }
            return false;
        }

        public bool Contains(Army army)
        {
            return !(this[army] is null);
        }


    }

    [Serializable]
    public class ArmyTreeInfo : INameable
    {
        public string name;
        public List<string> requires;

        public string Name => name;
        public Army Army => GameData.Prototypes.GetArmyPrototype(name);
        public List<Army> Requires => GameData.Prototypes.GetArmyPrototypes(requires.ToArray());
    }
}
