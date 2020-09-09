using System;
using Canute.Module;
using UnityEngine;
namespace Canute.BattleSystem
{
    [Serializable]
    public class Building : Prototype
    {
        public enum Types
        {
            passive,
            aggresive
        }

        [SerializeField] private int health;
        [SerializeField] private int defence;
        [SerializeField] private Types type;
        [SerializeField] private BattleProperty property;

        public Building() { }
        public Building(string name, int health, int defence, Types type, BattleProperty property)
        {
            this.name = name;
            this.health = health;
            this.defence = defence;
            this.type = type;
            this.property = property;
        }

        public int Health => health;
        public int Defence => defence;
        public Types Type => type;
        public BattleProperty Property => property;
        public override GameObject Prefab => prefab ?? GameData.Prefabs.DefaultBuilding;
        public static Building Empty => new Building();
    }

    [Serializable]
    public class BuildingPrototypes : DataList<BuildingPrototypeContainer>
    {

    }
}
