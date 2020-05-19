using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Canute.BattleSystem;
#if UNITY_EDITOR
#endif


namespace Canute.Testing
{

    public static class Fields
    {
        static public ArmyList tArmies = new ArmyList();
        static public LeaderList tLeaders = new LeaderList();
        static public EquipmentList tEquipments = new EquipmentList();
        static public EventCardList tEventCards = new EventCardList();
        static public BuildingList tBuildingList = new BuildingList();
    }

    public static class PrototypeLoader
    {

        public static string Path => Application.persistentDataPath + "/Testing/";

        public static void Export<T>(T prototype) where T : Prototype
        {
            string path = Path + typeof(T).Name + "/";
            string json = JsonUtility.ToJson(prototype);
            File.WriteAllText(path + prototype.Name + ".json", json);
        }

        public static T Import<T>(string path) where T : Prototype
        {
            string json = File.ReadAllText(path);
            T prototype = JsonUtility.FromJson<T>(json);
            return prototype;
        }

        public static List<T> LoadAllPrototype<T>() where T : Prototype
        {
            List<T> prototypes = new List<T>();
            string path = Path + typeof(T).Name;

            if (!File.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string[] paths = Directory.GetFiles(path);

            foreach (var item in paths)
            {
                prototypes.Add(Import<T>(item));
            }
            Debug.Log("Import " + prototypes.Count + " " + typeof(T).Name);
            return prototypes;
        }
    }
}