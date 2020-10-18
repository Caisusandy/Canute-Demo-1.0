using UnityEngine;

namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "Battle Building", menuName = "Level/Building Spawner")]
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

        [ContextMenu("Set as Campus")]
        public void SetAsCampus()
        {
            Building campus = GameData.Prototypes.GetBuildingPrototype("Campus");
            battleBuilding = new BattleBuilding(campus.Prefab, campus, UUID.Player);
            battleBuilding.coordinate = Vector2Int.zero;

        }
    }
}
