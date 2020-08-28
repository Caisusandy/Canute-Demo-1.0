using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.LevelScript
{
    public class C1L2 : MonoBehaviour
    {
        public Vector2Int logement1Pos => new Vector2Int(7, 12);
        public Vector2Int logement2Pos => new Vector2Int(16, 7);
        public Vector2Int logement3Pos => new Vector2Int(10, 22);
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (Game.CurrentBattle.CurrentStat != BattleSystem.Battle.Stat.normal)
            {
                return;
            }

            if (Game.CurrentBattle.Round.wave == 1)
            {
                if (Game.CurrentBattle.GetBuilding(logement1Pos)?.Owner == Game.CurrentBattle.Player)
                {
                    var area = Game.CurrentBattle.MapEntity.GetRectArea(new Vector2Int(11, 0), new Vector2Int(19, 15));
                    Game.CurrentBattle.MapEntity.OpenArea(area);
                    Game.CurrentBattle.WaveControl.NextWave();
                }
            }
            else if (Game.CurrentBattle.Round.wave == 2)
            {
                if (Game.CurrentBattle.GetBuilding(logement2Pos)?.Owner == Game.CurrentBattle.Player)
                {
                    var area = Game.CurrentBattle.MapEntity.GetRectArea(new Vector2Int(0, 16), new Vector2Int(19, 24));
                    Game.CurrentBattle.MapEntity.OpenArea(area);
                    Game.CurrentBattle.WaveControl.NextWave();
                }
            }
            else if (Game.CurrentBattle.Round.wave == 3)
            {
                if (Game.CurrentBattle.GetBuilding(logement3Pos).Owner == Game.CurrentBattle.Player)
                {
                    Game.CurrentBattle.Win();
                }
            }
        }
    }
}