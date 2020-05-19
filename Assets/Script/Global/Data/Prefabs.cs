using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{
    [CreateAssetMenu(fileName = "EntityPrefabs", menuName = "Game Data/Entity Prefab", order = 1)]
    public class Prefabs : ScriptableObject
    {
        [Header("Army")]
        [SerializeField] protected GameObject defaultArmy;
        [Header("Building")]
        [SerializeField] protected GameObject defaultBuilding;
        [Header("Cards")]
        [SerializeField] protected GameObject centralDeckCardPrefab;
        [SerializeField] protected GameObject eventCardPrefab;
        [SerializeField] protected GameObject armyCardPrefab;
        [Header("UI")]
        [SerializeField] protected GameObject armyDamageDisplayer;
        [SerializeField] protected GameObject floatPanelSmall;
        [SerializeField] protected GameObject label;


        public GameObject DefaultArmy => defaultArmy;
        public GameObject DefaultBuilding => defaultBuilding;

        public GameObject CentralDeckCard => centralDeckCardPrefab;
        public GameObject EventCard => eventCardPrefab;
        public GameObject ArmyCard => armyCardPrefab;
        public GameObject ArmyDamageDisplayer => armyDamageDisplayer;
        public GameObject FloatPanelSmall => floatPanelSmall;
        public GameObject Label => label;
    }
}