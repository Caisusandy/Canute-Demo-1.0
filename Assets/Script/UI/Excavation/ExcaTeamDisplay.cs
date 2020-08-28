using Canute.BattleSystem;
using Canute.ExplorationSystem;
using Canute.Shops;
using Canute.UI.LevelStart;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.Exploration
{
    public class ExcaTeamDisplay : MonoBehaviour
    {
        public static ExcaTeamDisplay instance;
        public List<LeaderIcon> leaderIcons;
        public ArmyIcon armyIcon;
        public Text TotalCost;

        public Button goOutButton;


        public Text RemainingTimeDisplayer;

        public ExplorationTeam Team { get => Game.PlayerData.ExplorationTeam; set => Game.PlayerData.ExplorationTeam = value; }

        #region Fund

        public InputField fedgramField;
        public InputField manpowerField;
        public InputField mantleAlloyField;

        public int Discount
        {
            get
            {
                switch (Team.RealLeader.Count)
                {
                    case 1:
                        return 0;
                    case 2:
                        return 5;
                    case 3:
                        return 15;
                    case 4:
                        return 25;
                    default:
                        return 0;
                }
            }
        }

        public int Fedgram { get { int a; return int.TryParse(fedgramField.text, out a) ? a : 0; } }
        public int Manpower { get { int a; return int.TryParse(manpowerField.text, out a) ? a : 0; } }
        public int MantleAlloy { get { int a; return int.TryParse(mantleAlloyField.text, out a) ? a : 0; } }
        public int TotalFedgram => (Fedgram * Team.RealLeader.Count).Bonus(-Discount);
        public int TotalManpower => (Manpower * Team.RealLeader.Count).Bonus(-Discount);
        public int TotalMantleAlloy => (MantleAlloy * Team.RealLeader.Count).Bonus(-Discount);

        #endregion
        private void Awake()
        {
            instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            ReloadTeamIcon();
        }
        // Update is called once per frame
        void Update()
        {
            TotalCost.text = "Total: Fedgram:" + TotalFedgram + ", " + "Manpower:" + TotalManpower + "\n" + "Mantle Alloy:" + TotalMantleAlloy;
            RemainingTimeDisplayer.text = Team.ToBackTime.ToString("g");
            goOutButton.interactable = Team.ToBackTime == TimeSpan.Zero;
        }

        public void ReloadTeamIcon()
        {
            armyIcon.Display(Team.Army);
            for (int i = 0; i < 4; i++)
            {
                LeaderIcon item = leaderIcons[i];
                item.Display(Team.Leaders[i]);
            }
        }

        public void ChangeArmy()
        {
            ArmyListUI.SelectEvent += Change;
            ArmyListUI.OpenArmyList();

            void Change(ArmyItem item)
            {
                Team.ArmyUUID = item.UUID;
                ReloadTeamIcon();
                PlayerFile.SaveCurrentData();
                ArmyListUI.CloseArmyList();
                ArmyListUI.SelectEvent -= Change;

            }
        }

        public void ChangeLeader(int id)
        {
            LeaderScroll.notShowingLeader = Team.Leaders.Except(new List<LeaderItem> { Team.Leaders[id] });
            LeaderScroll.SelectEvent += Change;
            LeaderScroll.OpenLeaderScroll();
            //            SceneControl.AddScene(MainScene.playerLeaderList);

            void Change(LeaderItem item)
            {
                Team.SetLeader(id, item);
                ReloadTeamIcon();
                PlayerFile.SaveCurrentData();
                LeaderScroll.CloseLeaderScroll();
                LeaderScroll.SelectEvent -= Change;

                // SceneControl.RemoveScene(MainScene.playerLeaderList);
            }
        }

        public void GoOut()
        {
            bool a = Game.PlayerData.Spent(new Currency(Currency.Type.fedgram, TotalFedgram), new Currency(Currency.Type.manpower, TotalManpower), new Currency(Currency.Type.mantleAlloy, TotalMantleAlloy));
            if (!a)
            {
                Debug.Log("Can't Afford");
                return;
            }
            ExplorationSystem.Exploration.GoOut(Team, new Fund(TotalFedgram, TotalManpower, TotalMantleAlloy));
            PrizeDisplay.instance.Start();
        }
    }
}