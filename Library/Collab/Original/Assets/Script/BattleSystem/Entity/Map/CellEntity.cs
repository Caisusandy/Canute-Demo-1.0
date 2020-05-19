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
        public override StatList StatList { get => OnMapData.StatList; }


        public virtual List<CellEntity> NearByCells => Game.CurrentBattle.MapEntity.GetNearbyCell(this);
        public virtual CellEntity LeftUp => Game.CurrentBattle.MapEntity.GetCell(x - 1, y + 1);
        public virtual CellEntity Left => Game.CurrentBattle.MapEntity.GetCell(x - 1, y);
        public virtual CellEntity LeftDown => Game.CurrentBattle.MapEntity.GetCell(x, y - 1);
        public virtual CellEntity RightUp => Game.CurrentBattle.MapEntity.GetCell(x, y + 1);
        public virtual CellEntity Right => Game.CurrentBattle.MapEntity.GetCell(x + 1, y);
        public virtual CellEntity RightDown => Game.CurrentBattle.MapEntity.GetCell(x + 1, y - 1);

        public virtual ArmyEntity HasArmyStandOn => transform.Find("Army")?.GetComponent<ArmyEntity>();
        public virtual BuildingEntity HasBuildingStandOn => transform.Find("Building")?.GetComponent<BuildingEntity>();
        public virtual List<ArmyEntity> Armies => Get<ArmyEntity>(data.Armies.ToUUIDList());
        public virtual List<BuildingEntity> Buildings => Get<BuildingEntity>(data.Armies.ToUUIDList());

        public static bool wasOnDrag { get; private set; }

        public override void Awake()
        {
            base.Awake();
            this.NewUUID();
            data.Position = Game.CurrentBattle.MapEntity.GetPosition(this);
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
            wasOnDrag = false;
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

        public virtual void Enter(OnMapEntity onMapEntity)
        {
            onMapEntity.OnMapData.Position = Position;
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

            MapEntity.CurrentMap.transform.position += delta;
            wasOnDrag = true;
        }

        public override string ToString()
        {
            return Name + ": " + Position;
        }
    }
}
