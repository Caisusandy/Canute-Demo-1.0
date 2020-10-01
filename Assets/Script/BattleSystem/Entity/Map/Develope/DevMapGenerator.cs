using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEditor;

namespace Canute.BattleSystem.Develop
{
    public static class CellSizeDefault
    {
        public const float dist_x = 40;
        public const float dist_y = 34.5f;
    }

    public class DevMapGenerator : MapGenerator
    {
        public bool useSeed;
        public int seed;

        public GameObject mapPrefab;


        [ContextMenu("CreateMap")]
        public override MapEntity CreateMap()
        {
            mapEntity = Instantiate(mapPrefab, transform).GetComponent<MapEntity>();
            for (int i = 0; i < rect_y; i++)
            {
                var column = Instantiate(columnPrefab, mapEntity.transform).GetComponent<ColumnEntity>();
                mapEntity.columnEntities.Add(column);

                column.name = "Column(" + i + ")";
                column.transform.position = new Vector3((i % 2) * CellSizeDefault.dist_x / 2, CellSizeDefault.dist_y * i, 0);
                column.transform.localPosition = new Vector3(column.transform.localPosition.x, column.transform.localPosition.y, 0);

                for (int j = 0; j < rect_x; j++)
                {
                    var cellEntity = CreateCell(j, i, column.transform);
                    column.cellEntities.Add(cellEntity);
                    cellEntity.data.Coordinate = new Vector2Int(j, i);
                }
            }
            if (random)
            {
                RandomTerrainGenerator random = useSeed ? new RandomTerrainGenerator(mapEntity, seed) : new RandomTerrainGenerator(mapEntity);
                random.Randomize();
            }
            return mapEntity;
        }

        private CellEntity CreateCell(int x, int y, Transform column)
        {
            var cellEntity = Instantiate(cellPrefab, column);

            Vector3 pos = new Vector3(CellSizeDefault.dist_x * x, 0, 0);

            cellEntity.transform.localPosition = pos;
            cellEntity.name = "Cell(" + x + ")";
            cellEntity.GetComponent<SpriteRenderer>().sortingOrder = -x;

            return cellEntity.GetComponent<CellEntity>();
        }
    }
}
