using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public class Equipment : Prototype
    {
        [SerializeField] private List<PropertyBonus> bounes;
        [SerializeField] private EquipmentType type;
        [SerializeField] private List<Army.Types> equipmentUsage;

        public List<PropertyBonus> Bonus { get => bounes; set => bounes = value; }
        public EquipmentType Type { get => type; set => type = value; }
        public List<Army.Types> EquipmentUsage { get => equipmentUsage; set => equipmentUsage = value; }

        [Flags]
        public enum EquipmentType
        {
            none,
            bionic,
            mechanic,
            any,
            other = 4,
        }

        [Flags]
        public enum UsageLimit
        {
            none,
            infantry = 1,
            mage = 2,
            warMachine = 4,
            shielder = 8,

            rifleman = 16,
            cavalry = 32,
            sapper = 64,

            airship = 128,
            aircraftFighter = 256,
            dragon = 512,

            air = 1024,
            land = 2048
        }

    }

}