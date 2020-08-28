using Canute.ExplorationSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.Exploration
{
    public class Prize : MonoBehaviour
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
            ExcaTeamDisplay.instance.Team.CurrentPrize.OpenNow();
            PrizeDisplay.instance.prizeInfo.text = prizeBox.prize.PrizeType + " " + prizeBox.prize.Name;
            if (prizeBox.prize.PrizeType == Item.Type.currency)
            {
                PrizeDisplay.instance.prizeInfo.text += ": " + prizeBox.prize.Count;
            }
            PrizeDisplay.instance.Start();
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