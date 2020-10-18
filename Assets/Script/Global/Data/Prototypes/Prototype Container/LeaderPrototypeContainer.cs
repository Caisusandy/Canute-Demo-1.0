using System;
using UnityEngine;
using Canute.Module;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "Leader", menuName = "Prototype/Leader Prototype")]
    public class LeaderPrototypeContainer : PrototypeContainer<Leader>
    {


        [ContextMenu("Add To Prototype Factory")]
        public override void AddToPrototypeFactory()
        {
            base.AddToPrototypeFactory();
        }
    }

    [Serializable]
    public class LeaderPrototypes : DataList<LeaderPrototypeContainer>
    {

    }

}