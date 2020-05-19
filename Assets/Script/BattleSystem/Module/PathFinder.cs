//using Chronicle.Language;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Canute.BattleSystem
{

    public static class PathFinder
    {
        public const int MaxiumPath = 10;

        /// <summary>
        /// 以层次寻找路径的寻路算法
        /// </summary>
        /// <param name="startCell">the cell start from</param>
        /// <param name="endCell"> the destination </param>
        /// <param name="armyEntity"> the <paramref name="armyEntity"/> that try to move (only use for army find path)</param>
        /// <param name="containOrigin"> does the path contain the origin</param>
        /// <param name="ignoreOtherArmy"> whether ignore the army on the path or walk around it </param>
        /// 
        /// <returns></returns>
        public static List<CellEntity> GetPath(CellEntity startCell, CellEntity endCell, ArmyEntity armyEntity, bool containOrigin = false, bool ignoreOtherArmy = false)
        {
            List<CellEntity> currentLevel = new List<CellEntity> { startCell };
            List<List<CellEntity>> cellLevel = new List<List<CellEntity>> { new List<CellEntity>(currentLevel) };
            List<CellEntity> allCells = new List<CellEntity> { startCell };
            List<CellEntity> path = new List<CellEntity> { endCell };

            if (endCell.HasArmyStandOn && !ignoreOtherArmy)
            {
                return new List<CellEntity>();
            }

            if (!endCell.data.canStandOn)
            {
                return new List<CellEntity>();
            }

            while (!cellLevel[cellLevel.Count - 1].Contains(endCell) && cellLevel.Count <= armyEntity?.data.Properties.MoveRange)
            {
                List<CellEntity> nextLevel = new List<CellEntity>();    //下一层
                foreach (CellEntity cellEntity in currentLevel)             //浏览当前层
                {
                    List<CellEntity> nearbyCells = cellEntity.NearByCells;//获取临近格子 

                    for (int j = nearbyCells.Count - 1; j > -1; j--)
                    {
                        if (nearbyCells[j].HasArmyStandOn && !ignoreOtherArmy)
                        {
                            nearbyCells.Remove(nearbyCells[j]);
                        }
                        else if (!nearbyCells[j].data.canStandOn)
                        {
                            nearbyCells.Remove(nearbyCells[j]);
                        }
                    }

                    nearbyCells = nearbyCells.Except(allCells).ToList();  //保留没有被使用过的格子
                    nextLevel = nextLevel.Union(nearbyCells).ToList();   //给下一层加入临近格子
                    allCells = allCells.Union(nearbyCells).ToList();     //给所有格子加入临近格子 
                }
                if (nextLevel.Count == 0)
                {
                    Debug.Log("No more cell can go");
                    return new List<CellEntity>();
                }
                cellLevel.Add(nextLevel);   //下一层加入到层级里
                currentLevel = nextLevel;   //现在层转为下一层 

            }

            cellLevel.Reverse();
            cellLevel.RemoveAt(0);

            for (int i = 0; i < cellLevel.Count; i++)
            {
                List<CellEntity> nearbyCells = path[i].NearByCells.Intersect(cellLevel[i]).ToList();
                if (nearbyCells.Count == 0)
                {
                    Debug.Log("Nothing is in the reversed path???");
                    return new List<CellEntity>();
                }

                CellEntity predictCell = nearbyCells[0];
                Vector3 unit = (startCell.transform.position - endCell.transform.position).normalized;
                float smallestAngle = 180;
                //float smallestDistance = 100000;

                //距离判定
                foreach (CellEntity cell in nearbyCells)
                {
                    //float CellDistance = Map.GetPointDistance(Cell, EndCell);
                    Vector3 curUnit = (cell.transform.position - endCell.transform.position).normalized;
                    float angle = Mathf.Acos(Vector2.Dot(curUnit, unit) / (curUnit.magnitude * unit.magnitude));

                    if (angle < smallestAngle)
                    {
                        smallestAngle = angle;
                        predictCell = cell;
                    }
                    else if (angle == smallestAngle)
                    {
                        if (cell.GetPointDistanceOf(endCell) < predictCell.GetPointDistanceOf(endCell))
                        {
                            smallestAngle = angle;
                            predictCell = cell;
                        }
                    }

                    //float distance = Vector3.Distance(cell.transform.position, endCell.transform.position);
                    //if (distance < smallestDistance)
                    //{
                    //    smallestDistance = distance;
                    //    predictCell = cell;
                    //}
                    //else if (distance == smallestDistance)
                    //{
                    //    if (cell.GetPointDistanceOf(endCell) < predictCell.GetPointDistanceOf(endCell))
                    //    {
                    //        smallestDistance = distance;
                    //        predictCell = cell;
                    //    }
                    //}
                }
                path.Add(predictCell);
            }

            path.Reverse();

            if (containOrigin)
            {
                path = new List<CellEntity> { startCell }.Union(path).ToList();
            }
            else
            {
                path.Remove(startCell);
            }

            //foreach (CellEntity item in path)
            //{
            //    Debug.Log(item.Position);
            //}

            return path;
        }
    }
}
