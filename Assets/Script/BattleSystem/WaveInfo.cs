using System;
using System.Collections.Generic;
using UnityEngine;


namespace Canute.BattleSystem
{
    [Serializable]
    //[CreateAssetMenu(fileName = "Wave n", menuName = "Level/Wave", order = 0)]
    public class WaveInfo : ICloneable
    {
        [SerializeField] private List<BattleArmySpawnAnchor> battleArmies;
        [SerializeField] private List<BattleBuildingSheet> battleBuildings;

        public List<BattleArmySpawnAnchor> BattleArmies { get => battleArmies; set => battleArmies = value; }
        public List<BattleBuildingSheet> BattleBuildings { get => battleBuildings; set => battleBuildings = value; }

        public object Clone()
        {
            WaveInfo waveInfoCopy = new WaveInfo
            {
                battleArmies = battleArmies.Clone(),
                battleBuildings = battleBuildings.Clone(),
            };
            return waveInfoCopy;
        }
    }
}
