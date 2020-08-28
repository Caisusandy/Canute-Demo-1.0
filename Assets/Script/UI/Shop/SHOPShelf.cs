using UnityEngine;
using Canute.Shops;
using System;
using UnityEngine.UI;

namespace Canute.UI.Shop
{
    public class SHOPShelf : MonoBehaviour
    {
        public Transform itemAnchor;
        public Text refreshTime;

        public GameObject ArmyShelfItemPrefab;
        public GameObject EquipmentShelfItemPrefab;

        public void Start()
        {
            if (Game.PlayerData.ShopInfo.NextRefreshTime < DateTime.Now)
            {
                GameData.Shop.Refresh(Game.PlayerData.ShopInfo);
            }
            CreateShelfItem(Game.PlayerData.ShopInfo);
        }
        public void Update()
        {
            refreshTime.text = (Game.PlayerData.ShopInfo.NextRefreshTime - DateTime.Now).ToString(@"hh\:mm\:ss");
        }


        public void CreateShelfItem(ShopInfo shopInfo)
        {
            for (int i = 0; i < 4; i++)
            {
                SHOPOnShelfItem sHOPOnShelfItem = Instantiate(ArmyShelfItemPrefab, itemAnchor).GetComponent<SHOPOnShelfItem>();
                sHOPOnShelfItem.Display(shopInfo.OnShopArmies[i]);
            }

            //for (int i = 0; i < 4; i++)
            //{
            //    SHOPOnShelfItem sHOPOnShelfItem = Instantiate(EquipmentShelfItemPrefab, itemAnchor).GetComponent<SHOPOnShelfItem>();
            //    sHOPOnShelfItem.Display(shopInfo.OnShopEquipments[i]);
            //}
        }
    }
}