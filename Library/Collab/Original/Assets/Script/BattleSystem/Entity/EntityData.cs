using Canute.Languages;
using System;
using System.Linq;
using UnityEngine;


namespace Canute.BattleSystem
{
    [Serializable]
    public abstract class EntityData : INameable, IUUIDLabeled, ICloneable, IEntityData
    {
        [Header("Basic Property")]
        [SerializeField] protected string name;
        [SerializeField, ContextMenuItem("Get a UUID", "NewUUID")] protected UUID uuid;
        [SerializeField] protected UUID ownerUUID;
        [SerializeField] protected GameObject prefab;

        public virtual Prototype Prototype { get => GameData.Prototypes.GetPrototype(name); set => name = value?.Name; }
        public virtual bool HasPrototype => !string.IsNullOrEmpty(name);

        public virtual string DisplayingName => Prototype.DisplayingName;
        public virtual Sprite DisplayingIcon => Prototype?.Icon;
        public virtual Sprite DisplayingPortrait => Prototype?.Portrait;
        public virtual string Name => name;
        public virtual Player Owner { get => Game.CurrentBattle?.GetPlayer(ownerUUID); set => ownerUUID = value is null ? UUID.Empty : value.UUID; }
        public virtual UUID UUID { get => uuid; set => uuid = value; }
        public virtual GameObject Prefab { get => prefab ?? GameData.Prefabs.DefaultBuilding; set => prefab = value; }
        public Entity Entity => Entity.Get(UUID);

        public virtual object Clone()
        {
            return MemberwiseClone() as EntityData;
        }

        public override string ToString()
        {
            return "Name: " + Name + ", Owner " + Owner?.Name;
        }

        protected EntityData()
        {
            NewUUID();
        }

        protected EntityData(string name) : this()
        {
            this.name = name;
        }

        protected EntityData(Prototype prototype) : this()
        {
            Prototype = prototype;
        }

        protected void NewUUID()
        {
            (this as IUUIDLabeled).NewUUID();
        }

        public static bool IsNullOrEmpty(EntityData battleLeader)
        {
            if (battleLeader is null)
            {
                return true;
            }
            return string.IsNullOrEmpty(battleLeader.name);
        }

    }

    [Serializable]
    public abstract class OnMapEntityData : EntityData, IStatusContainer, IOnMapEntityData
    {
        [SerializeField] protected bool allowMove;
        [Header("Coordinate")]
        public int x;
        public int y;
        [Header("Status List")]
        [SerializeField] protected StatList stats = new StatList();

        public virtual Vector2Int Position { get => new Vector2Int(x, y); set { x = value.x; y = value.y; } }
        public virtual Vector3Int HexCoord => new Vector3Int(x - y / 2, y, x - y / 2 + y);
        public virtual bool AllowMove { get => allowMove; set => allowMove = value; }
        public virtual Cell OnCellOf => Game.CurrentBattle.MapEntity[Position].data;

        public virtual StatList StatList => stats;
        public virtual StatList GetAllStatus() => StatList.Union(OnCellOf.StatList).ToStatList();

        public override string ToString()
        {
            return base.ToString() + " Position: " + Position;
        }

        protected OnMapEntityData() : base() { }

        protected OnMapEntityData(Prototype prototype) : base(prototype) { }
    }

    [Serializable]
    public abstract class NonOwnableEntityData : EntityData
    {
        public override Player Owner { get => null; set { } }

        protected NonOwnableEntityData() { }

        protected NonOwnableEntityData(Prototype prototype) : base(prototype) { }
    }
}