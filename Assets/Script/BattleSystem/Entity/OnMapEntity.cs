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
        public virtual Vector2Int Coordinate => OnCellOf.data.Coordinate;
        /// <summary> x <para></para></summary>
        public virtual int x => OnCellOf.data.Coordinate.x;
        /// <summary> y <para></para></summary>
        public virtual int y => OnCellOf.data.Coordinate.y;
        /// <summary>  <para></para></summary>
        public virtual OnMapEntityData OnMapData => Data as OnMapEntityData;
        /// <summary> 允许移动 <para> Determine whether the entity allows to move</para></summary>
        public virtual bool AllowMove { get => OnMapData.AllowMove; set => OnMapData.AllowMove = value; }
        /// <summary> </summary>
        public virtual StatusList StatList => OnMapData.StatList;
        /// <summary> </summary>
        public virtual StatusList GetAllStatus() => OnMapData.GetAllStatus();
        public abstract BattleProperty.Position StandPostion { get; }

        /// <summary>
        /// Get the straight distance to <paramref name="destination"/>
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        public int GetPointDistanceOf(OnMapEntity destination)
        {
            return GetPointDistanceOf(destination.HexCoord);
        }

        /// <summary>
        /// Get the straight distance to hexcoord
        /// </summary>
        /// <param name="v3"></param>
        /// <returns></returns>
        public int GetPointDistanceOf(Vector3Int v3)
        {
            int dx = Mathf.Abs(HexCoord.x - v3.x);
            int dy = Mathf.Abs(HexCoord.y - v3.y);
            int dz = Mathf.Abs(HexCoord.z - v3.z);

            //Debug.Log(new Vector3Int(dx, dy, dz));

            return Mathf.Max(dx, dy, dz);
        }

        /// <summary>
        /// Get the real distance to <paramref name="destination"/>
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="movingEntity"></param>
        /// <returns></returns>
        public int GetRealDistanceOf(OnMapEntity destination, OnMapEntity movingEntity = null)
        {
            return GetRealDistanceOf(OnCellOf, destination, movingEntity);
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

        /// <summary>
        /// Get the real distance between two on map object
        /// </summary>
        /// <param name="onMapEntity">start point</param>
        /// <param name="onMapEntity2">end point</param>
        /// <param name="movingEntity">a moving on map entity</param>
        /// <returns></returns>
        public static int GetRealDistanceOf(OnMapEntity onMapEntity, OnMapEntity onMapEntity2, OnMapEntity movingEntity = null)
        {
            PathFinder.FinderParam finderParam = PathFinder.FinderParam.ignoreDestinationArmy | PathFinder.FinderParam.ignoreDestinationBuilding;
            if (movingEntity is ArmyEntity)
            {
                finderParam |= PathFinder.FinderParam.ignoreBuilding;
            }
            else if (movingEntity is BuildingEntity)
            {
                finderParam |= PathFinder.FinderParam.ignoreArmy;
            }

            return PathFinder.GetPath(onMapEntity.OnCellOf, onMapEntity2.OnCellOf, -1, finderParam).Count;
        }
    }
}