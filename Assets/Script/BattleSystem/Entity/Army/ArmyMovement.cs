using Canute.BattleSystem.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    public static class ArmyMovement
    {
        public static ArmyEntity movingArmy;

        private static List<CellEntity> path;
        private static List<CellEntity> border;
        private static bool lockPath;

        public static void Initialize()
        {
            movingArmy = null;
            path = new List<CellEntity>();
            border = null;
            lockPath = false;
        }

        public static bool PrepareMove(ArmyEntity armyEntity)
        {
            Initialize();
            if (!armyEntity.HasAtLeastOneDestination())
            {
                Debug.Log("");
                return false;
            }

            movingArmy = armyEntity;
            AddMoveEvent();
            return true;
        }

        //public static void TryMove(Entity entity)
        //{
        //    CellEntity destination = (entity as OnMapEntity)?.OnCellOf;

        //    Card.LastCard.Effect.Type = Effect.Types.move;
        //    Card.LastCard.Effect.Source = movingArmy;
        //    Card.LastCard.Effect.Target = destination;
        //    Card.LastCard.Effect.Count = 1;
        //    Card.LastCard.Effect.Parameter = 0;

        //    if (!Card.LastCard.Play())
        //        Card.LastCard.Effect.Type = Effect.Types.enterMove;
        //    else
        //        RemoveMoveEvent();
        //}

        public static void TryMove()
        {
            Card.LastCard.Effect.Type = Effect.Types.move;
            Card.LastCard.Effect.Source = movingArmy;
            Card.LastCard.Effect.Target = path[path.Count - 1];
            Card.LastCard.Effect.Count = 1;
            Card.LastCard.Effect.Parameter = 0;

            if (!Card.LastCard.Play())
                Card.LastCard.Effect.Type = Effect.Types.enterMove;
            else
                RemoveMoveEvent();
        }

        #region 移动状态切换器
        /// <summary> 进入移动状态 </summary>
        private static void AddMoveEvent()
        {
            Game.CurrentBattle.InMotionAction();
            //Debug.Log(Game.CurrentBattle.CurrentStat);
            ShowMoveRange();
            StartSelectorListener();
            EffectExecute.AddSelectEvent(LockPath);
        }

        /// <summary> 退出移动状态 </summarys
        public static void RemoveMoveEvent()
        {
            EffectExecute.RemoveSelectEvent(LockPath);
            EndShowMoveRange();
            EndShowMovePath();
            Game.CurrentBattle.TryInNormal();
        }


        public static void ShowMoveRange()
        {
            border = movingArmy.GetMoveRange();
            //Game.CurrentBattle.MapEntity.StartCoroutine(new EntityEventPack(Draw).GetEnumerator());

            //IEnumerator Draw(params object[] vs)
            //{
            //    while (Game.CurrentBattle.CurrentStat == Battle.Stat.move)
            //    {
            //        Mark.Load(Mark.Type.moveRange, border);
            //        yield return new WaitForFixedUpdate();
            //    }
            //    yield return null;
            //}
        }


        public static void EndShowMoveRange()
        {
            Mark.Unload(Mark.Type.moveRange, border);
            border = null;
        }

        public static void StartSelectorListener()
        {
            border = movingArmy.GetMoveRange();
            Game.CurrentBattle.MapEntity.StartCoroutine(new EntityEventPack(Selector).GetEnumerator());

            IEnumerator Selector(params object[] vs)
            {
                while (Game.CurrentBattle.CurrentStat == Battle.Stat.move)
                {
                    if (!lockPath)
                    {
                        CellSeletor();
                    }
                    yield return new WaitForFixedUpdate();
                }
                yield return null;
            }
        }


        public static void ShowMovePath()
        {
            Mark.Load(Mark.Type.moveRange, path);
        }

        public static void EndShowMovePath()
        {
            Mark.Unload(Mark.Type.moveRange, path);
        }

        #endregion
        private static bool HasAtLeastOneDestination(this ArmyEntity armyEntity)
        {

            List<CellEntity> cellEntities = armyEntity.OnCellOf.NearByCells;
            for (int i = cellEntities.Count - 1; i >= 0; i--)
            {
                CellEntity cellEntity = cellEntities[i];
                if (cellEntity.HasArmyStandOn)
                {
                    cellEntities.RemoveAt(i);
                }
            }
            return cellEntities.Count != 0;
        }

        private static void CellSeletor()
        {
            // Cast a ray straight down.
            List<RaycastHit2D> hitObj = new List<RaycastHit2D>(10);
            ContactFilter2D contactFilter2D = new ContactFilter2D();
            Physics2D.Raycast(Control.UserInputPosition, -Vector2.zero, contactFilter2D.NoFilter(), hitObj);
            // If it hits something...

            for (int i = 0; i < hitObj.Count; i++)
            {
                RaycastHit2D hit = hitObj[i];
                if (hit.collider == null) //if collider found nothing
                {
                    continue;
                }

                Transform hitTransform = hit.transform;
                OnMapEntity entity = hitTransform.GetComponent<OnMapEntity>();

                if (!entity) //if it is not a CellEntity, ignore
                {
                    continue;
                }

                CellEntity cell = entity.OnCellOf;
                AddPath(cell);
            }
        }

        public static void AddPath(Entity entity)
        {
            CellEntity cell = (entity as OnMapEntity).OnCellOf;
            if (!cell)
            {
                return;
            }

            if (!IsValidDestination(cell, movingArmy))
            {
                return;
            }

            EndShowMovePath();

            if (path.Count == 0)
            {
                path.Add(movingArmy.OnCellOf);
            }

            if (path.Contains(cell))
            {
                int index = path.IndexOf(cell);
                for (int j = path.Count - 1; j > index; j--)
                {
                    path.RemoveAt(j);
                }
            }
            else if (movingArmy.data.Properties.MoveRange <= path.Count - 1)
            {

            }
            else if (path[path.Count - 1].NearByCells.Contains(cell))
            {
                path.Add(cell);
            }
            else
            {
                List<CellEntity> between = PathFinder.GetPath(path[path.Count - 1], cell, movingArmy.data.Properties.MoveRange, PathFinder.FinderParam.ignoreBuilding);
                path.AddRange(movingArmy.data.Properties.MoveRange - 1 >= path.Count + between.Count ? between : new List<CellEntity>());
            }
            ShowMovePath();
        }

        public static void LockPath(Entity entity)
        {
            lockPath = !lockPath;
        }

        public static List<CellEntity> GetPath()
        {
            List<CellEntity> ret = path.ShallowClone();
            ret.Remove(movingArmy.Exist()?.OnCellOf);
            return ret;
        }

        public static bool IsValidDestination(CellEntity cellEntity, ArmyEntity armyEntity)
        {
            if (cellEntity.HasArmyStandOn && cellEntity.HasArmyStandOn != armyEntity)
            {
                return false;
            }
            else if (!cellEntity.data.canStandOn)
            {
                return false;
            }
            return true;
        }
    }
}
