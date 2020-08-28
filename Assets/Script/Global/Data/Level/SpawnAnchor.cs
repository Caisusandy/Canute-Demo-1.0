using System;
using UnityEngine;

namespace Canute.BattleSystem
{
    public abstract class SpawnAnchor : ScriptableObject, ICloneable
    {
        public enum SpawnType
        {
            [InspectorName("exact Position")]
            //Exact Position that assigned
            coordinated,
            [InspectorName("random in radius")]
            //[InspectorName("a random Position that around the assigned position in the radius")]
            range,
            [InspectorName("total random")]
            //[InspectorName("a totally random position")]
            totalRandom
        }

        [Header("Prototype")]
        public PrototypeContainer Prototype;

        [Header("Spawn")]
        public SpawnType spawnType;
        public int radius;
        public int generateCount;


        public abstract Vector2Int Coordinate { get; set; }
        public abstract object Clone();
    }
}
