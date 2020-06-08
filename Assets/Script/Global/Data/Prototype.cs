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
        [SerializeField] protected Sprite protrait;
        [SerializeField] protected GameObject prefab;


        public string DisplayingName => this.Lang("name");
        public string Name => name;
        public Rarity Rarity => rarity;
        public Sprite Icon => icon;
        public Sprite Portrait => protrait;
        public Sprite Sprite => sprite;

        public virtual GameObject Prefab => prefab;

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
            else if (prototype.name == "Empty")
            {
                return false;
            }
            return true;
        }
    }

    public abstract class PrototypeContainer<T> : ScriptableObject, INameable where T : Prototype, IPrototype, INameable
    {
        [SerializeField] protected T prototype;

        /// <summary> rarity of the prototype </summary>
        public Rarity Rarity => Prototype.Rarity;
        /// <summary> icon of the prototype </summary>
        public Sprite Icon => Prototype.Icon;
        /// <summary> sprite of the prototype </summary>
        public Sprite Sprite => Prototype.Sprite;

        public string Name => prototype.Name;

        public T Prototype => prototype;

        private void OnEnable()
        {
            GameData.Prototypes.Add<PrototypeContainer<T>, T>(this);
        }

        public static implicit operator T(PrototypeContainer<T> prototypeContainer)
        {
            return prototypeContainer == null ? null : prototypeContainer.prototype;
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