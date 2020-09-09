using System;
using UnityEngine;
using Canute.Module;

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