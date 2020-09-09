using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.UI.Legion
{
    public class LSLegionDisplay : MonoBehaviour, IMonoinstanceMonoBehaviour
    {
        public static LSLegionDisplay instance;

        public LSResonance resonancePanel;
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
            ArmyItem armyItem = Legion.Armies[id];

            LSSingleArmyPanel.instance.Display(armyItem);
            LSSingleArmyPanel.instance.selectingArmyCard = armyCards[id].GetComponent<ArmyCardUI>();
            if (!armyItem)
            {
                LSSingleArmyPanel.instance.ChangeArmy();
            }
        }

        public void ReloadLegion()
        {
            LoadLegion(legionId);
        }

        public void LoadLegion(int id)
        {
            var legion = Game.PlayerData.Legions[id];
            legionId = id;
            for (int j = 0; j < legion.Armies.Count; j++)
            {
                ArmyItem item = legion.Armies[j];
                LSLegionSmallIcon.UpdateIcon(j, item);
            }

            int i = 0;
            for (; i < legion.RealArmyCount; i++)
            {
                GameObject item = armyCards[i];
                ArmyItem armyItem = legion.Armies[i];
                item.SetActive(true);
                item.GetComponent<ArmyCardUI>().Exist()?.Display(armyItem);
                LSLegionSmallIcon.UpdateIcon(i, armyItem);
            }
            if (i != 6)
            {
                GameObject item = armyCards[i];
                item.SetActive(true);
                item.GetComponent<ArmyCardUI>().Exist()?.Display(ArmyItem.Empty);
                i++;
            }
            for (; i < 6; i++)
            {
                GameObject item = armyCards[i];
                ArmyItem armyItem = legion.Armies[i];
                item.SetActive(false);
            }


            resonancePanel.LoadResonance();
            SelectArmy(0);
        }
    }
}