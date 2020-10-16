using System;
using UnityEngine;
using Canute.Module;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "Equipment", menuName = "Prototype/Equipment Prototype")]
    public class EquipmentPrototypeContainer : PrototypeContainer<Equipment>
    {
        [ContextMenu("Add To Prototype Factory")]
        public override void AddToPrototypeFactory()
        {
            base.AddToPrototypeFactory();
        }
    }
    [Serializable]
    public class EquipmentPrototypes : DataList<EquipmentPrototypeContainer>
    {

    }
}