using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class CardCollider : MonoBehaviour
    {
        public CardEntity cardEntity;
        public OnMapEntity last;

        private void Update()
        {
            if (cardEntity.isDraging && cardEntity)
            {
                ColliderRay();
            }

        }

        /// <summary>
        /// For throw card away only
        /// </summary>
        /// <param name="collisionInfo"></param>
        private void OnTriggerEnter2D(Collider2D collisionInfo)
        {
            GameObject Entity = collisionInfo.gameObject;

            if (cardEntity.isDraging && cardEntity.IsSelected)
            {
                if (Entity.name == "Trash")
                {
                    cardEntity.isOnTrashCan = true;
                }
            }
            if (Entity.name == "Trash")
            {
                cardEntity.isOnTrashCan = true;
            }
        }


        /// <summary>
        /// Get Target
        /// </summary>
        private void ColliderRay()
        {
            // Cast a ray straight down.
            List<RaycastHit2D> hitObj = new List<RaycastHit2D>(10);
            ContactFilter2D contactFilter2D = new ContactFilter2D();
            Physics2D.Raycast(transform.position, -Vector2.zero, contactFilter2D.NoFilter(), hitObj);
            // If it hits something...

            for (int i = 0; i < hitObj.Count; i++)
            {
                RaycastHit2D hit = hitObj[i];
                if (hit.collider == null) //if collider found nothing
                {
                    continue;
                }

                Transform hitTransform = hit.transform;

                if (hitTransform.GetComponent<CardEntity>())
                {
                    CardEntity card = hitTransform.GetComponent<CardEntity>();
                    if (card != cardEntity)
                    {
                        UnselectCurrent();
                        CardEntity.SelectingEntity = card;
                        CardEntity.SelectingEntity.Highlight();
                    }
                    return;
                }

                if (!hitTransform.GetComponent<CellEntity>()) //if it is not a CellEntity, ignore
                {
                    if (i == hitObj.Count - 1 && hitObj.Count == 1)
                    {
                        UnselectCurrent();
                    }
                    continue;
                }

                if (!last) //if last entity is not exist anymore
                {
                    last = null;
                }

                if (last?.gameObject == hitTransform.gameObject)// if the entity found is not same as last entity collider found
                {
                    break;
                }
                else
                {
                    //Debug.Log("!!!!!!!!!!!!");
                    Transform possibleArmyTransform = hitTransform.Find("Army");

                    if (!possibleArmyTransform) //if there is no ArmyEntity in this object's children or itself, then consider it as cell
                    {
                        //Debug.Log("add Cell");
                        last = hitTransform.GetComponent<OnMapEntity>();
                        UnselectCurrent();
                        last?.Select();
                        CardEntity.SelectingEntity = last;
                    }
                    else if (!possibleArmyTransform.GetComponent<ArmyEntity>().IsSelected) //if there is an armyEntity and it is not select yet
                    {
                        //Debug.Log("add Army");
                        last = possibleArmyTransform.GetComponent<OnMapEntity>();
                        UnselectCurrent();
                        last?.Select();
                        CardEntity.SelectingEntity = last;
                    }
                    return;
                }
            }
        }

        private static void UnselectCurrent()
        {
            OnMapEntity.SelectingEntity.Exist()?.Unselect();
            CardEntity.SelectingEntity.Exist()?.Unselect();
            CardEntity.SelectingEntity = null;
        }
    }

}