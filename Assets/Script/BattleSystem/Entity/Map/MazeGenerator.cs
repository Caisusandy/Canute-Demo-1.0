using Canute.BattleSystem.Buildings;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.BattleSystem
{
    public enum MapDirection
    {
        RU,
        R,
        RD,
        LD,
        L,
        LU
    }

    public class MazeGenerator : MonoBehaviour
    {

        MapEntity MapEntity => Game.CurrentBattle.MapEntity;

        public float singleWallConnectionPossibility = 0.96f;
        public int density;
        public Vector2Int center;
        public List<StartPointPair> startPointPair;

        public void Awake()
        {
            StartCoroutine("GenerateMaze");
        }

        public IEnumerator GenerateMaze()
        {
            while (!Game.CurrentBattle.MapEntity)
            {
                yield return new WaitForFixedUpdate();
            }
            Generate();
            yield return null;
        }
        public void Reopen()
        {
            MapEntity.OpenCells(Game.CurrentBattle.MapEntity);
        }


        public void Generate()
        {
            CellEntity current;
            List<CellEntity> centerRange = MapEntity.GetNearbyCell(MapEntity.Center, 1);
            List<CellEntity> outer = new List<CellEntity>();
            List<CellEntity> path = centerRange;

            List<MapDirection> unavailableDirection = new List<MapDirection>();
            List<MapDirection> list = ((MapDirection[])Enum.GetValues(typeof(MapDirection))).Except(unavailableDirection).ToList();

            for (int i = 0; i < 6; i++)
            {
                outer.Add(centerRange[i].GetByDirection(list[i]));
            }

            foreach (var item in outer)
            {
                current = item;
                path.Add(current);
                while (true)
                {
                    List<CellEntity> nearbyCell = current.NearByCells;
                    var usefulNearbyCells = nearbyCell.Except(path).Except(outer).Where((c) => c.NearByCells.Except(path).Count() >= 4 - (6 - current.NearByCells.Count)).ToList();
                    if (usefulNearbyCells.Count == 0) { break; }
                    CellEntity cellEntity = usefulNearbyCells[UnityEngine.Random.Range(0, usefulNearbyCells.Count)];
                    path.Add(cellEntity);
                    current = cellEntity;
                }
            }
            path.AddRange(MapEntity.GetMapBorderCell());
            MapEntity.CloseCells(MapEntity);
            MapEntity.OpenCells(path);

        }

        //public void Generate()
        //{
        //    Reopen();
        //    var border = new List<CellEntity>();

        //    //Vector2Int[] vector2Ints = { new Vector2Int(1, 1), new Vector2Int(1, MapEntity.Size.y - 2), new Vector2Int(MapEntity.Size.x - 2, 1), new Vector2Int(MapEntity.Size.x - 2, MapEntity.Size.y - 2) };
        //    //MapDirection[] posDir = { MapDirection.RU, MapDirection.RD, MapDirection.LU, MapDirection.LD };

        //    for (int j = 0; j < 4; j++)
        //    {
        //        int v = j;// UnityEngine.Random.Range(0, vector2Ints.Length);
        //        CellEntity start = startPointPair[v].CellEntity;
        //        CellEntity lastPoint = start;
        //        List<CellEntity> ends = new List<CellEntity>();

        //        MapDirection lastDirection = startPointPair[v].direction;
        //        List<MapDirection> unavailableDirection = new List<MapDirection>();
        //        border.Add(start);

        //        while (true)
        //        {
        //            IEnumerable<CellEntity> cells = lastPoint.NearByCells
        //                .Except(border)
        //                .Except(ends)
        //                .Where((c) => c.Exist()?.data.canStandOn == true)
        //                .Where((c1) => c1.NearByCells.Except(border).Count() > 3)
        //                .Where((c3) => c3.NearByCells
        //                    .Where((c3N) => c3N.NearByCells.Except(border).Count() > 3).Count() > 0)
        //                .Where((c4) => !MapEntity.IsBorderCell(c4));
        //            Debug.Log(cells.Count());
        //            Debug.Log(unavailableDirection.Count);

        //            if (cells.Count() == 0) { Debug.Log("No available cell"); break; }
        //            if (unavailableDirection.Count > 5) { Debug.Log("No available direction"); break; }

        //            List<MapDirection> list = ((MapDirection[])Enum.GetValues(typeof(MapDirection))).Except(unavailableDirection).ToList();
        //            if (list.Contains(lastDirection))
        //            {
        //                for (int i = 0; i < 6; i++) list.Add(lastDirection);
        //            }

        //            MapDirection direction = list[UnityEngine.Random.Range(0, list.Count)];
        //            Debug.Log(direction);

        //            CellEntity waitingCell = lastPoint.GetDirection(direction);

        //            Debug.Log(lastPoint.HexCoord);
        //            if (!waitingCell)
        //            {
        //                Debug.Log("cell not exist");
        //                if (!unavailableDirection.Contains(direction)) unavailableDirection.Add(direction);
        //                continue;
        //            }

        //            if (MapEntity.IsBorderCell(waitingCell))
        //            {
        //                Debug.Log("cell is a border");
        //                if (!unavailableDirection.Contains(direction)) unavailableDirection.Add(direction);
        //                continue;
        //            }

        //            if (ends.Contains(waitingCell))
        //            {
        //                Debug.Log("one of the ends");
        //                if (!unavailableDirection.Contains(direction)) unavailableDirection.Add(direction);
        //                continue;
        //            }

        //            if (!lastPoint.NearByCells.Contains(waitingCell))
        //            {
        //                Debug.Log("Not a near by cell. " + direction);
        //            }

        //            //point repeated
        //            if (border.Contains(waitingCell))
        //            {
        //                Debug.Log("repeat");
        //                if (!unavailableDirection.Contains(direction)) unavailableDirection.Add(direction);
        //                continue;
        //            }

        //            Debug.Log(waitingCell.HexCoord);

        //            //go forward 
        //            {
        //                border.Add(waitingCell);
        //                lastPoint = waitingCell;
        //                lastDirection = direction;
        //                unavailableDirection.Clear();
        //            }

        //            //back to turning point
        //            if (waitingCell.NearByCells.Where((c) => c.Exist()?.data.canStandOn == true).Except(border).Count() < 4)
        //            //|| waitingCell.NearByCells.Where((c3N) => c3N.NearByCells.Except(border).Count() > 2).Count() == 0)
        //            {
        //                Debug.Log("Back to last point");
        //                ends.Add(lastPoint);

        //                if (border.IndexOf(lastPoint) - 1 <= 0) { Debug.Log("No turning back"); break; }
        //                else lastPoint = border[border.IndexOf(lastPoint) - 1];
        //                unavailableDirection.Clear();
        //                continue;
        //            }
        //            Debug.Log(border.Count);

        //        }
        //    }
        //    MapEntity.CloseCells(border);
        //}
        //public void Generate()
        //{
        //    Reopen();
        //    var border = new List<CellEntity>();

        //    for (int j = 0; j < 4; j++)
        //    {

        //        Vector2Int[] vector2Ints = { new Vector2Int(1, 1), new Vector2Int(1, MapEntity.Size.y - 2), new Vector2Int(MapEntity.Size.x - 2, 1), new Vector2Int(MapEntity.Size.x - 2, MapEntity.Size.y - 2) };
        //        Direction[] posDir = { Direction.RU, Direction.LD, Direction.LU, Direction.RD };

        //        int v = j;// UnityEngine.Random.Range(0, vector2Ints.Length);
        //        CellEntity start = MapEntity[vector2Ints[v]];
        //        CellEntity lastTurningPoint = start;
        //        CellEntity lastPoint = start;

        //        List<CellEntity> ends = new List<CellEntity>();
        //        List<CellEntity> turningPoints = new List<CellEntity>();

        //        Direction lastDirection = posDir[v];
        //        List<Direction> unavailableDirection = new List<Direction>();

        //        while (true)
        //        {
        //            IEnumerable<CellEntity> cells = lastPoint.NearByCells
        //                .Except(border)
        //                .Except(ends)
        //                .Where((c) => c.Exist()?.data.canStandOn == true)
        //                .Where((c1) => c1.NearByCells.Except(border).Count() > 3)
        //                .Where((c3) => c3.NearByCells
        //                    .Where((c3N) => c3N.NearByCells.Except(border).Count() > 3).Count() > 0)
        //                .Where((c4) => !MapEntity.IsBorderCell(c4));
        //            Debug.Log(cells.Count());
        //            Debug.Log(unavailableDirection.Count);

        //            if (cells.Count() == 0) { Debug.Log("No available cell"); break; }
        //            if (unavailableDirection.Count > 5) { Debug.Log("No available direction"); break; }

        //            List<Direction> list = ((Direction[])Enum.GetValues(typeof(Direction))).Except(unavailableDirection).ToList();
        //            if (list.Contains(lastDirection))
        //            {
        //                for (int i = 0; i < 6; i++) list.Add(lastDirection);
        //            }

        //            Direction direction = list[UnityEngine.Random.Range(0, list.Count)];
        //            Debug.Log(direction);

        //            CellEntity waitingCell;
        //            switch (direction)
        //            {
        //                case Direction.RU:
        //                    waitingCell = lastPoint.RightUp;
        //                    break;
        //                case Direction.R:
        //                    waitingCell = lastPoint.Right;
        //                    break;
        //                case Direction.RD:
        //                    waitingCell = lastPoint.RightDown;
        //                    break;
        //                case Direction.LD:
        //                    waitingCell = lastPoint.LeftDown;
        //                    break;
        //                case Direction.L:
        //                    waitingCell = lastPoint.Left;
        //                    break;
        //                case Direction.LU:
        //                    waitingCell = lastPoint.LeftUp;
        //                    break;
        //                default:
        //                    if (lastPoint.LeftUp)
        //                        waitingCell = lastPoint.LeftUp;
        //                    continue;
        //            }

        //            if (!waitingCell)
        //            {
        //                if (!unavailableDirection.Contains(direction)) unavailableDirection.Add(direction);
        //                continue;
        //            }

        //            if (MapEntity.IsBorderCell(waitingCell))
        //            {
        //                if (!unavailableDirection.Contains(direction)) unavailableDirection.Add(direction);
        //                continue;
        //            }

        //            //point repeated
        //            if (border.Contains(waitingCell))
        //            {
        //                Debug.Log("repeated point");
        //                if (!unavailableDirection.Contains(direction)) unavailableDirection.Add(direction);
        //                continue;
        //            }

        //            //back to turning point
        //            if (waitingCell.NearByCells.Where((c) => c.Exist()?.data.canStandOn == true).Except(border).Count() < 4)
        //            //|| waitingCell.NearByCells.Where((c3N) => c3N.NearByCells.Except(border).Count() > 2).Count() == 0)
        //            {
        //                Debug.Log("Back to last turning point");
        //                ends.Add(lastPoint);
        //                if (lastPoint == lastTurningPoint)
        //                {
        //                    if (turningPoints.IndexOf(lastTurningPoint) - 1 <= 0) { Debug.Log("No turning back"); break; }
        //                    else lastPoint = turningPoints[turningPoints.IndexOf(lastTurningPoint) - 1];
        //                }
        //                else { lastPoint = lastTurningPoint; }

        //                unavailableDirection.Clear();
        //                continue;
        //            }
        //            //go forward
        //            else
        //            {
        //                border.Add(lastPoint);
        //                lastPoint = waitingCell;
        //                lastDirection = direction;
        //                unavailableDirection.Clear();
        //            }
        //            //change Turning point
        //            if (direction != lastDirection)
        //            {
        //                Debug.Log("road turned, change turning point and add more turning Points");
        //                lastTurningPoint = waitingCell;
        //                if (!turningPoints.Contains(waitingCell)) turningPoints.Add(waitingCell);
        //            }
        //            Debug.Log(border.Count);

        //        }
        //    }
        //    MapEntity.CloseCells(border);
        //}
        //public void Generate()
        //{
        //    CellEntity origin = MapEntity.Origin;
        //    CellEntity endPoint = MapEntity.GetCell(MapEntity.Size.x - 1, MapEntity.Size.y - 1);
        //    List<CellEntity> borders = new List<CellEntity>();
        //    for (int i = 0; i < density; i++)
        //    {
        //        int x = UnityEngine.Random.Range(0, MapEntity.Size.x);
        //        int y = UnityEngine.Random.Range(0, MapEntity.Size.y);

        //        var cellEntity = MapEntity.GetCell(x, y);

        //        var currentDirection = new List<CellEntity>();
        //        while (true)
        //        {
        //            var a = UnityEngine.Random.value;
        //            if (a < singleWallConnectionPossibility)
        //            {
        //                CellEntity item = MapEntity.GetCell(cellEntity.x + 1, cellEntity.y);
        //                currentDirection.Add(item);
        //                if (item) if (item.NearByCells.Except(currentDirection).All((c) => c.data.canStandOn))
        //                        borders.Add(item);
        //            }
        //            else
        //            {
        //                currentDirection = new List<CellEntity>();
        //                break;
        //            }
        //        } while (true)
        //        {
        //            var a = UnityEngine.Random.value;
        //            if (a < singleWallConnectionPossibility)
        //            {
        //                CellEntity item = MapEntity.GetCell(cellEntity.x - 1, cellEntity.y);
        //                currentDirection.Add(item);
        //                if (item) if (item.NearByCells.Except(currentDirection).All((c) => c.data.canStandOn))
        //                        borders.Add(item);
        //            }
        //            else
        //            {
        //                currentDirection = new List<CellEntity>();
        //                break;
        //            }
        //        } while (true)
        //        {
        //            var a = UnityEngine.Random.value;
        //            if (a < singleWallConnectionPossibility)
        //            {
        //                CellEntity item = MapEntity.GetCell(cellEntity.x, cellEntity.y + 1);
        //                currentDirection.Add(item);
        //                if (item) if (item.NearByCells.Except(currentDirection).All((c) => c.data.canStandOn))
        //                        borders.Add(item);
        //            }
        //            else
        //            {
        //                currentDirection = new List<CellEntity>();
        //                break;
        //            }
        //        } while (true)
        //        {
        //            var a = UnityEngine.Random.value;
        //            if (a < singleWallConnectionPossibility)
        //            {
        //                CellEntity item = MapEntity.GetCell(cellEntity.x, cellEntity.y - 1);
        //                currentDirection.Add(item);
        //                if (item) if (item.NearByCells.Except(currentDirection).All((c) => c.data.canStandOn))
        //                        borders.Add(item);
        //            }
        //            else
        //            {
        //                currentDirection = new List<CellEntity>();
        //                break;
        //            }
        //        }

        //    }
        //    MapEntity.CloseCells(borders);


        //    CampusEntity campusEntity = CampusEntity.GetCampus(Game.CurrentBattle.Player);
        //    MapEntity.OpenCells(campusEntity.OnCellOf);
        //    MapEntity.OpenCells(campusEntity.OnCellOf.NearByCells);
        //}
    }
}
