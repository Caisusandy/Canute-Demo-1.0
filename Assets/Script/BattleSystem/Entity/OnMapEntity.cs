using Canute.BattleSystem.UI;
using System;
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
        public MarkController entityMark;


        public virtual CellEntity OnCellOf => transform.parent.GetComponent<CellEntity>();
        public virtual Vector3Int HexCoord => OnCellOf.data.HexCoord;
        public virtual Vector2Int Coordinate => OnCellOf.data.Coordinate;
        public virtual int x => OnCellOf.data.x;
        public virtual int y => OnCellOf.data.y;
        public virtual OnMapEntityData OnMapData => Data as OnMapEntityData;
        public virtual bool AllowMove { get => OnMapData.AllowMove; set => OnMapData.AllowMove = value; }
        public virtual StatusList StatList => OnMapData.StatList;
        public virtual StatusList GetAllStatus() => OnMapData.GetAllStatus();
        public new OnMapEntity entity => Get<OnMapEntity>(UUID);
        public abstract BattleProperty.Position StandPostion { get; }

        public int GetPointDistanceOf(OnMapEntity destination)
        {
            return GetPointDistanceOf(destination.HexCoord);
        }

        private int GetPointDistanceOf(Vector3Int v3)
        {
            int dx = Mathf.Abs(HexCoord.x - v3.x);
            int dy = Mathf.Abs(HexCoord.y - v3.y);
            int dz = Mathf.Abs(HexCoord.z - v3.z);

            //Debug.Log(new Vector3Int(dx, dy, dz));

            return Mathf.Max(dx, dy, dz);
        }
        public int GetRealDistanceOf(OnMapEntity destination, OnMapEntity movingEntity = null)
        {
            return GetRealDistanceOf(OnCellOf, destination, movingEntity);
        }

        public virtual void FaceTo(OnMapEntity onMapEntity)
        {
            var a = onMapEntity.transform.position - transform.position;
            FaceTo(a.x > 0);
        }

        public virtual void FaceTo(bool isToRight)
        {
            var scale = transform.localScale;
            scale.x = isToRight ? 1 : -1;
            transform.localScale = scale;
        }

        public override void Select()
        {
            SelectingEntity.Exist()?.Unselect();
            base.Select();
            SelectingEntity = this;
        }

        public override void Unselect()
        {
            if (SelectingEntity != this)
                SelectingEntity.Exist()?.Unselect();
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

        internal static void SetAllEntityCollider(bool v)
        {
            foreach (var item in entities)
            {
                if (item is OnMapEntity)
                {
                    try
                    {
                        item.GetComponent<Collider2D>().enabled = false;
                    }
                    catch { }
                }
            }
        }
    }
}