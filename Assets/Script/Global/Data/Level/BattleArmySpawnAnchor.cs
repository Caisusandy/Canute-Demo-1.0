using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.BattleSystem
{


    [CreateAssetMenu(fileName = "Battle Army", menuName = "Level/Army Spawner")]
    public class BattleArmySpawnAnchor : SpawnAnchor
    {
        [Header("Data")]
        public BattleArmy battleArmy;

        public override Vector2Int Coordinate { get => battleArmy.Coordinate; set => battleArmy.Coordinate = value; }

        public static implicit operator BattleArmy(BattleArmySpawnAnchor battleArmySheet)
        {
            if (!battleArmySheet)
            {
                return null;
            }
            return battleArmySheet.battleArmy;
        }

        public override object Clone()
        {
            return Instantiate(this);
        }
    }

}
