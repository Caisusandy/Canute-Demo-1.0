using System;
using Canute.Module;
using UnityEngine;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "Army", menuName = "Prototype/Army Prototype")]
    public class ArmyPrototypeContainer : PrototypeContainer<Army>
    {
        [ContextMenu("Add To Prototype Factory")]
        public override void AddToPrototypeFactory()
        {
            base.AddToPrototypeFactory();
        }
    }

    [Serializable]
    public class ArmyPrototypes : DataList<ArmyPrototypeContainer>
    {

    }
}

