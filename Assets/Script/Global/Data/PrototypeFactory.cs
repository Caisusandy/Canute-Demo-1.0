using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "Game Data", menuName = "Game Data/Prototype Factory", order = 3)]
    public class PrototypeFactory : ScriptableObject
    {
        [SerializeField] private ArmyPrototypes armyPrototypes = new ArmyPrototypes();
        [SerializeField] private LeaderPrototypes leaderPrototypes = new LeaderPrototypes();
        [SerializeField] private EquipmentPrototypes equipmentPrototypes = new EquipmentPrototypes();
        [SerializeField] private EventCardPrototypes eventCardPrototypes = new EventCardPrototypes();
        [SerializeField] private BuildingPrototypes buildingPrototypes = new BuildingPrototypes();

        [Header("Default")]
        [SerializeField] private ArmyPrototypeContainer defaultArmy;
        [SerializeField] private BuildingPrototypeContainer defaultBuilding;
        [SerializeField] private EventCardPrototypeContainer defaultEventCard;
        [SerializeField] private EquipmentPrototypeContainer defaultEquipment;
        [SerializeField] private LeaderPrototypeContainer defaultLeader;


        public Prototype GetPrototype(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            if (!(GetBuildingPrototype(name) is null))
            {
                return GetBuildingPrototype(name);
            }
            else if (!(GetArmyPrototype(name) is null))
            {
                return GetArmyPrototype(name);
            }
            else if (!(GetLeaderPrototype(name) is null))
            {
                return GetLeaderPrototype(name);
            }
            else if (!(GetEquipmentPrototype(name) is null))
            {
                return GetEquipmentPrototype(name);
            }
            else if (!(GetEventCardPrototype(name) is null))
            {
                return GetEventCardPrototype(name);
            }
            return null;
        }


        public ArmyList TestingArmies => Testing.Fields.tArmies;
        public LeaderList TestingLeaders => Testing.Fields.tLeaders;
        public EquipmentList TestingEquipments => Testing.Fields.tEquipments;
        public EventCardList TestingEventCards => Testing.Fields.tEventCards;
        public BuildingList TestingBuildings => Testing.Fields.tBuildingList;

        public ArmyPrototypes Armies => armyPrototypes;
        public LeaderPrototypes Leaders => leaderPrototypes;
        public EquipmentPrototypes Equipments => equipmentPrototypes;
        public EventCardPrototypes EventCards => eventCardPrototypes;
        public BuildingPrototypes Buildings => buildingPrototypes;


        public Army GetArmyPrototype(string name)
        {
            if (GameData.BuildSetting.IsInDebugMode)
            {
                return Armies.Get(name) ?? TestingArmies.Get(name);
            }
            return Armies.Get(name) ?? defaultArmy;
        }

        public List<Army> GetArmyPrototypes(string[] names)
        {
            List<Army> armies = new List<Army>();
            foreach (string name in names)
            {
                armies.Add(GetArmyPrototype(name));
            }
            return armies;
        }

        public Building GetBuildingPrototype(string name)
        {
            if (GameData.BuildSetting.IsInDebugMode)
            {
                return Buildings.Get(name) ?? TestingBuildings.Get(name);
            }
            return Buildings.Get(name) ?? defaultBuilding;
        }

        public List<Building> GetBuildingPrototypes(string[] names)
        {
            List<Building> buildings = new List<Building>();
            foreach (string name in names)
            {
                buildings.Add(GetBuildingPrototype(name));
            }
            return buildings;
        }

        public Leader GetLeaderPrototype(string name)
        {
            if (GameData.BuildSetting.IsInDebugMode)
            {
                return Leaders.Get(name) ?? TestingLeaders.Get(name);
            }
            return Leaders.Get(name) ?? defaultLeader;
        }

        public List<Leader> GetLeaderPrototypes(string[] names)
        {
            List<Leader> leaders = new List<Leader>();
            foreach (string name in names)
            {
                leaders.Add(GetLeaderPrototype(name));
            }
            return leaders;
        }

        public Equipment GetEquipmentPrototype(string name)
        {
            if (GameData.BuildSetting.IsInDebugMode)
            {
                return Equipments.Get(name) ?? TestingEquipments.Get(name);
            }
            return Equipments.Get(name) ?? defaultEquipment;
        }

        public List<Equipment> GetEquipmentPPrototypes(string[] names)
        {
            List<Equipment> equipments = new List<Equipment>();
            foreach (string name in names)
            {
                equipments.Add(GetEquipmentPrototype(name));
            }
            return equipments;
        }

        public EventCard GetEventCardPrototype(string name)
        {
            if (GameData.BuildSetting.IsInDebugMode)
            {
                return EventCards.Get(name) ?? TestingEventCards.Get(name);
            }
            return EventCards.Get(name) ?? defaultEventCard;
        }

        public void Add<T1, T2>(T1 item) where T1 : PrototypeContainer<T2> where T2 : Prototype
        {
            if (item.Prototype is Army)
            {
                ArmyPrototypeContainer container = item as ArmyPrototypeContainer;
                if (Armies.Contains(container))
                {
                    return;
                }
                Armies.Add(container);
            }
            if (item.Prototype is Building)
            {
                BuildingPrototypeContainer container = item as BuildingPrototypeContainer;
                if (Buildings.Contains(container))
                {
                    return;
                }
                Buildings.Add(container);
            }
            if (item.Prototype is Leader)
            {
                LeaderPrototypeContainer container = item as LeaderPrototypeContainer;
                if (Leaders.Contains(container))
                {
                    return;
                }
                Leaders.Add(container);
            }
            if (item.Prototype is Equipment)
            {
                EquipmentPrototypeContainer container = item as EquipmentPrototypeContainer;
                if (Equipments.Contains(container))
                {
                    return;
                }
                Equipments.Add(container);
            }
            if (item.Prototype is EventCard)
            {
                EventCardPrototypeContainer container = item as EventCardPrototypeContainer;
                if (EventCards.Contains(container))
                {
                    return;
                }
                EventCards.Add(container);
            }
        }
    }
}