using System;
using UnityEngine;
using Canute.Module;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "Equipment", menuName = "Prototype/Equipment Prototype")]
    public class EquipmentPrototypeContainer : PrototypeContainer<Equipment>
    {

    }
    [Serializable]
    public class EquipmentPrototypes : DataList<EquipmentPrototypeContainer>
    {

    }
}