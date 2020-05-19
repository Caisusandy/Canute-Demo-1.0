using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "Army", menuName = "Prototype/Army Prototype")]
    public class ArmyPrototypeContainer : PrototypeContainer<Army>
    {
        public void OnEnable()
        {

            //Debug.Log(prototype.Properties[2].Skill);
        }
    }

    [Serializable]
    public class ArmyPrototypes : DataList<ArmyPrototypeContainer>
    {

    }
}

