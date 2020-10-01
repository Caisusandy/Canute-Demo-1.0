using Canute.BattleSystem.Develop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class FakeMapGenerator : MonoBehaviour
    {
        public static int fakeLevel = 15;
        public static FakeMapGenerator instance;

        public GameObject fakeCellPrefab;
        public GameObject fakeColumn;

        public void Awake()
        {
            instance = this;
        }

        [ContextMenu("Create Fake Cells")]
        public List<FakeCell> CreateFakeCells()
        {
            var list = new List<FakeCell>();
            fakeColumn = new GameObject("fakeCells");
            fakeColumn.transform.SetParent(MapEntity.CurrentMap.transform);

            for (int i = -fakeLevel; i < MapEntity.CurrentMap.columnEntities[0].cellEntities.Count + fakeLevel; i++)
                //y
                for (int j = -fakeLevel; j < MapEntity.CurrentMap.columnEntities.Count + fakeLevel; j++)
                {
                    if (!MapEntity.CurrentMap.GetCell(i, j))
                    {
                        var a = CreateFakeCell(i, j);
                        list.Add(a);
                    }
                }

            fakeColumn.transform.localScale = Vector3.one;
            fakeColumn.transform.position = MapEntity.CurrentMap.Origin.transform.position;

            return list;
        }


        private FakeCell CreateFakeCell(int x, int y)
        {
            var fakeCellObject = Instantiate(fakeCellPrefab, fakeColumn.transform);

            Vector3 pos = new Vector3(CellSizeDefault.dist_x * x + y % 2 * CellSizeDefault.dist_x / 2, CellSizeDefault.dist_y * y, 0);
            fakeCellObject.transform.localPosition = pos;

            //cellEntity.transform.localPosition = new Vector3(cellEntity.transform.localPosition.x, cellEntity.transform.localPosition.y, 0);
            //Debug.Log(pos); Debug.Log(cellEntity.transform.localPosition);

            fakeCellObject.name = "FakeCell(" + x + "," + y + ")";
            fakeCellObject.GetComponent<SpriteRenderer>().sortingLayerName = "Map";
            fakeCellObject.GetComponent<SpriteRenderer>().sortingOrder = -y;
            fakeCellObject.GetComponent<FakeCell>().pos = new Vector2Int(x, y);


            return fakeCellObject.GetComponent<FakeCell>();
        }
    }
}
