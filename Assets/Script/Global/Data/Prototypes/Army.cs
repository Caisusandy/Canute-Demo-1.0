using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{
    /// <summary> 军队原型 </summary>
    [Serializable]
    public class Army : Prototype
    {
        public enum Types
        {
            none = -1,
            infantry = 0,
            mage,
            warMachine,
            shielder,

            rifleman,
            cavalry,
            sapper,

            airship,
            aircraftFighter,
            dragon,

            scout,
        }

        [SerializeField] private double health;
        [SerializeField] private double damage;
        [SerializeField] private Types type;
        [SerializeField] private Career career = Career.none;

        [SerializeField] private List<BattleProperty> properties = new List<BattleProperty>();
        public static Army Empty => new Army();

        public Army() { properties = new List<BattleProperty>() { new BattleProperty(), new BattleProperty(), new BattleProperty() }; }


        public Army(string name, double health, double damage, Types type, Career career, List<BattleProperty> properties)
        {
            this.name = name;
            this.health = health;
            this.damage = damage;
            this.type = type;
            this.career = career;
            this.properties = properties;
        }
        public Army(string name, double health, double damage, Types type, Career career, GameObject prefab, List<BattleProperty> properties)
        {
            this.name = name;
            this.health = health;
            this.damage = damage;
            this.type = type;
            this.prefab = prefab;
            this.career = career;
            this.properties = properties;
        }

        public double Health => health;
        public double Damage => damage;
        public Types Type => type;
        public Career Career => career;
        public List<BattleProperty> Properties => properties;
        public override GameObject Prefab => prefab ?? GameData.Prefabs.DefaultArmy;
    }
}

namespace Canute.BattleSystem
{
    public static class AttackTypes
    {
        public static bool IsTypeOf(this BattleProperty.AttackType attackTypes, BattleProperty.AttackType param)
        {
            return (attackTypes & param) == param;
        }
    }

    public static class Positions
    {
        public static bool IsTypeOf(this BattleProperty.Position attackTypes, BattleProperty.Position param)
        {
            return (attackTypes & param) == param;
        }
    }
}

