using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.Shops
{
    [Serializable]
    public class ShopInfo
    {
        [Header("Last Refresh Time")]
        [SerializeField] private WorldTime nextRefreshTime;
        [Header("On Shop Item")]
        [SerializeField] private PriceList onShopArmies = new PriceList();
        [SerializeField] private PriceList onShopEquipments = new PriceList();

        public PriceList OnShopArmies { get => onShopArmies; set => onShopArmies = value; }
        public PriceList OnShopEquipments { get => onShopEquipments; set => onShopEquipments = value; }
        public WorldTime NextRefreshTime { get => nextRefreshTime; set => nextRefreshTime = value; }



        /// <summary>
        /// Clear old items in the shop
        /// </summary>
        public void Clear()
        {
            onShopArmies.Clear();
            onShopEquipments.Clear();
        }
    }

    [Serializable]
    [CreateAssetMenu(fileName = "Shop", menuName = "Game Data/Shop", order = 4)]
    public class Shop : ScriptableObject
    {
        [Header("Weight List")]
        [SerializeField] private List<WeightItem> army;
        [SerializeField] private List<WeightItem> leader;
        [SerializeField] private List<WeightItem> equipment;

        private readonly TimeSpan RefleshDurationHour = new TimeSpan(6, 0, 0);
        private const int armyOnShopCount = 4;
        private const int leaderOnShopCount = 4;
        private const int equipmentOnShopCount = 4;

        [Header("Possible Item")]
        //[SerializeField] protected PriceList onShopArmies;
        //[SerializeField] protected PriceList onShopLeaders;
        //[SerializeField] protected PriceList onShopEquipments;
        [SerializeField] protected PriceList ArmyPriceList;
        [SerializeField] protected PriceList LeaderPriceList;
        [SerializeField] protected PriceList EquipementPriceList;


        public List<WeightItem> Army { get => army; set => army = value; }
        public List<WeightItem> Leader { get => leader; set => leader = value; }
        public List<WeightItem> Equipment { get => equipment; set => equipment = value; }

        public ArmyItem BuyArmy(PricePair armyWithPrice)
        {
            ArmyItem armyItem = new ArmyItem(GameData.Prototypes.GetArmyPrototype(armyWithPrice.Name));
            Game.PlayerData.AddArmyItem(armyItem);
            return armyItem;
        }

        /// <summary>
        /// Shop Reflesh
        /// </summary>
        public void Refresh(ShopInfo shopInfo)
        {
            if (shopInfo.NextRefreshTime > DateTime.Now)
            {
                return;
            }

            shopInfo.NextRefreshTime = ((DateTime.Now)).Add(RefleshDurationHour);
            shopInfo.Clear();
            GetNewItems(shopInfo);
            PlayerFile.SaveCurrentData();
        }

        /// <summary>
        /// Get new items to the shop
        /// </summary>
        private void GetNewItems(ShopInfo shopInfo)
        {
            for (int i = 0; i < armyOnShopCount; i++)
            {
                shopInfo.OnShopArmies.Add(ArmyPriceList.RandomOut());
            }
            //for (int i = 0; i < leaderOnShopCount; i++)
            //{
            //    shopInfo.onShopLeaders.Add(LeaderPriceList.RandomOut());
            //}
            //for (int i = 0; i < equipmentOnShopCount; i++)
            //{
            //    shopInfo.OnShopEquipments.Add(EquipementPriceList.RandomOut());
            //}
        }

    }
}
