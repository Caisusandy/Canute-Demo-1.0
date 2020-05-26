using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class MapEntity : DecorativeEntity, IEnumerable<ColumnEntity>, IEnumerable<CellEntity>
    {
        public List<ColumnEntity> columnEntities;
        public static MapEntity CurrentMap => Game.CurrentBattle.MapEntity;

        public Map data => new Map(this);
        public override EntityData Data => data;
        public int Count => columnEntities.Count;
        public int CellCount => GetCellCount();

        private int GetCellCount()
        {
            int count = 0;
            foreach (var item in columnEntities)
            {
                count += item.cellEntities.Count;
            }
            return count;
        }

        public override void Awake()
        {
            name = "Map";
            Game.CurrentBattle.MapEntity = this;
            MapSetUp();
        }

        public static implicit operator List<ColumnEntity>(MapEntity mapEntity)
        {
            return mapEntity.columnEntities;
        }

        public ColumnEntity this[int index] => columnEntities[index];
        public CellEntity this[Vector2Int pos] => this[pos.y][pos.x];
        public CellEntity this[int x, int y, int z] => columnEntities[y][x + y / 2];
        public CellEntity this[Vector3Int pos] => this[pos.x, pos.y, pos.z];


        public void MapSetUp()
        {
            foreach (ColumnEntity item in columnEntities)
            {
                item.CellSetup();
            }
        }

        public IEnumerator<ColumnEntity> GetEnumerator()
        {
            return ((IEnumerable<ColumnEntity>)columnEntities).GetEnumerator();
        }

        IEnumerator<CellEntity> IEnumerable<CellEntity>.GetEnumerator()
        {
            foreach (ColumnEntity columnEntity in columnEntities)
            {
                foreach (CellEntity item in columnEntity)
                {
                    yield return item;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<ColumnEntity>)columnEntities).GetEnumerator();
        }

        public int InColumnOf(CellEntity cellEntity)
        {
            foreach (ColumnEntity item in this)
            {
                if (item.cellEntities.Contains(cellEntity))
                {
                    return columnEntities.IndexOf(item);
                }
            }
            return -1;
        }

        public CellEntity GetCellEntityByHex(int hexX, int hexY)
        {
            return GetCell(hexX + hexY / 2, hexY);
        }

        public CellEntity GetCell(int x, int y)
        {
            if (y < 0 || x < 0)
            {
                return null;
            }
            if (columnEntities.Count > y)
            {
                if (this[y].cellEntities.Count > x)
                {
                    CellEntity cellEntity = this[y][x];

                    if (cellEntity.data.hide)
                    {
                        return null;
                    }

                    return cellEntity;
                }
            }
            return null;
        }

        public CellEntity GetCell(Vector2Int pos)
        {
            return GetCell(pos.x, pos.y);
        }

        public CellEntity GetCell(int x, int y, int z)
        {
            Vector2Int vector2 = Cell.V32V2(new Vector3Int(x, y, z));
            return GetCell(vector2);
        }

        public CellEntity GetCell(Vector3Int pos)
        {
            return GetCell(pos.x, pos.y, pos.z);
        }



        public List<CellEntity> GetNearbyCell(CellEntity centerCell)
        {
            int hexX = centerCell.data.HexCoord.x;
            int hexY = centerCell.data.HexCoord.y;
            List<CellEntity> Nearby = new List<CellEntity>();

            if (GetCellEntityByHex(hexX, hexY + 1))
            {
                Nearby.Add(GetCellEntityByHex(hexX, hexY + 1));   //Y+1
            }


            if (GetCellEntityByHex(hexX + 1, hexY))
            {
                Nearby.Add(GetCellEntityByHex(hexX + 1, hexY));   //X+1
            }


            if (GetCellEntityByHex(hexX + 1, hexY - 1))
            {
                Nearby.Add(GetCellEntityByHex(hexX + 1, hexY - 1));//z+1
            }


            if (GetCellEntityByHex(hexX, hexY - 1))
            {
                Nearby.Add(GetCellEntityByHex(hexX, hexY - 1));   //Y-1
            }


            if (GetCellEntityByHex(hexX - 1, hexY))
            {
                Nearby.Add(GetCellEntityByHex(hexX - 1, hexY));   //X-1
            }


            if (GetCellEntityByHex(hexX - 1, hexY + 1))
            {
                Nearby.Add(GetCellEntityByHex(hexX - 1, hexY + 1));//Z
            }


            return Nearby;
        }

        public List<CellEntity> GetNearbyCell(CellEntity centerCell, bool ignoreArmy)
        {
            List<CellEntity> nearbys = GetNearbyCell(centerCell);
            for (int i = nearbys.Count - 1; i >= 0; i--)
            {
                if (nearbys[i].HasArmyStandOn && !ignoreArmy)
                {
                    nearbys.RemoveAt(i);
                }
            }
            return nearbys;
        }

        /// <summary>
        /// use to draw move range only
        /// </summary>
        /// <param name="centerCell"></param>
        /// <param name="armyPosition"></param>
        /// <returns></returns>
        public List<CellEntity> GetNearbyCell(CellEntity centerCell, BattleProperty.Position armyPosition)
        {
            List<CellEntity> nearbys = GetNearbyCell(centerCell);
            for (int i = nearbys.Count - 1; i >= 0; i--)
            {
                if (nearbys[i].HasArmyStandOn)
                {
                    if (nearbys[i].HasArmyStandOn.data.Properties.StandPosition == armyPosition)
                    {
                        nearbys.RemoveAt(i);
                    }
                }

            }

            return nearbys;
        }



        public List<CellEntity> GetNearbyCell(CellEntity centerCell, int range, bool ignoreArmy = true)
        {
            List<CellEntity> currentLevel = new List<CellEntity> { centerCell };
            List<List<CellEntity>> cellLevel = new List<List<CellEntity>> { new List<CellEntity>(currentLevel) };
            List<CellEntity> allCells = new List<CellEntity>();

            for (int i = 0; i < range; i++)
            {
                List<CellEntity> nextLevel = new List<CellEntity>();    //下一层
                foreach (CellEntity cellEntity in currentLevel)             //浏览当前层
                {
                    List<CellEntity> nearbyCells = GetNearbyCell(cellEntity, ignoreArmy);//获取临近格子 

                    nearbyCells = nearbyCells.Except(allCells).ToList();  //保留没有被使用过的格子
                    nextLevel = nextLevel.Union(nearbyCells).ToList();   //给下一层加入临近格子
                    allCells = allCells.Union(nearbyCells).ToList();     //给所有格子加入临近格子 
                }
                cellLevel.Add(nextLevel);   //下一层加入到层级里
                currentLevel = nextLevel;   //现在层转为下一层 
            }

            return allCells;
        }

        /// <summary>
        /// use to draw army attack range
        /// </summary>
        /// <param name="centerCell"></param>
        /// <param name="range"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<CellEntity> GetNearbyCell(CellEntity centerCell, int range, BattleProperty.AttackType type)
        {
            List<CellEntity> currentLevel = new List<CellEntity> { centerCell };
            List<List<CellEntity>> cellLevel = new List<List<CellEntity>> { new List<CellEntity>(currentLevel) };
            List<CellEntity> allCells = new List<CellEntity>();

            for (int i = 0; i < range; i++)
            {
                List<CellEntity> nextLevel = new List<CellEntity>();    //下一层
                foreach (CellEntity cellEntity in currentLevel)             //浏览当前层
                {
                    List<CellEntity> nearbyCells = GetNearbyCell(cellEntity, type.IsTypeOf(BattleProperty.AttackType.projectile));//获取临近格子 
                    List<CellEntity> nearbyCellsWithArmy = GetNearbyCell(cellEntity);//获取临近格子

                    nearbyCells = nearbyCells.Except(allCells).ToList();  //保留没有被使用过的格子
                    nearbyCellsWithArmy = nearbyCellsWithArmy.Except(allCells).ToList();  //保留没有被使用过的格子
                    nextLevel = nextLevel.Union(nearbyCells).ToList();   //给下一层加入临近格子
                    allCells = allCells.Union(nearbyCellsWithArmy).ToList();     //给所有格子加入临近格子 
                }
                cellLevel.Add(nextLevel);   //下一层加入到层级里
                currentLevel = nextLevel;   //现在层转为下一层 
            }

            return allCells;
        }

        /// <summary>
        /// use to draw army attack range
        /// </summary>
        /// <param name="centerCell"></param>
        /// <param name="range"></param> 
        /// <returns></returns>
        public List<CellEntity> GetAttackArea(CellEntity centerCell, int range, BattleableEntityData battleArmy)
        {
            List<CellEntity> currentLevel = new List<CellEntity> { centerCell };
            List<List<CellEntity>> cellLevel = new List<List<CellEntity>> { new List<CellEntity>(currentLevel) };
            List<CellEntity> allCells = new List<CellEntity>();

            for (int i = 0; i < range; i++)
            {
                List<CellEntity> nextLevel = new List<CellEntity>();    //下一层
                foreach (CellEntity cellEntity in currentLevel)             //浏览当前层
                {
                    List<CellEntity> nearbyCells = new List<CellEntity>();
                    List<CellEntity> nearbyCellsWithArmy = GetNearbyCell(cellEntity);//获取临近格子
                    nearbyCellsWithArmy = nearbyCellsWithArmy.Except(allCells).ToList();

                    foreach (CellEntity cell in nearbyCellsWithArmy)
                    {
                        if (!cell.HasArmyStandOn || battleArmy.Properties.Attack.IsTypeOf(BattleProperty.AttackType.projectile))
                        {
                            nearbyCells.Add(cell);
                        }
                        else
                        {
                            bool isvalid = false;
                            if (cell.HasArmyStandOn)
                            {
                                isvalid = cell.HasArmyStandOn.data.StandPosition != battleArmy.StandPosition;
                            }
                            if (isvalid)
                            {
                                nearbyCells.Add(cell);
                            }
                        }
                    }

                    nearbyCells = nearbyCells.Except(allCells).ToList();  //保留没有被使用过的格子
                    nextLevel = nextLevel.Union(nearbyCells).ToList();   //给下一层加入临近格子
                    allCells = allCells.Union(nearbyCellsWithArmy).ToList();     //给所有格子加入临近格子 
                }
                cellLevel.Add(nextLevel);   //下一层加入到层级里
                currentLevel = nextLevel;   //现在层转为下一层 
            }

            return allCells;
        }

        /// <summary>
        /// use to draw army attack range
        /// </summary>
        /// <param name="centerCell"></param>
        /// <param name="range"></param> 
        /// <returns></returns>
        public List<CellEntity> GetMoveArea(CellEntity centerCell, int range, BattleableEntityData battleArmy)
        {
            List<CellEntity> currentLevel = new List<CellEntity> { centerCell };
            List<List<CellEntity>> cellLevel = new List<List<CellEntity>> { new List<CellEntity>(currentLevel) };
            List<CellEntity> allCells = new List<CellEntity>();

            for (int i = 0; i < range; i++)
            {
                List<CellEntity> nextLevel = new List<CellEntity>();    //下一层
                foreach (CellEntity cellEntity in currentLevel)             //浏览当前层
                {
                    List<CellEntity> nearbyCells = new List<CellEntity>();
                    List<CellEntity> nearbyCellsWithArmy = GetNearbyCell(cellEntity);//获取临近格子
                    nearbyCellsWithArmy = nearbyCellsWithArmy.Except(allCells).ToList();

                    foreach (CellEntity cell in nearbyCellsWithArmy)
                    {
                        if (!cell.HasArmyStandOn)
                        {
                            nearbyCells.Add(cell);
                        }
                        else
                        {
                            bool isvalid = false;
                            if (cell.HasArmyStandOn)
                            {
                                isvalid = cell.HasArmyStandOn.data.StandPosition != battleArmy.StandPosition;
                            }
                            if (isvalid)
                            {
                                nearbyCells.Add(cell);
                            }
                        }
                    }

                    nearbyCells = nearbyCells.Except(allCells).ToList();  //保留没有被使用过的格子
                    nextLevel = nextLevel.Union(nearbyCells).ToList();   //给下一层加入临近格子
                    allCells = allCells.Union(nearbyCellsWithArmy).ToList();     //给所有格子加入临近格子 
                }
                cellLevel.Add(nextLevel);   //下一层加入到层级里
                currentLevel = nextLevel;   //现在层转为下一层 
            }

            return allCells;
        }

        public List<CellEntity> GetNearbyCell(CellEntity centerCell, int range, BattleProperty.Position notIgnorePositions)
        {
            List<CellEntity> currentLevel = new List<CellEntity> { centerCell };
            List<List<CellEntity>> cellLevel = new List<List<CellEntity>> { new List<CellEntity>(currentLevel) };
            List<CellEntity> allCells = new List<CellEntity>();

            for (int i = 0; i < range; i++)
            {
                List<CellEntity> nextLevel = new List<CellEntity>();    //下一层
                foreach (CellEntity cellEntity in currentLevel)             //浏览当前层
                {
                    List<CellEntity> nearbyCells = GetNearbyCell(cellEntity, notIgnorePositions);//获取临近格子 

                    nearbyCells = nearbyCells.Except(allCells).ToList();  //保留没有被使用过的格子
                    nextLevel = nextLevel.Union(nearbyCells).ToList();   //给下一层加入临近格子
                    allCells = allCells.Union(nearbyCells).ToList();     //给所有格子加入临近格子 
                }
                cellLevel.Add(nextLevel);   //下一层加入到层级里
                currentLevel = nextLevel;   //现在层转为下一层 
            }

            return allCells;
        }

        public List<CellEntity> GetBorderCell(CellEntity centerCell, int range, bool ignoreArmy = true)
        {
            List<CellEntity> inner = GetNearbyCell(centerCell, range - 1, ignoreArmy);
            List<CellEntity> outer = GetNearbyCell(centerCell, range, ignoreArmy);
            return DrawBorder(inner, outer);
        }

        public List<CellEntity> GetBorderCell(CellEntity centerCell, int range, BattleProperty.AttackType type)
        {
            List<CellEntity> inner = GetNearbyCell(centerCell, range - 1, type);
            List<CellEntity> outer = GetNearbyCell(centerCell, range, type);
            return DrawBorder(inner, outer);
        }

        public List<CellEntity> GetBorderCell(CellEntity centerCell, int range, BattleProperty.Position notIgnorePositions)
        {
            List<CellEntity> inner = GetNearbyCell(centerCell, range - 1, notIgnorePositions);
            List<CellEntity> outer = GetNearbyCell(centerCell, range, notIgnorePositions);
            return DrawBorder(inner, outer);
        }

        public List<CellEntity> GetLine(CellEntity origin, CellEntity direction)
        {
            if (!origin || !direction)
            {
                Debug.Log("Something doesn't exist");
                return new List<CellEntity>();
            }

            int d;
            List<CellEntity> cellEntities = new List<CellEntity>();
            CellEntity leftBound;
            CellEntity rightBound;

            if (origin.y == direction.y)
            {
                Debug.Log("Same Y");
                d = direction.y - origin.y;
                leftBound = d > 0 ? origin : direction;
                rightBound = d < 0 ? origin : direction;
                foreach (var item in this[origin.y])
                {
                    if (item.y > leftBound.y && item.y < rightBound.y)
                    {
                        cellEntities.Add(item);
                    }
                }
            }
            else if (origin.HexCoord.x == direction.HexCoord.x)
            {
                Debug.Log("Same X");
                d = direction.HexCoord.x - origin.HexCoord.x;
                leftBound = d > 0 ? origin : direction;
                rightBound = d < 0 ? origin : direction;
                cellEntities.AddRange(from column in columnEntities
                                      from item in column
                                      where item.HexCoord.x > leftBound.HexCoord.x && item.HexCoord.x < rightBound.HexCoord.x
                                      select item);
            }
            else if (origin.HexCoord.z == direction.HexCoord.z)
            {
                Debug.Log("Same Z");
                d = direction.HexCoord.z - origin.HexCoord.z;
                leftBound = d > 0 ? origin : direction;
                rightBound = d < 0 ? origin : direction;
                cellEntities.AddRange(from column in columnEntities
                                      from item in column
                                      where item.HexCoord.z > leftBound.HexCoord.z && item.HexCoord.z < rightBound.HexCoord.z
                                      select item);
            }

            return cellEntities;
        }

        public List<CellEntity> GetRay(CellEntity origin, CellEntity direction)
        {
            if (!origin || !direction)
            {
                Debug.Log("Something doesn't exist");
                return new List<CellEntity>();
            }

            int d;
            bool forward;
            List<CellEntity> cellEntities = new List<CellEntity>();

            if (origin.y == direction.y)
            {
                Debug.Log("Same Y");
                d = direction.y - origin.y;
                forward = d > 0;
                foreach (var item in this[origin.y])
                {
                    if (item.x > origin.x && forward)
                    {
                        cellEntities.Add(item);
                    }
                    else if (item.x < origin.x && !forward)
                    {
                        cellEntities.Add(item);
                    }
                }
            }
            else if (origin.HexCoord.x == direction.HexCoord.x || (origin.HexCoord.z == direction.HexCoord.z))
            {
                Debug.Log("Same X/Z");
                d = direction.HexCoord.y - origin.HexCoord.y;
                forward = d > 0;
                foreach (var column in this)
                {
                    foreach (var item in column)
                    {
                        if (item.HexCoord.x != origin.HexCoord.x)
                        {
                            continue;
                        }
                        if (item.HexCoord.y > origin.HexCoord.y && forward)
                        {
                            cellEntities.Add(item);
                        }
                        else if (item.HexCoord.y < origin.HexCoord.y && !forward)
                        {
                            cellEntities.Add(item);
                        }
                    }
                }

            }
            else if (origin.HexCoord.z == direction.HexCoord.z)
            {
                Debug.Log("Same X/Z");
                d = direction.HexCoord.y - origin.HexCoord.y;
                forward = d > 0;
                foreach (var column in this)
                {
                    foreach (var item in column)
                    {
                        if (item.HexCoord.z != origin.HexCoord.z)
                        {
                            continue;
                        }
                        if (item.HexCoord.y > origin.HexCoord.y && forward)
                        {
                            cellEntities.Add(item);
                        }
                        else if (item.HexCoord.y < origin.HexCoord.y && !forward)
                        {
                            cellEntities.Add(item);
                        }
                    }
                }

            }

            return cellEntities;
        }

        protected List<CellEntity> DrawBorder(List<CellEntity> inner, List<CellEntity> outer)
        {
            List<CellEntity> border = outer.Except(inner).ToList();
            foreach (var item in inner)
            {
                foreach (var cellEntity in item.NearByCells)
                {
                    if (cellEntity.NearByCells.Concat(border).ToList().Count <= 2)
                    {
                        border.Add(cellEntity);
                    }
                }
            }
            return border;
        }

        public List<ArmyEntity> FindArmies(List<CellEntity> cellEntities)
        {
            List<ArmyEntity> armyEntities = new List<ArmyEntity>();
            foreach (var item in ArmyEntity.onMap)
            {
                if (cellEntities.Contains(item.OnCellOf))
                {
                    armyEntities.Add(item);
                }
            }
            return armyEntities;
        }

        public Vector2Int GetPosition(CellEntity cellEntity)
        {
            return new Vector2Int(columnEntities[InColumnOf(cellEntity)].cellEntities.IndexOf(cellEntity), InColumnOf(cellEntity));
        }


        public static List<CellEntity> GetBorderCell(List<CellEntity> cellEntities)
        {
            List<CellEntity> cells = cellEntities.ShallowClone();
            foreach (var item in cellEntities)
            {
                int count = 0;
                foreach (var neighbor in item.NearByCells)
                {
                    if (cellEntities.Contains(neighbor))
                        count++;
                }
                if (count == 6)
                {
                    cells.Remove(item);
                }
            }
            return cells;
        }

    }

    #region With Map

    [Serializable]
    public class Map : NonOwnableEntityData, IEnumerable<Column>, IEnumerable<Cell>
    {

        public List<Column> columns = new List<Column>();
        public Column this[int index] { get => columns[index]; set => columns[index] = value; }
        public Cell this[int x, int y] { get => columns[x][y]; set => columns[x][y] = value; }

        public Map(MapEntity mapEntity)
        {
            foreach (ColumnEntity columnEntity in mapEntity)
            {
                columns.Add(new Column(columnEntity));

            }
        }

        public IEnumerator<Column> GetEnumerator()
        {
            return ((IEnumerable<Column>)columns).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<Column>)columns).GetEnumerator();
        }

        IEnumerator<Cell> IEnumerable<Cell>.GetEnumerator()
        {
            foreach (Column column in columns)
            {
                foreach (Cell cell in column)
                {
                    yield return cell;
                }
            }
        }
    }

    #endregion

}
