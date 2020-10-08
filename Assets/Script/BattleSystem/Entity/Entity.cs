using Canute.LanguageSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Canute.BattleSystem
{

    public delegate void SelectEvent(Entity entity);
    public delegate void EntityEvent(params object[] vs);

    /// <summary>
    /// The component of any game entity. All other entity are inherit from this class. You cannot use this class directly </para>
    /// <para> all entities will be add into Entity.entities when Awake, remove when destoryed </para>
    /// </summary>
    public abstract class Entity : MonoBehaviour, IEntity, INameable, IUUIDLabeled
    {
        #region Normal Actions
        protected const string isAttacking = "isAttacking";
        protected const string isPerformingSkill = "isPerformingSkill";
        protected const string isMoving = "isMoving";
        protected const string isDefeated = "isDefeated";
        protected const string isDefensing = "isDefensing";
        protected const string isWinning = "isWinning";
        protected const string isDragingCard = "isDraging";
        protected const string isIdle = "isIdle";
        #endregion

        public static List<Entity> entities = new List<Entity>();
        public static event SelectEvent SelectEvent;
        public static event SelectEvent UnselectEvent;

        /// <summary> Data of the entity </summary>
        public abstract EntityData Data { get; }
        /// <summary> Name of the entity </summary>
        public virtual string Name => Data?.Name;
        /// <summary> Owner(a player) of the entity (actually store in Data)</summary>
        public virtual Player Owner { get => Data?.Owner; set => Data.Owner = value; }
        /// <summary> UUID of the entity (actually store in Data)</summary>
        public virtual UUID UUID { get => Data is null ? UUID.Empty : Data.UUID; set => Data.UUID = value; }
        /// <summary> Animator of the entity</summary>
        public virtual Animator Animator { get => GetComponent<Animator>(); }
        /// <summary> entity</summary>
        public virtual Entity entity => this;


        /// <summary> is any action is occuring </summary>
        public bool IsIdle => Animator.GetBool(isIdle);


        public virtual void Awake()
        {
            if (UUID == UUID.Empty)
            {
                Debug.LogWarning("An non-UUIDLabled entity has spawned named:" + Name);
            }
            if (!entities.Contains(this))
            {
                entities.Add(this);
            }
        }

        public virtual void Start()
        {
            if (UUID == UUID.Empty)
            {
                Debug.LogError("An non-UUIDLabled entity has spawned named:" + Name + ", An UUID is replaced for it" + Data?.ToString());
                this.NewUUID();
            }
            else if (Get(UUID) != this)
            {
                Debug.LogError("UUID crash! a new UUID is replaced for this entity " + Data.ToString());
                this.NewUUID();
            }
        }

        public virtual void Update()
        {

        }

        public virtual void OnDestroy()
        {
            Animator.RemoveFromBattle();
            entities?.Remove(this);
            //Debug.Log("the entity is destroyed " + this);
        }

        /// <summary>
        /// Detroy the Entity and clear anything neccesary
        /// </summary>
        public virtual void Destroy()
        {
            Destroy(gameObject);
        }

        /// <summary>
        /// Triggerer of the Entity Select Event
        /// </summary>
        /// <param name="IsSelected"></param>
        public virtual void TriggerSelectEvent(bool IsSelected)
        {
            if (MapEntity.WasOnDrag)
            {
                return;
            }
            if (IsSelected) { SelectEvent?.Invoke(this); }
            else { UnselectEvent?.Invoke(this); }
        }

        /// <summary>
        /// perform an action in coroutine
        /// </summary>
        /// <param name="action"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public virtual Coroutine Action(Func<object[], IEnumerator> action, params object[] param)
        {
            var c = StartCoroutine(Action(action(param)));
            return c;
        }

        /// <summary>
        /// perform an action in coroutine
        /// </summary>
        /// <param name="entityEvent"></param>
        /// <returns></returns>
        public virtual Coroutine Action(params EntityEventPack[] entityEvent)
        {
            //if (isInAction)
            //{
            //    return null;
            //} 
            var coroutine = StartCoroutine(Action(AsIEnumerator(entityEvent)));
            return coroutine;
        }

        /// <summary>
        /// Transfer a EntityEventPack to a Enumerator
        /// </summary>
        /// <param name="funcs"></param>
        /// <returns></returns>
        protected IEnumerator AsIEnumerator(IEnumerable<EntityEventPack> funcs)
        {
            yield return null;
            foreach (EntityEventPack item in funcs)
            {
                yield return item.Execute();
            }
            yield return null;
        }

        /// <summary>
        /// Allow the coroutine to sleep
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        protected IEnumerator Sleep(params object[] param)
        {
            float second = float.Parse(param[0].ToString());
            Debug.Log(second);
            yield return new WaitForSeconds(second);
        }

        /// <summary>
        /// executor of action, only use in Action(Func, params[])
        /// </summary>
        /// <param name="enumerator"></param>
        /// <returns></returns>
        protected virtual IEnumerator Action(IEnumerator enumerator)
        {
            //Debug.Log("action start");
            yield return enumerator;
        }

        /// <summary>
        /// Change Entity to a state of Performing Animation
        /// </summary>
        protected virtual void InPerformingAnimation()
        {
            Animator.SetBool(isIdle, false);
            Animator.AddToBattle();
        }

        /// <summary>
        /// Change Entity to a normal state
        /// </summary>
        /// <param name="vs"></param>
        protected virtual void Idle(params object[] vs)
        {
            foreach (var item in Animator.parameters)
            {
                if (item.type == AnimatorControllerParameterType.Bool)
                {
                    Animator.SetBool(item.name, false);
                }
            }
            Animator.SetBool(isIdle, true);
            Animator.TryRemoveFromBattle();

        }

        /// <summary>
        /// Change Entity to a normal state
        /// </summary>
        /// <param name="vs"></param>
        protected virtual IEnumerator IdleDelay(params object[] vs)
        {
            float seconds = (float)vs[0];
            yield return Sleep(seconds);
            yield return new EntityEventPack(Idle).Execute();
        }

        /// <summary>
        /// Change Entity to a normal state
        /// </summary>
        /// <param name="vs"></param>
        protected virtual IEnumerator Idle()
        {
            return IdleDelay(0f);
        }


        #region Static Methods

        public static Entity Get(UUID sourceEntity)
        {
            foreach (Entity item in entities)
            {
                if (item.UUID == sourceEntity && item)
                {
                    return item;
                }
            }
            return null;
        }

        public static T Get<T>(UUID sourceEntity) where T : Entity
        {
            foreach (Entity item in entities)
            {
                if (item.UUID == sourceEntity && item && item is T)
                {
                    return item as T;
                }
            }
            return null;
        }

        public static List<Entity> Get(List<UUID> sourceEntity)
        {
            List<Entity> entities = new List<Entity>();
            foreach (UUID item in sourceEntity)
            {
                entities.Add(Get(item));
            }
            return entities;
        }

        public static List<T> Get<T>(List<UUID> sourceEntity) where T : Entity
        {
            List<T> entities = new List<T>();
            foreach (UUID item in sourceEntity)
            {
                entities.Add(Get<T>(item));
            }
            return entities;
        }

        public static void Initialize()
        {
            foreach (var item in entities)
            {
                if (item)
                    item.Destroy();
            }
            entities.Clear();
            ArmyEntity.onMap.Clear();
            BuildingEntity.onMap.Clear();
            CardEntity.cards.Clear();
            ArmyCardEntity.armyCards.Clear();
            OnMapEntity.SelectingEntity = null;
            CardEntity.SelectingCard = null;
            CardEntity.SelectingEntity = null;
        }

        public static void ClearAllEvent()
        {
            foreach (var item in SelectEvent.GetInvocationList())
            {
                SelectEvent -= item as SelectEvent;
            }
            foreach (var item in UnselectEvent.GetInvocationList())
            {
                UnselectEvent -= item as SelectEvent;
            }
        }

        #endregion
    }

    public abstract class InteractableEntity : Entity, IInteractableEntity
    {
        public Color HightGreen => new Color(0.7f, 1, 0.7f);

        [Header("Entity Status")]
        [SerializeField] protected bool isHighlighted;
        [SerializeField] protected bool isSelected;

        public virtual bool IsHighlighted { get => isHighlighted; set => isHighlighted = value; }
        public virtual bool IsSelected { get => isSelected; set => isSelected = value; }

        /// <summary> Select this entity </summary>
        public virtual void Select()
        {
            IsSelected = true;
            Highlight();
        }

        /// <summary> Unselect this entity </summary>
        public virtual void Unselect()
        {
            Unhighlight();
            IsSelected = false;
        }

        /// <summary> Highlight this entity </summary>
        public virtual void Highlight()
        {
            if (isHighlighted)
            {
                return;
            }
            try
            {
                transform.localScale *= 1.2f;
                IsHighlighted = true;
            }
            catch { }
        }

        /// <summary> Unhighlight this entity </summary>
        public virtual void Unhighlight()
        {
            if (!isHighlighted)
            {
                return;
            }
            try
            {
                transform.localScale = new Vector3(1, 1, 1);
                IsHighlighted = false;
            }
            catch { }
        }

        public virtual void CardTargetHighlight()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                var c = spriteRenderer.color;
                c.a -= 0.5f;
                spriteRenderer.color = c;
            }
        }

        public virtual void CardTargetUnhighlight()
        {
            var spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer)
            {
                var c = spriteRenderer.color;
                c.a = 1;
                spriteRenderer.color = c;
            }
        }

        /// <summary>
        /// toggle the selection status of the entity
        /// </summary>
        /// <returns></returns>
        public virtual bool ToggleSelect()
        {
            if (IsSelected)
            {
                Unselect();
            }
            else
            {
                Select();
            }
            return IsSelected;
        }

        public virtual void OnMouseDown() { }

        public virtual void OnMouseDrag() { }

        public virtual void OnMouseUp() { ToggleSelect(); TriggerSelectEvent(IsSelected); }

    }

    public abstract class DecorativeEntity : Entity
    {
        public override void Awake() { }
        public override void Start() { }
        public override Player Owner => null;
        public override string Name => string.Empty;
        public override UUID UUID { get => UUID.Empty; set => Debug.LogError("trying to assign a decorative entity a UUID is invalid"); }
    }

    /// <summary>
    /// an advanced coroutine pack
    /// </summary>
    public struct EntityEventPack
    {
        private EntityEvent entityEvent;
        private Func<object[], IEnumerator> enumerator;
        private object[] vs;

        public IEnumerator Execute()
        {
            return enumerator is null ? AsIEnumerator(entityEvent) : enumerator(vs);
        }

        public IEnumerator GetEnumerator()
        {
            return enumerator(vs);
        }

        public IEnumerator AsIEnumerator(EntityEvent func)
        {
            yield return null;
            func(vs);
            yield return null;
        }

        public EntityEventPack(EntityEvent entityEvent, params object[] vs)
        {
            this.entityEvent = entityEvent;
            enumerator = null;
            this.vs = vs;
        }

        public EntityEventPack(Func<object[], IEnumerator> enumerator, params object[] vs)
        {
            entityEvent = null;
            this.enumerator = enumerator;
            this.vs = vs;
        }

        public bool Equals(EntityEventPack other)
        {
            return other.enumerator.Equals(enumerator) && entityEvent.Equals(other.entityEvent) && vs.Equals(other.vs);
        }

        public override bool Equals(object obj)
        {
            return obj is EntityEventPack ? entityEvent.Equals((EntityEventPack)obj) : false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(EntityEventPack left, EntityEventPack right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(EntityEventPack left, EntityEventPack right)
        {
            return !(left == right);
        }
    }
}