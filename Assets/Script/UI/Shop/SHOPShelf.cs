using UnityEngine;
using Canute.Shops;
using System;
using UnityEngine.UI;

namespace Canute.UI.Shop
{
    public class SHOPShelf : MonoBehaviour
    {
        public static SHOPShelf instance;
        public Transform itemAnchor;
        public Text refreshTime;

        public GameObject ArmyShelfItemPrefab;
        public GameObject EquipmentShelfItemPrefab;

        private TimeSpan nextRefreshTime;
        private void OnDestroy()
        {
            instance = null;
        }
        private void Awake()
        {
            instance = this;
        }
        public void Start()
        {
            if (Game.PlayerData.ShopInfo.NextRefreshTime == null)
            {

            }
            if (Game.PlayerData.ShopInfo.NextRefreshTime < DateTime.Now)
            {
                Game.PlayerData.ShopInfo.Refresh();
            }
            CreateShelfItem(Game.PlayerData.ShopInfo);
        }

        public void Update()
        {
            if (nextRefreshTime < Game.PlayerData.ShopInfo.NextRefreshTime - DateTime.Now)
            {
                Refresh();
            }
            nextRefreshTime = Game.PlayerData.ShopInfo.NextRefreshTime - DateTime.Now;
            refreshTime.text = (nextRefreshTime).ToString(@"hh\:mm\:ss");
        }

        public void Refresh()
        {
            Clear();
            CreateShelfItem(Game.PlayerData.ShopInfo);
        }

        private void Clear()
        {
            foreach (Transform item in itemAnchor)
            {
                Destroy(item.gameObject);
            }
        }

        public void CreateShelfItem(ShopInfo shopInfo)
        {
            for (int i = 0; i < shopInfo.OnShopArmies.Count; i++)
            {
                SHOPOnShelfItem sHOPOnShelfItem = Instantiate(ArmyShelfItemPrefab, itemAnchor).GetComponent<SHOPOnShelfItem>();
                sHOPOnShelfItem.Display(shopInfo.OnShopArmies[i]);
            }

            for (int i = 0; i < shopInfo.OnShopEquipments.Count; i++)
            {
                SHOPOnShelfItem sHOPOnShelfItem = Instantiate(EquipmentShelfItemPrefab, itemAnchor).GetComponent<SHOPOnShelfItem>();
                sHOPOnShelfItem.Display(shopInfo.OnShopEquipments[i]);
            }
        }
    }
}