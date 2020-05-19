using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "Leader", menuName = "Prototype/Leader Prototype")]
    public class LeaderPrototypeContainer : PrototypeContainer<Leader>
    {

    }

    [Serializable]
    public class LeaderPrototypes : DataList<LeaderPrototypeContainer>
    {

    }

}