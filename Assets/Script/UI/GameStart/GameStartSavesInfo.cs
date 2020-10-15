using Canute.Shops;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public class GameStartSavesInfo : MonoBehaviour
    {
        public Image icon;
        public Text levelName;
        public Text lastTime;

        public Save playerData;

        public void Start()
        {
            levelName.text = playerData.NextLevelName();
            lastTime.text = (playerData.LastOperationTime).ToShortDateString() + "\t" + Currency.Type.fedgram.Lang() + ": " + playerData.federgram;
            //lastTime.text = ((DateTime)playerData.playerLastOperationTime).ToShortDateString();
        }

        public void Select()
        {
            bool s = PlayerFile.LoadData(playerData.uuid);
            if (s) SceneControl.GotoScene(MainScene.mainHall);
        }
    }
}