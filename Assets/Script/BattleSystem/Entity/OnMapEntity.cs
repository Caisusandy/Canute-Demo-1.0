using System.Linq;
using UnityEngine;
namespace Canute.BattleSystem
{
    /// <summary>
    /// 地图上的实体
    /// </summary>
    public abstract class OnMapEntity : InteractableEntity, IOnMapEntity, IStatusContainer
    {
        public static OnMapEntity SelectingEntity;


        /// <summary> 物体所在的格子 <para>The CellEntity this object stand on </para> </summary>
        public virtual CellEntity OnCellOf => transform.parent.GetComponent<CellEntity>();
        /// <summary> 六边形坐标 <para> Coordinate for hex-map </para></summary>
        public virtual Vector3Int HexCoord => OnCellOf.data.HexCoord;
        /// <summary> 坐标 <para> Coordinate </para></summary>
        public virtual Vector2Int Position => OnCellOf.data.Position;
        /// <summary> x <para></para></summary>
        public virtual int x => OnCellOf.data.Position.x;
        /// <summary> y <para></para></summary>
        public virtual int y => OnCellOf.data.Position.y;
        /// <summary>  <para></para></summary>
        public virtual OnMapEntityData OnMapData => Data as OnMapEntityData;
        /// <summary> 允许移动 <para> Determine whether the entity allows to move</para></summary>
        public virtual bool AllowMove { get => OnMapData.AllowMove; set => OnMapData.AllowMove = value; }
        /// <summary> </summary>
        public virtual StatusList StatList => OnMapData.StatList;
        /// <summary> </summary>
        public virtual StatusList GetAllStatus() => OnMapData.GetAllStatus();
        public abstract BattleProperty.Position StandPostion { get; }

        public int GetPointDistanceOf(OnMapEntity other)
        {
            return GetPointDistanceOf(other.HexCoord);
        }

        public int GetPointDistanceOf(Vector3Int v3)
        {
            int dx = Mathf.Abs(HexCoord.x - v3.x);
            int dy = Mathf.Abs(HexCoord.y - v3.y);
            int dz = Mathf.Abs(HexCoord.z - v3.z);

            //Debug.Log(new Vector3Int(dx, dy, dz));

            return Mathf.Max(dx, dy, dz);
        }

        public int GetRealDistanceOf(OnMapEntity onMapEntity2, OnMapEntity movingEntity = null)
        {
            return GetRealDistanceOf(OnCellOf, onMapEntity2, movingEntity);
        }

        public override void Select()
        {
            SelectingEntity?.Unselect();
            base.Select();
            SelectingEntity = this;
        }

        public override void Unselect()
        {
            SelectingEntity = null;
            base.Unselect();
        }

        public static int GetRealDistanceOf(OnMapEntity onMapEntity, OnMapEntity onMapEntity2, OnMapEntity movingEntity = null)
        {
            return PathFinder.GetPath(onMapEntity.OnCellOf, onMapEntity2.OnCellOf, movingEntity as ArmyEntity, false, movingEntity is ArmyEntity).Count;
        }
    }
}