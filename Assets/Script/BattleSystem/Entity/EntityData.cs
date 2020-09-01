using Canute.LanguageSystem;
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
        [SerializeField] protected GameObject prefab;
        [SerializeField] protected UUID uuid;
        [SerializeField] protected UUID ownerUUID;

        public virtual Prototype Prototype { get => GameData.Prototypes.GetPrototype(name); set => name = value?.Name; }
        public virtual bool HasValidPrototype => !string.IsNullOrEmpty(name) ? Prototype : false;

        public virtual string Name => name;
        public virtual string DisplayingName => GetDisplayingName();
        public virtual Sprite Icon => GetIcon();
        public virtual Sprite Portrait => GetPortrait();
        public virtual Player Owner { get => Game.CurrentBattle?.GetPlayer(ownerUUID); set => ownerUUID = value is null ? UUID.Empty : value.UUID; }
        public virtual UUID UUID { get => uuid; set => uuid = value; }
        public virtual GameObject Prefab { get => prefab; set => prefab = value; }
        public Entity Entity => Entity.Get(UUID);

        public virtual object Clone()
        {
            return MemberwiseClone() as EntityData;
        }

        public override string ToString()
        {
            return "Name: " + Name + ";\nOwner " + Owner?.Name;
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

        private Sprite GetIcon()
        {
            return Prototype?.Icon;
        }

        private Sprite GetPortrait()
        {
            return Prototype?.Portrait;
        }

        protected virtual void NewUUID()
        {
            (this as IUUIDLabeled).NewUUID();
        }

        protected virtual string GetDisplayingName()
        {
            return Prototype.DisplayingName;
        }

        public static bool IsNullOrEmpty(EntityData entityData)
        {
            if (entityData is null)
            {
                return true;
            }
            if (string.IsNullOrEmpty(entityData.name))
            {
                return true;
            }
            return !entityData.HasValidPrototype;
        }

        public static implicit operator bool(EntityData entityData)
        {
            return !IsNullOrEmpty(entityData);
        }

    }

    [Serializable]
    public abstract class OnMapEntityData : EntityData, IStatusContainer, IOnMapEntityData
    {
        [SerializeField] protected bool allowMove;
        [Header("Coordinate")]
        public Vector2Int coordinate;
        [Header("Status List")]
        [SerializeField] protected StatusList stats = new StatusList();

        public virtual int x { get => coordinate.x; set { coordinate = new Vector2Int(value, y); } }
        public virtual int y { get => coordinate.y; set { coordinate = new Vector2Int(x, value); } }
        public virtual Vector2Int Coordinate { get => coordinate; set { coordinate = value; } }
        public virtual Vector3Int HexCoord => new Vector3Int(x - y / 2, y, x - y / 2 + y);
        public virtual bool AllowMove { get => allowMove; set => allowMove = value; }
        public virtual Cell OnCellOf => Game.CurrentBattle.MapEntity[Coordinate].data;

        public virtual StatusList StatList => stats;
        public virtual StatusList GetAllStatus() => StatList.Union(OnCellOf.StatList).ToStatList();

        public override string ToString()
        {
            return base.ToString() + ";\nPosition: " + Coordinate;
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