using Canute.BattleSystem.Develop;
using System;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class MapGenerator : MonoBehaviour
    {
        public int rect_x;
        public int rect_y;

        public GameObject columnPrefab;
        public GameObject cellPrefab;

        public MapEntity mapEntity;

        public bool random;

        public void Awake()
        {
            CreateMap();
        }

        [ContextMenu("CreateMap")]
        public MapEntity CreateMap()
        {
            mapEntity = GetComponent<MapEntity>();
            for (int i = 0; i < rect_y; i++)
            {
                var column = Instantiate(columnPrefab, mapEntity.transform).GetComponent<ColumnEntity>();
                mapEntity.columnEntities.Add(column);

                column.name = "Column(" + i + ")";
                column.transform.position = new Vector3((i % 2) * CellSizeDefault.dist_x / 2, CellSizeDefault.dist_y * i, 0);
                column.transform.localPosition = new Vector3(column.transform.localPosition.x * transform.lossyScale.x, column.transform.localPosition.y * transform.lossyScale.y, 0);

                for (int j = 0; j < rect_x; j++)
                {
                    var cellEntity = CreateCell(j, i, column.transform);
                    column.cellEntities.Add(cellEntity);
                    cellEntity.data.Coordinate = new Vector2Int(j, i);
                }
            }
            if (random)
            {
                var random = new RandomMapGenerator(this);
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
