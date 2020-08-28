using Canute.BattleSystem;
using System;
using UnityEngine;

namespace Canute.Shops
{
    [Serializable]
    [CreateAssetMenu(fileName = "Shop", menuName = "Game Data/Shop", order = 4)]
    public class Shop : ScriptableObject
    {
        [SerializeField]
        private WorldTime nextRefleshTime;

        private const int RefleshDurationHour = 6;
        private const int armyOnShopCount = 5;
        private const int leaderOnShopCount = 5;
        private const int equipmentOnShopCount = 5;

        [SerializeField] protected PriceList onShopArmies;
        [SerializeField] protected PriceList onShopLeaders;
        [SerializeField] protected PriceList onShopEquipments;
        [SerializeField] protected PriceList ArmyPriceList;
        [SerializeField] protected PriceList LeaderPriceList;
        [SerializeField] protected PriceList EquipementPriceList;

        public ArmyItem BuyArmy(PricePair armyWithPrice)
        {
            ArmyItem armyItem = new ArmyItem(GameData.Prototypes.GetArmyPrototype(armyWithPrice.Name));
            Game.PlayerData.AddArmyItem(armyItem);
            return armyItem;
        }

        /// <summary>
        /// Shop Reflesh
        /// </summary>
        public void Refresh()
        {
            if (nextRefleshTime < DateTime.UtcNow)
            {
                return;
            }
            nextRefleshTime = ((DateTime)nextRefleshTime).AddHours(RefleshDurationHour);

            Clear();
            GetNewItems();
        }

        /// <summary>
        /// Get new items to the shop
        /// </summary>
        private void GetNewItems()
        {
            for (int i = 0; i < armyOnShopCount; i++)
            {
                onShopArmies.Add(ArmyPriceList.RandomOut());
            }
            for (int i = 0; i < leaderOnShopCount; i++)
            {
                onShopLeaders.Add(LeaderPriceList.RandomOut());
            }
            for (int i = 0; i < equipmentOnShopCount; i++)
            {
                onShopEquipments.Add(EquipementPriceList.RandomOut());
            }
        }

        /// <summary>
        /// Clear old items in the shop
        /// </summary>
        private void Clear()
        {
            onShopArmies.Clear();
            onShopEquipments.Clear();
            onShopLeaders.Clear();
        }
    }
}
