using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Canute.BattleSystem
{

    /// <summary>
    /// Basic Entity interface of Canute
    /// </summary>
    public interface IInteractableEntity : IEntity, IOwnable, IUUIDLabeled, INameable
    {
        void Select();
        void Unselect();
        bool IsHighlighted { get; set; }
        bool IsSelected { get; set; }
    }

    /// <summary>
    /// Basic Entity interface of Canute
    /// </summary>
    public interface IEntity : IOwnable, IUUIDLabeled, INameable
    {
        // Unity MonoBehaviour  
        bool enabled { get; set; }
        GameObject gameObject { get; }
        Transform transform { get; }
        Animator Animator { get; }
        Entity entity { get; }
    }

    public static class Entities
    {
        public static Entity ToEntity(this IEntity entity)
        {
            return entity as Entity;
        }

        public static List<Entity> ToEntities<T>(this IEnumerable<T> entities) where T : IEntity
        {
            List<Entity> entities1 = new List<Entity>();
            foreach (var entity in entities)
            {
                entities1.Add(entity as Entity);
            }
            return entities1;
        }
    }
}