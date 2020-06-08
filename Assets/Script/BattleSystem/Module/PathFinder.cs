//using Chronicle.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


namespace Canute.BattleSystem
{

    public static class PathFinder
    {
        public const int MaxiumPath = 10;

        [Flags]
        public enum FinderParam
        {
            none = 0,
            containOrgin = 1,
            ignoreLandArmy = 2,
            ignoreAirArmy = 4,
            ignoreArmy = ignoreAirArmy | ignoreLandArmy,
            ignoreBuilding = 8,
            ignoreDestinationArmy = 16,
            ignoreDestinationBuilding = 32
        }

        /// <summary>
        /// 以层次寻找路径的寻路算法
        /// </summary>
        /// <param name="startCell">the cell start from</param>
        /// <param name="endCell"> the destination </param> 
        /// <param name="moveRange"></param>
        /// <param name="finderParam"></param>
        /// <returns></returns>
        public static List<CellEntity> GetPath(CellEntity startCell, CellEntity endCell, int moveRange, FinderParam finderParam)
        {
            List<CellEntity> currentLevel = new List<CellEntity> { startCell };
            List<List<CellEntity>> cellLevel = new List<List<CellEntity>> { new List<CellEntity>(currentLevel) };
            List<CellEntity> allCells = new List<CellEntity> { startCell };
            List<CellEntity> path = new List<CellEntity> { endCell };

            if (endCell.HasArmyStandOn && !finderParam.HasFlag(FinderParam.ignoreDestinationArmy))
            {
                if (!finderParam.HasFlag(FinderParam.ignoreAirArmy) && endCell.HasArmyStandOn.data.StandPosition == BattleProperty.Position.air)
                {
                    Debug.Log("Has army on end point");
                    return new List<CellEntity>();
                }
                else if (!finderParam.HasFlag(FinderParam.ignoreLandArmy) && endCell.HasArmyStandOn.data.StandPosition == BattleProperty.Position.land)
                {
                    Debug.Log("Has army on end point");
                    return new List<CellEntity>();
                }
            }

            if (endCell.HasBuildingStandOn && !finderParam.HasFlag(FinderParam.ignoreBuilding) && !finderParam.HasFlag(FinderParam.ignoreDestinationBuilding))
            {
                Debug.Log("Has building on end point");
                return new List<CellEntity>();
            }

            if (!endCell.data.canStandOn)
            {
                Debug.Log("can't stand on end point");
                return new List<CellEntity>();
            }

            while (!cellLevel[cellLevel.Count - 1].Contains(endCell) && (cellLevel.Count <= moveRange || moveRange == -1))
            {
                List<CellEntity> nextLevel = new List<CellEntity>();    //下一层
                foreach (CellEntity cellEntity in currentLevel)             //浏览当前层
                {
                    List<CellEntity> nearbyCells = cellEntity.NearByCells;//获取临近格子 

                    for (int j = nearbyCells.Count - 1; j > -1; j--)
                    {
                        CellEntity curCell = nearbyCells[j];
                        if (curCell == endCell)
                        {
                            break;
                        }
                        if (curCell.HasArmyStandOn)
                        {
                            if (!finderParam.HasFlag(FinderParam.ignoreAirArmy) && curCell.HasArmyStandOn.data.StandPosition == BattleProperty.Position.air)
                            {
                                nearbyCells.Remove(curCell);
                                continue;
                            }
                            else if (!finderParam.HasFlag(FinderParam.ignoreLandArmy) && curCell.HasArmyStandOn.data.StandPosition == BattleProperty.Position.land)
                            {
                                nearbyCells.Remove(curCell);
                                continue;
                            }
                        }
                        if (curCell.HasBuildingStandOn)
                        {
                            if (!finderParam.HasFlag(FinderParam.ignoreBuilding))
                            {
                                nearbyCells.Remove(curCell);
                                continue;
                            }
                        }
                        else if (!curCell.data.canStandOn)
                        {
                            nearbyCells.Remove(curCell);
                            continue;
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
                }
                path.Add(predictCell);
            }

            path.Reverse();

            if (finderParam.HasFlag(FinderParam.containOrgin))
            {
                path = new List<CellEntity> { startCell }.Union(path).ToList();
            }
            else
            {
                path.Remove(startCell);
            }


            return path;
        }
    }
}
