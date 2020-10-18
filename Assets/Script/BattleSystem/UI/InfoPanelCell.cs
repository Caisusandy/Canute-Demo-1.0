using Canute.Module;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class InfoPanelCell : InfoPanel
    {
        [Header("Displaying entity")]
        public CellEntity cellEntity;
        [Header("Displaying info")]
        public Text terrain;
        public Text temperature;
        public Text moisture;
        public Image terrainIcon;
        public GameObject cantAccess;

        public override IStatusContainer StatusContainer => cellEntity.data;

        private void Update()
        {
            if (!cellEntity)
            {
                return;
            }
            Cell data = cellEntity.data;
            terrain.text = data.terrain.ToString();
            temperature.text = "T: " + data.temperature.ToString() + "°C";
            moisture.text = "Humidity: " + data.humidity.ToString() + "%";
            terrainIcon.sprite = cellEntity.GetComponent<SpriteRenderer>().sprite;
            cantAccess.SetActive(!cellEntity.data.canStandOn);
        }
    }
}
