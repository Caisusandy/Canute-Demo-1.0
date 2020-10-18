using System;
using System.IO;
using System.Collections.Generic;
using Canute.Module;
using UnityEngine;

namespace Canute.BattleSystem
{
    public static class TempPrototypes
    {
        public static ArmyPrototypes tempArmyPrototypes = new ArmyPrototypes();
        public static LeaderPrototypes tempLeaderPrototypes = new LeaderPrototypes();
        public static EquipmentPrototypes tempEquipmentPrototypes = new EquipmentPrototypes();
        public static EventCardPrototypes tempEventCardPrototypes = new EventCardPrototypes();
        public static BuildingPrototypes tempBuildingPrototypes = new BuildingPrototypes();
    }

    [CreateAssetMenu(fileName = "Game Data", menuName = "Game Data/Prototype Factory", order = 3)]
    public class PrototypeFactory : ScriptableObject
    {
        [Header("Prototypes")]
        [SerializeField] private ArmyPrototypes armyPrototypes = new ArmyPrototypes();
        [SerializeField] private LeaderPrototypes leaderPrototypes = new LeaderPrototypes();
        [SerializeField] private EquipmentPrototypes equipmentPrototypes = new EquipmentPrototypes();
        [SerializeField] private BuildingPrototypes buildingPrototypes = new BuildingPrototypes();
        [SerializeField] private CharacterContainerList characterList = new CharacterContainerList();
        [Header("Event Cardss")]
        [SerializeField] private EventCardPrototypes eventCardPrototypes = new EventCardPrototypes();
        [SerializeField] private EventCardPrototypes dragonEventCardPrototypes = new EventCardPrototypes();


        [Header("Default")]
        [SerializeField] private ArmyPrototypeContainer defaultArmy;
        [SerializeField] private BuildingPrototypeContainer defaultBuilding;
        [SerializeField] private EventCardPrototypeContainer defaultEventCard;
        [SerializeField] private EquipmentPrototypeContainer defaultEquipment;
        [SerializeField] private LeaderPrototypeContainer defaultLeader;
        [SerializeField] private CharacterContainer defaultCharacter;


        public void OnEnable()
        {
        }

        public Prototype GetPrototype(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            if (GetArmyPrototype(name))
            {
                return GetArmyPrototype(name);
            }
            else if ((GetBuildingPrototype(name)))
            {
                return GetBuildingPrototype(name);
            }
            else if ((GetLeaderPrototype(name)))
            {
                return GetLeaderPrototype(name);
            }
            else if ((GetEquipmentPrototype(name)))
            {
                return GetEquipmentPrototype(name);
            }
            else if ((GetEventCardPrototype(name)))
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
        public EventCardPrototypes DragonEventCards => dragonEventCardPrototypes;
        public BuildingPrototypes Buildings => buildingPrototypes;

        public ArmyPrototypes TempArmies => TempPrototypes.tempArmyPrototypes;
        public LeaderPrototypes TempLeaders => TempPrototypes.tempLeaderPrototypes;
        public EquipmentPrototypes TempEquipments => TempPrototypes.tempEquipmentPrototypes;
        public EventCardPrototypes TempEventCards => TempPrototypes.tempEventCardPrototypes;
        public BuildingPrototypes TempBuildings => TempPrototypes.tempBuildingPrototypes;

        public Army GetArmyPrototype(string name)
        {
            if (Game.Configuration.UseCustomDefaultPrototype)
            {
                return TestingArmies.Get(name) ?? defaultArmy;
            }
            return Armies.Get(name).Exist()?.Prototype ?? TempArmies.Get(name).Exist()?.Prototype ?? defaultArmy;
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
            if (Game.Configuration.UseCustomDefaultPrototype)
            {
                return TestingBuildings.Get(name) ?? defaultBuilding;
            }
            return Buildings.Get(name).Exist()?.Prototype ?? TempBuildings.Get(name).Exist()?.Prototype ?? defaultBuilding;
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
            if (Game.Configuration.UseCustomDefaultPrototype)
            {
                return TestingLeaders.Get(name) ?? defaultLeader;
            }
            return Leaders.Get(name).Exist()?.Prototype ?? TempLeaders.Get(name).Exist()?.Prototype ?? defaultLeader;
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
            if (Game.Configuration.UseCustomDefaultPrototype)
            {
                return TestingEquipments.Get(name) ?? defaultEquipment;
            }
            return Equipments.Get(name).Exist()?.Prototype ?? TempEquipments.Get(name).Exist()?.Prototype ?? defaultEquipment;
        }

        public List<Equipment> GetEquipmentPrototypes(string[] names)
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
            if (Game.Configuration.UseCustomDefaultPrototype)
            {
                return TestingEventCards.Get(name) ?? defaultEventCard;
            }
            return EventCards.Get(name).Exist()?.Prototype ?? DragonEventCards.Get(name).Exist()?.Prototype ?? TempEventCards.Get(name).Exist()?.Prototype ?? defaultEventCard;
        }

        public Character GetCharacter(string name)
        {
            return characterList.Get(name).Exist()?.character ?? defaultCharacter.character;
        }

        public void Add<T1>(T1 item, bool isMainPrototype = false) where T1 : PrototypeContainer
        {
            Prototype prototype = item.GetPrototype();
            if (prototype is Army)
            {
                ArmyPrototypeContainer container = item as ArmyPrototypeContainer;
                if (Armies.Contains(container) || TempArmies.Contains(container))
                    return;
                if (isMainPrototype)
                    Armies.Add(container);
                else
                    TempArmies.Add(container);
            }
            else if (prototype is Building)
            {
                BuildingPrototypeContainer container = item as BuildingPrototypeContainer;
                if (Buildings.Contains(container) || TempBuildings.Contains(container))
                    return;
                if (isMainPrototype)
                    Buildings.Add(container);
                else
                    TempBuildings.Add(container);
            }
            else if (prototype is Leader)
            {
                LeaderPrototypeContainer container = item as LeaderPrototypeContainer;
                if (Leaders.Contains(container) || TempLeaders.Contains(container))
                    return;
                if (isMainPrototype)
                    Leaders.Add(container);
                else
                    TempLeaders.Add(container);
            }
            else if (prototype is Equipment)
            {
                EquipmentPrototypeContainer container = item as EquipmentPrototypeContainer;
                if (Equipments.Contains(container))
                {
                    return;
                }
                if (Equipments.Contains(container) || TempEquipments.Contains(container))
                    return;
                if (isMainPrototype)
                    Equipments.Add(container);
                else
                    TempEquipments.Add(container);
            }
            else if (prototype is EventCard)
            {
                EventCardPrototypeContainer container = item as EventCardPrototypeContainer;
                if (EventCards.Contains(container))
                {
                    return;
                }
                if (EventCards.Contains(container) || TempEventCards.Contains(container))
                    return;
                if (isMainPrototype)
                    EventCards.Add(container);
                else
                    TempEventCards.Add(container);
            }
        }
    }
}