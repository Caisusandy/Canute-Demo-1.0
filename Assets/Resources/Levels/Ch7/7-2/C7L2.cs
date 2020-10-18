using Canute.BattleSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.LevelScript
{
    public class C7L2 : MonoBehaviour
    {
        public List<CellEntity> destination;


        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            foreach (var item in destination)
            {
                if (item.HasArmyStandOn)
                {
                    if (item.HasArmyStandOn.Owner == Game.CurrentBattle.Player)
                    {
                        Game.CurrentBattle.Win();
                        DestroyImmediate(this);
                    }
                }
            }
        }
    }
}