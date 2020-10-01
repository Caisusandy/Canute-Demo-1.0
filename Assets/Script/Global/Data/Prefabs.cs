using Canute.BattleSystem;
using Canute.Module;
using System;
using UnityEngine;

namespace Canute
{
    [CreateAssetMenu(fileName = "EntityPrefabs", menuName = "Game Data/Entity Prefab", order = 1)]
    public class Prefabs : ScriptableObject
    {
        [SerializeField] protected GameObject gameBackgroundMusicManager;
        [Header("Army")]
        [SerializeField] protected GameObject defaultArmy;
        [Header("Building")]
        [SerializeField] protected GameObject defaultBuilding;
        [Header("Cards")]
        [SerializeField] protected GameObject centralDeckCardPrefab;
        [SerializeField] protected GameObject eventCardPrefab;
        [SerializeField] protected GameObject armyCardPrefab;
        [SerializeField] protected GameObject dragonEventCardPrefab;
        [SerializeField] protected GameObject buildingEventCardPrefab;
        [Header("Other")]
        [SerializeField] protected PrefabPairList prefabPairs;

        public GameObject DefaultArmy => defaultArmy;
        public GameObject DefaultBuilding => defaultBuilding;

        public GameObject CentralDeckCard => centralDeckCardPrefab;
        public GameObject NormalEventCard => eventCardPrefab;
        public GameObject ArmyCard => armyCardPrefab;
        public GameObject DragonEventCard => dragonEventCardPrefab;
        public GameObject BuildingEventCard => buildingEventCardPrefab;

        public GameObject GameBackgroundMusicManager => gameBackgroundMusicManager;

        public GameObject Get(string index)
        {
            return prefabPairs.Get(index)?.Value;
        }

    }

    [Serializable]
    public class PrefabPair : ArgType<GameObject>
    {
        public PrefabPair(string key, GameObject value) : base(key, value)
        {
        }
    }

    [Serializable]
    public class PrefabPairList : DataList<PrefabPair>
    {

    }
}