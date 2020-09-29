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
        public Image terrainIcon;

        public override IStatusContainer StatusContainer => cellEntity.data;

        private void Update()
        {
            if (!cellEntity)
            {
                return;
            }
            Cell data = cellEntity.data;
            terrain.text = data.terrain.ToString();
            terrainIcon.sprite = cellEntity.GetComponent<SpriteRenderer>().sprite;
        }
    }
}
