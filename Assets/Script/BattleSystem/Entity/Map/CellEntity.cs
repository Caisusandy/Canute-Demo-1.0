using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    /// <summary>
    /// 格子实体
    /// </summary>
    public class CellEntity : OnMapEntity
    {
        public Mark mark;
        public Cell data;
        public override EntityData Data => data;
        public override CellEntity OnCellOf => this;


        public virtual List<CellEntity> NearByCells => Game.CurrentBattle.MapEntity.GetNearbyCell(this);
        public virtual CellEntity LeftUp => Game.CurrentBattle.MapEntity.GetCell(x - 1, y + 1);
        public virtual CellEntity Left => Game.CurrentBattle.MapEntity.GetCell(x - 1, y);
        public virtual CellEntity LeftDown => Game.CurrentBattle.MapEntity.GetCell(x, y - 1);
        public virtual CellEntity RightUp => Game.CurrentBattle.MapEntity.GetCell(x, y + 1);
        public virtual CellEntity Right => Game.CurrentBattle.MapEntity.GetCell(x + 1, y);
        public virtual CellEntity RightDown => Game.CurrentBattle.MapEntity.GetCell(x + 1, y - 1);
        public override BattleProperty.Position StandPostion => BattleProperty.Position.land;

        public virtual ArmyEntity HasArmyStandOn => Game.CurrentBattle?.GetArmy(Coordinate)?.Entity;
        public virtual BuildingEntity HasBuildingStandOn => Game.CurrentBattle?.GetBuilding(Coordinate)?.Entity;


        public static bool WasOnDrag { get; private set; }

        public override void Awake()
        {
            base.Awake();
            this.NewUUID();
            data.Coordinate = Game.CurrentBattle.MapEntity.GetPosition(this);
            GetComponent<SpriteRenderer>().sortingLayerName = "Map";
            GetComponent<SpriteRenderer>().sortingOrder = -y;
        }

        public override void Start()
        {
            base.Start();

            if (data.hide)
            {
                GetComponent<SpriteRenderer>().enabled = false;
            }
            GetComponent<SpriteRenderer>().sprite = GameData.SpriteLoader.Get(SpriteAtlases.cells, data.terrain.ToString() + UnityEngine.Random.Range(0, 11));
        }

        public override void OnMouseDrag()
        {
            MoveMap();
        }

        public override void OnMouseUp()
        {
            if (HasArmyStandOn)
            {
                HasArmyStandOn.OnMouseUp();
            }
            else if (HasBuildingStandOn)
            {
                HasBuildingStandOn.OnMouseUp();
            }
            else
            {
                base.OnMouseUp();
            }
            WasOnDrag = false;
        }

        public override void Highlight()
        {
            Highlight(Mark.Type.select);
            IsHighlighted = true;
        }

        public override void Unhighlight()
        {
            Unhighlight(Mark.Type.select);
            IsHighlighted = false;
        }

        public virtual void Highlight(Mark.Type type)
        {
            //if (!transform.Find("Mark"))
            //{
            //    GameObject Mark = Instantiate(Game.EntityPrefabs.CellMark, transform);
            //    Mark.name = "Mark";
            //    Mark.GetComponent<Mark>().Awake();
            //}
            mark.Load(type);
        }

        public virtual void Unhighlight(Mark.Type type)
        {
            mark?.Unload(type);
        }


        /// <summary>
        /// Triggerer of EntityArrive
        /// </summary>
        public virtual void Enter(OnMapEntity onMapEntity, Effect effect)
        {
            onMapEntity.transform.SetParent(transform);
            onMapEntity.OnMapData.Coordinate = Coordinate;

            this.Trigger(TriggerCondition.Conditions.entityArrive, ref effect);
            onMapEntity.Trigger(TriggerCondition.Conditions.entityArrive, ref effect);
        }

        /// <summary>
        /// Triggerer of EntityLeft
        /// </summary>
        public virtual void Leave(OnMapEntity onMapEntity, Effect effect)
        {
            this.Trigger(TriggerCondition.Conditions.entityLeft, ref effect);
            onMapEntity.Trigger(TriggerCondition.Conditions.entityLeft, ref effect);
        }

        public static void MoveMap()
        {
            Vector3 finalPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 delta = finalPos - Control.instance.inputPos;

            if (Vector3.Magnitude(delta) == 0)
            {
                return;
            }

            if (Game.CurrentBattle.OngoingAnimation.Count != 0)
            {
                return;
            }

            //Debug.Log(delta);
            MapEntity.CurrentMap.transform.position += delta;
            WasOnDrag = true;
        }

        public bool IsValidDestination(IOnMapEntity movingEntity)
        {
            if (this.HasArmyStandOn && movingEntity is ArmyEntity)
            {
                Debug.Log("Army tried to move to a place has army stand on");
                return false;
            }
            else if (this.HasBuildingStandOn && movingEntity is BuildingEntity)
            {
                Debug.Log("Building tried to move to a place has building stand on");
                return false;
            }
            else if (!data.canStandOn)
            {
                return false;
            }
            else if (data.hide)
            {
                return false;
            }
            return true;
        }


        public override string ToString()
        {
            return Name + ": " + Coordinate;
        }
    }
}
