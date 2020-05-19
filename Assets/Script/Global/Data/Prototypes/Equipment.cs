using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public class Equipment : Prototype
    {
        [SerializeField] private List<PropertyBounes> bounes;
        [SerializeField] private EquipmentType type;

        public override GameObject Prefab => null;
        public List<PropertyBounes> Bouneses { get => bounes; set => bounes = value; }
        public EquipmentType Type { get => type; set => type = value; }

        [Flags]
        public enum EquipmentType
        {
            none,
            bionic,
            mechanic,
            any,
            other = 4,

        }

    }

}