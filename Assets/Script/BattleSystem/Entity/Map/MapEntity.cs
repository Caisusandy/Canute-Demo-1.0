using Canute.BattleSystem.Buildings;
using Canute.BattleSystem.Develop;
using Canute.BattleSystem.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Canute.BattleSystem
{
    public class MapEntity : DecorativeEntity, IEnumerable<ColumnEntity>, IEnumerable<CellEntity>
    {
        private static MapEntity instance;


        public List<ColumnEntity> columnEntities;
        public List<FakeCell> fakeCells;
        public bool wasOnDrag;

        [Tooltip("is map initialized")] public bool isInitialized;
        [Tooltip("is map random generated")] public bool isRandomMap;
        [Tooltip("is map has only one terrain")] public bool isSingleTerrainMap;
        [Tooltip("is map drew already")] public bool isDrew;
        [Tooltip("is map round? end->origin")] public bool isRound;
        [Tooltip("seed of the random map [also the fake cells]")] public int seed;

        static float maxX;
        static float maxY;
        static float minX;
        static float minY;

        public static MapEntity CurrentMap => instance;
        public static bool WasOnDrag { get => instance.wasOnDrag; set { instance.wasOnDrag = value; SetCellCollider(!value); } }

        public Map data => new Map(this);
        public override EntityData Data => data;
        public int Count => columnEntities.Count;
        public int CellCount => GetCellCount();
        public float MapRadius => ((Vector2)Origin.transform.position - (Vector2)Center.transform.position).magnitude * ((Center.x - 1) / Center.x);
        public CellEntity Origin => this[0][0];
        public CellEntity Center { get => GetCenter(); }
        public CellEntity End { get => columnEntities.Last().Last(); }

        public ColumnEntity this[int index] => columnEntities[index];
        public CellEntity this[Vector2Int pos] => GetCell(pos);
        public CellEntity this[int x, int y, int z] => columnEntities[y][x + y / 2];
        public CellEntity this[Vector3Int pos] => this[pos.x, pos.y, pos.z];
        public Vector2Int Size => new Vector2Int(this[0].cellEntities.Count, Count);

        #region Map setup
        public override void Awake()
        {
            name = "Map";
            instance = this;
            transform.localScale = Vector3.one;

            if (!isInitialized) BattleMapSetUp();
            if (!(Game.CurrentBattle is null)) Game.CurrentBattle.MapEntity = this;
            if (columnEntities.Count > 0) FixColumnPosition();
        }

        private void FixColumnPosition()
        {
            for (int i = 0; i < columnEntities.Count; i++)
            {
                ColumnEntity item = columnEntities[i];
                var pos = item.transform.localPosition;
                pos.y = i * CellSize.y;
                item.transform.localPosition = pos;
                for (int j = 0; j < item.cellEntities.Count; j++)
                {
                    var cell = item.cellEntities[j];
                    pos = cell.transform.localPosition;
                    pos.x = j * CellSize.x;
                    cell.transform.localPosition = pos;
                }
            }
        }

        public override void Start()
        {
            base.Start();

            SetPlayerCameraToPosition();
            CellSetupAndColor();

            maxX = End.transform.position.x;
            maxY = End.transform.position.y;
            minX = Origin.transform.position.x;
            minY = Origin.transform.position.y;
        }

        private void SetPlayerCameraToPosition()
        {
            CampusEntity campusEntity = CampusEntity.GetCampus(Game.CurrentBattle.Player);
            if (campusEntity)
            {
                Vector3 position = campusEntity.transform.position;
                position.z = 0;
                transform.position -= position;
            }
            else if (Game.CurrentBattle.Player.BattleArmies.Select((b) => b.Entity).Where((e) => e).Count() > 0)
            {
                Vector3 position = Game.CurrentBattle.Player.BattleArmies[0].Entity.transform.position;
                position.z = 0;
                transform.position -= position;
            }
            else
            {
                Vector3 position = Center.transform.position;
                position.z = 0;
                transform.position -= position;
            }
        }

        private void CellSetupAndColor()
        {
            StartCoroutine(SetUpEnumerator());
            IEnumerator SetUpEnumerator()
            {
                yield return BattleUI.ShowRotator(true);
                yield return FakeMapGenerator.instance.CreateFakeCells();
                yield return FakeCellSetUp();

                CellColoration cellColoration = new CellColoration(this);
                yield return cellColoration.Color();
                yield return BattleUI.ShowRotator(false);
                BattleUI.FadeInBattle();
            }
        }


        private IEnumerator FakeCellSetUp()
        {
            fakeCells = FakeMapGenerator.instance.fakeCells;
            FakeMapGenerator.instance.fakeColumn.transform.position = Origin.transform.position;

            Vector3 localPosition = FakeMapGenerator.instance.fakeColumn.transform.localPosition; localPosition.z = 0;
            FakeMapGenerator.instance.fakeColumn.transform.localPosition = localPosition;

            if (!isSingleTerrainMap)
            {
                var rmg = new RandomTerrainGenerator(this, seed);
                rmg.RandomizeFakeCellTerrain();
            }
            else
            {
                foreach (var item in fakeCells)
                {
                    item.terrain = Origin.data.terrain;
                }
            }
            yield return null;
        }

        [ContextMenu("Initialize")]
        public void BattleMapSetUp()
        {
            instance = this;
            foreach (Transform columnTransform in transform)
            {
                ColumnEntity item = columnTransform.GetComponent<ColumnEntity>();
                columnEntities.Add(item);
            }
            foreach (Transform columnTransform in transform)
            {
                ColumnEntity item = columnTransform.GetComponent<ColumnEntity>();
                item.ColumnSetup();
            }
            if (isRandomMap)
            {
                var rtg = new RandomTerrainGenerator(this, seed);
                rtg.RandomizeTerrain();
            }
            isInitialized = true;
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
            //Debug.Log(hexX + "," + hexY);
            //Debug.Log((hexX + hexY / 2) + "," + hexY);
            return GetCell(hexX + hexY / 2, hexY);
        }

        #endregion

        private int GetCellCount()
        {
            int count = 0;
            foreach (var item in columnEntities)
            {
                count += item.cellEntities.Count;
            }
            return count;
        }

        /// <summary>
        /// return the center of the map, whether the cell is reachable for the player or not.
        /// </summary>
        /// <returns></returns>
        private CellEntity GetCenter()
        {
            int y = columnEntities.Count / 2;
            int x = columnEntities[y].cellEntities.Count / 2;
            CellEntity cellEntity = this[y][x];
            return cellEntity;
        }


        public static implicit operator List<ColumnEntity>(MapEntity mapEntity)
        {
            return mapEntity.columnEntities;
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



        public CellEntity GetCell(int x, int y)
        {
            //Debug.Log(x + "," + y);
            //if (y < 0 || x < 0)
            //{
            //    return null;
            //}
            //if (columnEntities.Count > y)
            //{
            //    if (this[y].cellEntities.Count > x)
            //    {
            try
            {
                CellEntity cellEntity = this[y][x];
                if (cellEntity.data.hide)
                {
                    return null;
                }

                return cellEntity;
            }
            catch
            {
                if (isRound)
                {
                    if (x < 0) x += Size.x;
                    if (x >= Size.x) x -= Size.x;
                    if (y < 0) y += Size.y;
                    if (y >= Size.y) y -= Size.y;
                    return GetCell(x, y);
                }
            }
            //    }
            //}
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


        #region Nearby
        public List<CellEntity> GetNearbyCell(CellEntity centerCell)
        {
            int hexX = centerCell.data.HexCoord.x;
            int hexY = centerCell.data.HexCoord.y;
            List<CellEntity> Nearby = new List<CellEntity>();

            CellEntity yi = GetCellEntityByHex(hexX, hexY + 1); //RU
            if (yi)
                Nearby.Add(yi);   //Y+1 


            CellEntity xi = GetCellEntityByHex(hexX + 1, hexY); //R
            if (xi)
            {
                Nearby.Add(xi);   //X+1
            }


            CellEntity zi = GetCellEntityByHex(hexX + 1, hexY - 1); //RD
            if (zi)
            {
                Nearby.Add(zi);//z+1
            }


            CellEntity iy = GetCellEntityByHex(hexX, hexY - 1); //LD
            if (iy)
            {
                Nearby.Add(iy);   //Y-1
            }


            CellEntity ix = GetCellEntityByHex(hexX - 1, hexY); //L
            if (ix)
            {
                Nearby.Add(ix);   //X-1
            }


            CellEntity iz = GetCellEntityByHex(hexX - 1, hexY + 1); //LU
            if (iz)
            {
                Nearby.Add(iz);//Z
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
        public List<CellEntity> GetAttackArea(CellEntity centerCell, int range, BattleEntityData battleArmy)
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
                    List<CellEntity> nearbyCellsWithArmy = GetNearbyCell(cellEntity).Where((cell) => cell.data.canStandOn).ToList();//获取临近格子
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
        public List<CellEntity> GetMoveArea(CellEntity centerCell, int range, BattleEntityData battleArmy)
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
                    List<CellEntity> nearbyCellsWithArmy = GetNearbyCell(cellEntity).Where((cell) => cell.data.canStandOn).ToList();//获取临近格子
                    nearbyCellsWithArmy = nearbyCellsWithArmy.Except(allCells).ToList();

                    foreach (CellEntity cell in nearbyCellsWithArmy)
                    {
                        if (!cell.HasArmyStandOn)
                        {
                            nearbyCells.Add(cell);
                        }
                        //else
                        //{
                        //    bool isvalid = false;
                        //    if (cell.HasArmyStandOn)
                        //    {
                        //        isvalid = cell.HasArmyStandOn.data.StandPosition != battleArmy.StandPosition;
                        //    }
                        //    if (isvalid)
                        //    {
                        //        nearbyCells.Add(cell);
                        //    }
                        //}
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



        public List<CellEntity> GetMapBorderCell()
        {
            return ((IEnumerable<CellEntity>)this).Where((c) => c.x == 0 || c.y == 0 || c.x == Size.x - 1 || c.y == Size.y - 1).ToList();
        }

        #endregion

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
                d = direction.x - origin.x;
                forward = d > 0;
                for (int i = 0; i < Mathf.Abs(d); i++)
                {
                    ColumnEntity columnEntity = this[origin.y];
                    int index = origin.x + (forward ? 1 : -1) * i;
                    if (index >= 0 && index < columnEntity.cellEntities.Count)
                    {
                        CellEntity item = columnEntity.cellEntities[index];
                        cellEntities.Add(item);
                    }
                }

                //foreach (var item in this[origin.y])
                //{
                //    if (item.x > origin.x && forward)
                //    {
                //        cellEntities.Add(item);
                //    }
                //    else if (item.x < origin.x && !forward)
                //    {
                //        cellEntities.Add(item);
                //    }
                //}
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

        public List<CellEntity> DrawBorder(List<CellEntity> inner, List<CellEntity> outer)
        {
            List<CellEntity> border = outer.Except(inner).ToList();
            foreach (var item in inner)
            {
                foreach (var cellEntity in item.NearByCells)
                {
                    if (cellEntity.NearByCells.Concat(border).ToList().Count <= 2 && !IsBorderCell(cellEntity))
                    {
                        border.Add(cellEntity);
                    }
                }
            }
            return border;
        }

        public List<CellEntity> GetRectArea(Vector2Int p1, Vector2Int p2)
        {
            if (p1.x > p2.x || p1.y > p2.y)
            {
                return new List<CellEntity>();
            }

            var list = new List<CellEntity>();
            for (int i = p1.y; i <= p2.y; i++)
            {
                for (int j = p1.x; j <= p2.x; j++)
                {
                    list.Add(GetCell(j, i));
                }
            }

            return list;
        }

        public Vector2Int GetPosition(CellEntity cellEntity)
        {
            return new Vector2Int(columnEntities[InColumnOf(cellEntity)].cellEntities.IndexOf(cellEntity), InColumnOf(cellEntity));
        }

        public bool IsBorderCell(CellEntity cellEntity)
        {
            if (!GetCell(cellEntity.Coordinate))
            {
                return false;
            }
            if (cellEntity.Coordinate.x == 0 || cellEntity.Coordinate.y == 0)
            {
                return true;
            }
            if (cellEntity.Coordinate.x == Size.x)
            {
                return true;
            }
            if (cellEntity.Coordinate.y == Size.y)
            {
                return true;
            }
            return false;
        }



        public void OpenCells(params CellEntity[] area)
        {
            foreach (var item in area)
            {
                item.Open();
            }
        }

        public void OpenCells(IEnumerable<CellEntity> area) => OpenCells(area.ToArray());

        public void CloseCells(params CellEntity[] area)
        {
            foreach (var item in area)
            {
                item.Close();
            }
        }

        public void CloseCells(IEnumerable<CellEntity> area) => CloseCells(area.ToArray());



        public static void MoveMap()
        {
            Vector3 finalPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 delta = finalPos - BattleControl.instance.inputPos;

            if (Vector3.Magnitude(delta) == 0) return;
            if (Module.Motion.ongoingMotions.Count > 0) return;

            Vector2 newPos = Camera.main.transform.position - delta;
            float magnitude = (new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y) - newPos).magnitude;


            //Debug.Log(maxX);
            //Debug.Log(maxY);
            //Debug.Log(minX);
            //Debug.Log(minY);
            //Debug.Log(newPos);

            if (newPos.x < maxX && newPos.y < maxY && newPos.x > minX && newPos.y > minY)
            {
                Camera.main.transform.position -= delta;
            }
            else
            {
                //Vector2 oldPos = Camera.main.transform.position;
                //float oldMagnitude = (new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y) - oldPos).magnitude;
                //if (magnitude > oldMagnitude) { return; }
                //Camera.main.transform.position -= delta;
            }
            WasOnDrag = true;
            //if (magnitude > CurrentMap.MapRadius)
            //{
            //    Vector2 oldPos = CurrentMap.Center.transform.position;
            //    float oldMagnitude = (new Vector2(Camera.main.transform.position.x, Camera.main.transform.position.y) - oldPos).magnitude;
            //    if (magnitude > oldMagnitude)
            //    {
            //        return;
            //    }
            //} 
            //WasOnDrag = true;
            //Camera.main.transform.position -= delta;
        }

        public static void SetCellCollider(bool value)
        {
            foreach (var item in CurrentMap)
            {
                foreach (var cell in item)
                {
                    cell.GetComponent<Collider2D>().enabled = value;
                }
            }
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



        [ContextMenu("Draw Cell")]
        public void DrawCell()
        {
            foreach (var item in columnEntities)
            {
                foreach (var cell in item)
                {
                    cell.SetCellSprite();
                }
            }
        }


        [ContextMenu("Horizontal Flip")]
        public void HorizontalFlip()
        {
            for (int i = 0; i < columnEntities.Count; i++)
            {
                List<Vector3> pos = new List<Vector3>();
                int cellCount = columnEntities[i].cellEntities.Count;
                for (int j = 0; j < cellCount; j++)
                {
                    CellEntity cellEntity = columnEntities[i].cellEntities[j];
                    pos.Add(cellEntity.transform.position);
                }
                for (int j = cellCount - 1; j >= 0; j--)
                {
                    columnEntities[i].cellEntities[j].transform.position = pos[cellCount - j - 1];
                    columnEntities[i].cellEntities[j].transform.SetAsLastSibling();
                }
                columnEntities[i].cellEntities.Reverse();
            }
        }

        [ContextMenu("Vertical Flip")]
        public void VerticalFlip()
        {
            List<Vector3> pos = new List<Vector3>();
            int columnCount = columnEntities.Count;
            for (int j = 0; j < columnCount; j++)
            {
                var cellEntity = columnEntities[j];
                pos.Add(cellEntity.transform.position);
            }
            for (int j = columnCount - 1; j >= 0; j--)
            {
                columnEntities[j].transform.position = pos[columnCount - j - 1];
                columnEntities[j].transform.SetAsLastSibling();
            }
            columnEntities.Reverse();
        }
    }

    #region With Map

    [Serializable]
    public class Map : NonOwnableEntityData, IEnumerable<Column>, IEnumerable<Cell>
    {
        public List<Column> columns = new List<Column>();
        public bool random;
        public int seed;

        public Column this[int index] { get => columns[index]; set => columns[index] = value; }
        public Cell this[int x, int y] { get => columns[x][y]; set => columns[x][y] = value; }

        public Map(MapEntity mapEntity)
        {
            this.random = mapEntity.isRandomMap;
            this.seed = mapEntity.seed;
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
