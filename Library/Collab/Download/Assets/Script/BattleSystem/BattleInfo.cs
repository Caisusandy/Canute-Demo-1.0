using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public abstract class BattleInfo : ScriptableObject
    {
        public Battle.Type Type;

    }


    public class EndlessBattleInfo : BattleInfo
    {
        public List<BattleArmy> battleArmies;
    }
}