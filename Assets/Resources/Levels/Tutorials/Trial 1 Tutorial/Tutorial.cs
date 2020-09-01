﻿using Canute.StorySystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class Tutorial : MonoBehaviour
    {
        public List<CellEntity> winningCell;

        // Start is called before the first frame update
        void Start()
        {
            foreach (var item in ArmyCardEntity.armyCards)
            {
                Destroy(item);
            }
            Game.CurrentBattle.Player.StatList.Clear(true);
            Game.CurrentBattle.Start();
            StartCoroutine("OpenTutorial");


        }

        IEnumerator OpenTutorial()
        {
            while (StoryDisplayer.instance != null)
            {
                yield return new WaitForFixedUpdate();
            }
            StoryDisplayer.Load("Tutorial");

        }

        // Update is called once per frame
        void Update()
        {
            foreach (var item in winningCell)
            {
                if (item.HasArmyStandOn)
                {
                    if (item.HasArmyStandOn.Owner == Game.CurrentBattle.Player)
                    {
                        Game.CurrentBattle.Win();
                    }
                }
            }
        }
    }
}