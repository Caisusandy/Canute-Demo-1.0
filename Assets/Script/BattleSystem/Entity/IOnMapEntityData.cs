using UnityEngine;

namespace Canute.BattleSystem
{
    public interface IOnMapEntityData : IEntityData, IStatusContainer
    {
        bool AllowMove { get; set; }
        Vector3Int HexCoord { get; }
        Cell OnCellOf { get; }
        Vector2Int Position { get; set; }
    }
}