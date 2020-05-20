using System;
using System.Collections.Generic;
using UnityEngine;

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