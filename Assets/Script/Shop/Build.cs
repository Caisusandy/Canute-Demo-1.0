using Canute.BattleSystem;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.Shops
{
    [Obsolete]
    public static class Build
    {
        //public static Prize BuildPrize(uint aethium, uint mantleAlloy, uint manpower, uint federgram)
        //{
        //    float aethiumRarityBounesParam = UnityEngine.Random.value;
        //    float rarityRandomParam = UnityEngine.Random.value;


        //    float aethiumPrefer = aethium / 5f * 0.8f;
        //    float equipmentPrefer = mantleAlloy / 20f * 0.8f;
        //    float armyPrefer = manpower / 1000f * 0.8f;
        //    float leaderPrefer = federgram / 1000f * 0.8f;


        //    float rarityPos = (equipmentPrefer + armyPrefer + leaderPrefer + rarityRandomParam * 9) / 12;
        //    rarityPos += (1 - rarityPos) * (aethiumPrefer + 0.2f) * (0.2f + aethiumRarityBounesParam / 4);
        //    float prefer = Mathf.Max(equipmentPrefer, armyPrefer, leaderPrefer);


        //    IWeightable itemWeight = WeightItem.Get(UnityEngine.Random.value, new WeightItem((int)(10000 * armyPrefer), "Army"), new WeightItem((int)(10000 * leaderPrefer), "Leader"), new WeightItem((int)(10000 * equipmentPrefer), "Equipment"));



        //    Rarity rarity;
        //    if (rarityPos < 0.43)
        //        rarity = Rarity.common;
        //    else if (rarityPos < 0.65)
        //        rarity = Rarity.rare;
        //    else if (rarityPos < 0.8)
        //        rarity = Rarity.epic;
        //    else
        //        rarity = Rarity.legendary;
        //    switch (itemWeight.Name)
        //    {
        //        case "Army":
        //            if (armyPrefer != prefer && rarity != Rarity.common)
        //            {
        //                rarity -= 1;
        //            }
        //            return GetArmy(rarity);
        //        case "Equipment":
        //            if (equipmentPrefer != prefer && rarity != Rarity.common)
        //            {
        //                rarity -= 1;
        //            }
        //            return GetEquipement(rarity);
        //        case "Leader":
        //            if (leaderPrefer != prefer && rarity != Rarity.common)
        //            {
        //                rarity -= 1;
        //            }
        //            return GetLeader(rarity);
        //        default:
        //            break;
        //    }


        //    //Debug.Log("=========================================================");
        //    //Debug.Log("Build Thing: " + itemWeight.itemName + ", Rarity: " + rarity);
        //    //Debug.Log("Rarity Posiiton: " + rarityPos + ",    AE: " + aethiumPrefer + ";    MA: " + armyPrefer + ";    MP: " + equipmentPrefer + ";    FG: " + leaderPrefer + "；  Rarity Param:" + rarityRandomParam + "  Aethium Bounes Param: " + aethiumRarityBounesParam);

        //    return new Prize(Prize.Type.none, "", rarity);
        //}

        //public static Prize GetArmy(Rarity rarity)
        //{
        //    List<WeightItem> armies = new List<WeightItem>();
        //    foreach (var army in GameData.Shop.Army)
        //    {
        //        Army item = GameData.Prototypes.GetArmyPrototype(army.itemName);
        //        if (item.Rarity == rarity)
        //        {
        //            armies.Add(army);
        //        }
        //    }
        //    IWeightable itemWeight = WeightItem.Get(UnityEngine.Random.value, armies.OfType<IWeightable>().ToArray());
        //    return new Prize(Prize.Type.army, itemWeight.Name);
        //}

        //public static Prize GetEquipement(Rarity rarity)
        //{
        //    List<WeightItem> items = new List<WeightItem>();
        //    foreach (var i in GameData.Shop.Equipment)
        //    {
        //        Army item = GameData.Prototypes.GetArmyPrototype(i.itemName);
        //        if (item.Rarity == rarity)
        //        {
        //            items.Add(i);
        //        }
        //    }
        //    IWeightable itemWeight = WeightItem.Get(UnityEngine.Random.value, items.OfType<IWeightable>().ToArray());
        //    return new Prize(Prize.Type.equipement, itemWeight.Name);
        //}

        //public static Prize GetLeader(Rarity rarity)
        //{
        //    List<WeightItem> items = new List<WeightItem>();
        //    foreach (var i in GameData.Shop.Leader)
        //    {
        //        Army item = GameData.Prototypes.GetArmyPrototype(i.itemName);
        //        if (item.Rarity == rarity)
        //        {
        //            items.Add(i);
        //        }
        //    }
        //    IWeightable itemWeight = WeightItem.Get(UnityEngine.Random.value, items.OfType<IWeightable>().ToArray());
        //    return new Prize(Prize.Type.leader, itemWeight.Name);
        //}
    }

    //public struct Prize
    //{
    //    public enum Type
    //    {
    //        none,
    //        army,
    //        leader,
    //        equipement
    //    }

    //    public Type type;
    //    public string name;
    //    public Rarity rarity;

    //    public Prize(Type type, string name, Rarity rarity)
    //    {
    //        this.type = type;
    //        this.name = name;
    //        this.rarity = rarity;
    //    }

    //    public Prize(Type type, string name) : this()
    //    {
    //        this.type = type;
    //        this.name = name;
    //    }
    //}
    [Obsolete]
    public class ItemWeightList : ScriptableObject
    {
    }

}
