using Canute.StorySystem;
using Canute.SupplyTeam;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.SupplyTeam
{
    public class STPrize : MonoBehaviour
    {
        [HideInInspector]
        public PrizeBox prizeBox;

        public Text remainingTime;
        public Image pic;
        public Button openPrizeButton;

        private void Awake()
        {
            prizeBox = null;
        }

        // Start is called before the first frame update
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (prizeBox == null)
            {
                Disable();
                return;
            }

            if (prizeBox.prize == null)
            {
                Disable();
                return;
            }

            if (prizeBox.ToReadyTime != TimeSpan.Zero)
            {
                openPrizeButton.interactable = false;
                remainingTime.text = prizeBox.ToReadyTime.ToString(@"hh\:mm\:ss");
            }
            else
            {
                remainingTime.text = TimeSpan.Zero.ToString(@"hh\:mm\:ss");
                openPrizeButton.interactable = true;
            }
        }

        public void Open()
        {
            //Debug.Log(STTeamDisplay.instance.Team);
            //Debug.Log(STTeamDisplay.instance.Team.Leaders[0]);
            //Debug.Log(STTeamDisplay.instance.Team.Leaders[0].Prototype);
            //Debug.Log(STTeamDisplay.instance.Team.Leaders[0].Prototype.Character);
            //Debug.Log(STTeamDisplay.instance.Team.Leaders[0].Prototype.Character.SupplyTeamComeBackItem);
            switch (prizeBox.prize.PrizeType)
            {
                case Item.Type.none:
                case Item.Type.commonItem:
                case Item.Type.currency:
                case Item.Type.equipment:
                    Debug.Log("Currency");
                    if (STTeamDisplay.instance.Team?.RealLeaders.Count > 0)
                        if (STTeamDisplay.instance.Team?.RealLeaders[0]?.Prototype?.Character)
                            StoryDisplayer.Load(STTeamDisplay.instance.Team.RealLeaders[0].Prototype.Character.SupplyTeamComeBackItem);
                    break;
                case Item.Type.army:
                case Item.Type.leader:
                    Debug.Log("leader");
                    if (STTeamDisplay.instance.Team?.RealLeaders.Count > 0)
                        if (STTeamDisplay.instance.Team?.RealLeaders[0]?.Prototype?.Character)
                            StoryDisplayer.Load(STTeamDisplay.instance.Team.RealLeaders[0].Prototype.Character.SupplyTeamComeBackLeader);
                    break;
                case Item.Type.letter:
                    Debug.Log("letter");
                    if (STTeamDisplay.instance.Team?.RealLeaders.Count > 0)
                        if (STTeamDisplay.instance.Team?.RealLeaders[0]?.Prototype?.Character)
                            StoryDisplayer.Load(STTeamDisplay.instance.Team.RealLeaders[0].Prototype.Character.SupplyTeamComeBackLetter);
                    break;
                case Item.Type.story:
                    Debug.Log("story");
                    if (STTeamDisplay.instance.Team?.RealLeaders.Count > 0)
                        if (STTeamDisplay.instance.Team?.RealLeaders[0]?.Prototype?.Character)
                            StoryDisplayer.Load(STTeamDisplay.instance.Team.RealLeaders[0].Prototype.Character.SupplyTeamComeBackStory);
                    break;
                default:
                    break;
            }

            STTeamDisplay.instance.Team.CurrentPrize.OpenNow();

            var info = "Canute.STPrizeDisplay.NewPrize".Lang().Replace("@prizeType", prizeBox.prize.PrizeType.ToString()).Replace("@name", prizeBox.prize.DisplayingName);
            if (prizeBox.prize.PrizeType == Item.Type.currency) info += ": " + prizeBox.prize.Count;
            InfoWindow.Create(info);

            STPrizeDisplay.instance.Start();
        }

        public void Display(PrizeBox prize)
        {
            prizeBox = prize;
        }

        public void Disable()
        {
            openPrizeButton.interactable = false;
            prizeBox = null;
            remainingTime.text = TimeSpan.Zero.ToString(@"hh\:mm\:ss");
        }
    }
}