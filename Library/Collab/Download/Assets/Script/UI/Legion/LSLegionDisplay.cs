using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.UI.Legion
{
    public class LSLegionDisplay : MonoBehaviour, IMonoinstanceMonoBehaviour
    {
        public static LSLegionDisplay instance;
        public List<GameObject> armyCards;
        public List<LSLegionSmallIcon> legionSmallIcons;

        public int legionId;
        public int lastId { get; set; }

        public Canute.Legion Legion => Game.PlayerData.Legions[legionId];
        public LSLegionSmallIcon LSLegionSmallIcon => legionSmallIcons[legionId];

        public void Awake()
        {
            instance = this;
            LSLegionSmallIcon.SelectEvent = LoadLegion;
        }

        public void OnDestroy()
        {
            instance = null;
        }

        // Start is called before the first frame update
        void Start()
        {
            LoadLegion(0);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public void SelectArmy(int id)
        {
            LSSingleArmyPanel.instance.Display(Legion.Armies[id]);
            LSSingleArmyPanel.instance.selectingArmyCard = armyCards[id].GetComponent<ArmyCardUI>();
        }

        public void ReloadLegion()
        {
            LoadLegion(legionId);
        }

        public void LoadLegion(int id)
        {
            var legion = Game.PlayerData.Legions[id];
            legionId = id;

            for (int i = 0; i < armyCards.Count; i++)
            {
                GameObject item = armyCards[i];
                ArmyItem armyItem = i < legion.Armies.Count ? legion.Armies[i] : ArmyItem.Empty;
                item.GetComponent<ArmyCardUI>().Exist()?.Display(armyItem);
                LSLegionSmallIcon.UpdateIcon(i, armyItem);
            }

            SelectArmy(0);
        }
    }
}