using Canute.BattleSystem;
using Canute.Languages;
using System;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public abstract class Prototype : IPrototype
    {
        [SerializeField] protected string name;
        [SerializeField] protected Rarity rarity;
        [SerializeField] protected Sprite icon;
        [SerializeField] protected Sprite sprite;
        [SerializeField] protected Sprite portrait;
        [SerializeField] protected GameObject prefab;


        public string DisplayingName => this.Lang("name");
        public string Name => name;
        public Rarity Rarity => rarity;
        public Sprite Icon => GetIcon();
        public Sprite Portrait => GetPortrait();
        public Sprite Sprite => sprite;

        public virtual GameObject Prefab => prefab;


        protected virtual Sprite GetIcon()
        {
            if (Game.Configuration.UseCustomDefaultPrototype && !icon)
            {
                return GameData.Prototypes.GetPrototype(name)?.icon;
            }
            return icon;
        }

        protected virtual Sprite GetPortrait()
        {
            if (Game.Configuration.UseCustomDefaultPrototype && !icon)
            {
                return GameData.Prototypes.GetPrototype(name)?.portrait;
            }
            return portrait;
        }

        protected Prototype()
        {

        }

        public static implicit operator bool(Prototype prototype)
        {
            if (prototype is null)
            {
                return false;
            }
            else if (prototype.name is null)
            {
                return false;
            }
            else if (string.IsNullOrEmpty(prototype.name))
            {
                return false;
            }
            else if (prototype.name == "Empty")
            {
                return false;
            }
            return true;
        }
    }

    public abstract class PrototypeContainer : ScriptableObject, INameable
    {
        public abstract string Name { get; }
        /// <summary> rarity of the prototype </summary>
        public abstract Rarity Rarity { get; }
        /// <summary> icon of the prototype </summary>
        public abstract Sprite Icon { get; }
        /// <summary> sprite of the prototype </summary>
        public abstract Sprite Sprite { get; }

        public abstract Prototype GetPrototype();
    }

    public abstract class PrototypeContainer<T> : PrototypeContainer, INameable where T : Prototype, IPrototype, INameable
    {
        public bool isTemporaryPrototype;

        [SerializeField] protected T prototype;

        /// <summary> rarity of the prototype </summary>
        public override Rarity Rarity => Prototype.Rarity;
        /// <summary> icon of the prototype </summary>
        public override Sprite Icon => Prototype.Icon;
        /// <summary> sprite of the prototype </summary>
        public override Sprite Sprite => Prototype.Sprite;

        public override string Name => prototype.Name;

        public T Prototype => prototype;


        private void OnEnable()
        {
            if (isTemporaryPrototype)
            {
                return;
            }
            GameData.Prototypes.Add<PrototypeContainer<T>, T>(this);
        }

        public static implicit operator T(PrototypeContainer<T> prototypeContainer)
        {
            return prototypeContainer == null ? null : prototypeContainer.prototype;
        }

        public override Prototype GetPrototype()
        {
            return prototype;
        }

        public void SetPrototype(T prototype)
        {
            this.prototype = prototype;
        }
    }

    public interface IPrototype : INameable
    {
        /// <summary> rarity of the prototype </summary>
        Rarity Rarity { get; }
        /// <summary> icon of the prototype </summary>
        Sprite Icon { get; }
        /// <summary> sprite of the prototype </summary>
        Sprite Sprite { get; }
        Sprite Portrait { get; }
    }
}