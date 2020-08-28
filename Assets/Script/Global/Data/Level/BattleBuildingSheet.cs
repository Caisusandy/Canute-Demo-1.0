using UnityEngine;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "Battle Building", menuName = "Level/Level Data/Building")]
    public class BattleBuildingSheet : SpawnAnchor
    {
        public BattleBuilding battleBuilding;
        public override Vector2Int Coordinate { get => battleBuilding.Coordinate; set => battleBuilding.Coordinate = value; }

        public static implicit operator BattleBuilding(BattleBuildingSheet battlebuildingSheet)
        {
            if (!battlebuildingSheet)
            {
                return null;
            }
            return battlebuildingSheet.battleBuilding;
        }
        public override object Clone()
        {
            return Instantiate(this);
        }
    }
}
