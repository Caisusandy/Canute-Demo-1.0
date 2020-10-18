using Canute.BattleSystem.UI;
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
        [SerializeField] private List<CellMark> marks = new List<CellMark>();


        public Cell data;
        public override EntityData Data => data;
        public override CellEntity OnCellOf => this;


        public List<CellEntity> NearByCells => Game.CurrentBattle.MapEntity.GetNearbyCell(this);

        public virtual CellEntity LeftUp => Game.CurrentBattle.MapEntity.GetCellEntityByHex(HexCoord.x - 1, HexCoord.y + 1);
        public virtual CellEntity Left => Game.CurrentBattle.MapEntity.GetCellEntityByHex(HexCoord.x - 1, HexCoord.y);
        public virtual CellEntity LeftDown => Game.CurrentBattle.MapEntity.GetCellEntityByHex(HexCoord.x, HexCoord.y - 1);
        public virtual CellEntity RightUp => Game.CurrentBattle.MapEntity.GetCellEntityByHex(HexCoord.x, HexCoord.y + 1);
        public virtual CellEntity Right => Game.CurrentBattle.MapEntity.GetCellEntityByHex(HexCoord.x + 1, HexCoord.y);
        public virtual CellEntity RightDown => Game.CurrentBattle.MapEntity.GetCellEntityByHex(HexCoord.x + 1, HexCoord.y - 1);

        public override BattleProperty.Position StandPostion => BattleProperty.Position.land;

        public ArmyEntity HasArmyStandOn => data.HasArmyStandOn?.Entity;
        public BuildingEntity HasBuildingStandOn => data.HasBuildingStandOn?.Entity;

        public List<CellMark> Marks { get => marks; set => marks = value; }


        //public CellMark Mark { get => GetMark(); set => mark = value; }

        public override void Awake()
        {
            base.Awake();
            this.NewUUID();
            entityMark = new MarkController(CellMark.Type.select);
        }

        public override void Start()
        {
            base.Start();
            GetComponent<SpriteRenderer>().sortingLayerName = "Map";
            GetComponent<SpriteRenderer>().sortingOrder = -y;

            if (data.hide)
            {
                GetComponent<SpriteRenderer>().enabled = false;
                GetComponent<Collider2D>().enabled = false;
            }

            GetComponent<SpriteRenderer>().color = !data.canStandOn ? new Color(150f / 255, 150f / 255, 150f / 255) : Color.white;

            //SetCellSprite();
        }

        [ContextMenu("reload cell sprite")]
        public void SetCellSprite()
        {
            GetComponent<SpriteRenderer>().sprite = GetCellSprite();
            GetComponent<SpriteRenderer>().size = new Vector2(40, 67.72008f);
        }

        public void SetCellSprite(Sprite sprite)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
            GetComponent<SpriteRenderer>().size = new Vector2(40, 67.72008f);
        }

        private Sprite GetCellSprite()
        {
            return GameData.SpriteLoader.Get(SpriteAtlases.cells, data.terrain.ToString() + UnityEngine.Random.Range(0, data.terrain == Terrain.Plain ? 11 : 3));
        }

        public override void OnMouseDown()
        {
            if (StorySystem.StoryDisplayer.instance) return;
            if (PausePanel.instance.enabled) return;
            base.OnMouseDown();
            //Select();
            //TriggerSelectEvent(true);
        }

        public override void OnMouseDrag()
        {
            if (StorySystem.StoryDisplayer.instance) return;
            if (PausePanel.instance.enabled) return;
            MapEntity.MoveMap();
        }

        public override void OnMouseUp()
        {
            if (StorySystem.StoryDisplayer.instance) return;
            if (PausePanel.instance.enabled) return;
            if (HasArmyStandOn)
            {
                Unselect();
                HasArmyStandOn.OnMouseUp();
            }
            else if (HasBuildingStandOn)
            {
                Unselect();
                HasBuildingStandOn.OnMouseUp();
            }
            else if (MapEntity.WasOnDrag)
            {
                Debug.Log("was dragin map");
            }
            else
            {
                base.OnMouseUp();
            }
            MapEntity.WasOnDrag = false;
        }

        public override void Highlight()
        {
            entityMark.Refresh(this);
            entityMark.Display();
            IsHighlighted = true;
        }

        public override void Unhighlight()
        {
            //Debug.Log("Cell Unhighlighted");
            entityMark.ClearDisplay();
            IsHighlighted = false;
        }


        /// <summary>
        /// Triggerer of EntityArrive
        /// </summary>
        public void Enter(OnMapEntity onMapEntity, Effect moveEffect)
        {
            onMapEntity.transform.SetParent(transform);
            onMapEntity.OnMapData.Coordinate = Coordinate;
            onMapEntity.entityMark.Refresh(this);
            onMapEntity.entityMark.Display();

            Status.TriggerOf(TriggerCondition.Conditions.entityArrive, ref moveEffect, this, onMapEntity, Game.CurrentBattle);
            //this.TriggerOf(TriggerCondition.Conditions.entityArrive, ref effect);
            //onMapEntity.TriggerOf(TriggerCondition.Conditions.entityArrive, ref effect);
            //Game.CurrentBattle.TriggerOf(TriggerCondition.Conditions.entityArrive, ref effect);
        }

        /// <summary>
        /// Triggerer of EntityLeft
        /// </summary>
        public void Leave(OnMapEntity onMapEntity, Effect moveEffect)
        {
            Status.TriggerOf(TriggerCondition.Conditions.entityLeft, ref moveEffect, this, onMapEntity, Game.CurrentBattle);
            //this.TriggerOf(TriggerCondition.Conditions.entityLeft, ref effect);
            //onMapEntity.TriggerOf(TriggerCondition.Conditions.entityLeft, ref effect);
            //Game.CurrentBattle.TriggerOf(TriggerCondition.Conditions.entityLeft, ref effect);
            onMapEntity.entityMark.Refresh();
        }

        public void Open()
        {
            data.canStandOn = true;
            GetComponent<SpriteRenderer>().color = Color.white;
        }

        public void Close()
        {
            data.canStandOn = false;
            GetComponent<SpriteRenderer>().color = new Color(150f / 255, 150f / 255, 150f / 255);
        }

        public override void FaceTo(bool isToRight)
        {
            //cellEntity does not face to anyone
        }

        public CellEntity GetByDirection(MapDirection direction)
        {
            switch (direction)
            {
                case MapDirection.RU:
                    return RightUp;
                case MapDirection.R:
                    return Right;
                case MapDirection.RD:
                    return RightDown;
                case MapDirection.LD:
                    return LeftDown;
                case MapDirection.L:
                    return Left;
                case MapDirection.LU:
                    return LeftUp;
                default: return null;
            }
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
