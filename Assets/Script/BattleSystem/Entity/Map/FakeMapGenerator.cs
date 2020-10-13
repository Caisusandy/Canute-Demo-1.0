using Canute.BattleSystem.Develop;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class FakeMapGenerator : MonoBehaviour
    {
        public static int fakeLevelX = 12;
        public static int fakeLevelY = 6;
        public static FakeMapGenerator instance;

        public List<FakeCell> fakeCells = new List<FakeCell>();
        public GameObject fakeCellPrefab;
        public GameObject fakeColumn;

        public void Awake()
        {
            instance = this;
        }

        [ContextMenu("Create Fake Cells")]
        public IEnumerator CreateFakeCells()
        {
            var list = new List<FakeCell>();
            fakeColumn = new GameObject("fakeCells");
            fakeColumn.transform.SetParent(MapEntity.CurrentMap.transform);

            for (int x = -fakeLevelX; x < MapEntity.CurrentMap.columnEntities[0].cellEntities.Count + fakeLevelX; x++)
                //y
                for (int y = -fakeLevelY; y < MapEntity.CurrentMap.columnEntities.Count + fakeLevelY; y++)
                {
                    if (!MapEntity.CurrentMap.GetCell(x, y))
                    {
                        GameObject fakeCellObject;
                        yield return fakeCellObject = Instantiate(fakeCellPrefab, fakeColumn.transform);
                        Vector3 pos = new Vector3(CellSize.x * x + y % 2 * CellSize.x / 2, CellSize.y * y, 0);
                        fakeCellObject.transform.localPosition = pos;

                        //cellEntity.transform.localPosition = new Vector3(cellEntity.transform.localPosition.x, cellEntity.transform.localPosition.y, 0);
                        //Debug.Log(pos); Debug.Log(cellEntity.transform.localPosition);

                        fakeCellObject.name = "FakeCell(" + x + "," + y + ")";
                        fakeCellObject.GetComponent<SpriteRenderer>().sortingLayerName = "Map";
                        fakeCellObject.GetComponent<SpriteRenderer>().sortingOrder = -y;
                        fakeCellObject.GetComponent<FakeCell>().pos = new Vector2Int(x, y);
                        list.Add(fakeCellObject.GetComponent<FakeCell>());
                    }
                }

            fakeColumn.transform.localScale = Vector3.one;
            fakeColumn.transform.position = MapEntity.CurrentMap.Origin.transform.position;
            fakeCells = list;
            yield return null;

            //            return list; 
        }


        private FakeCell CreateFakeCell(int x, int y)
        {
            var fakeCellObject = Instantiate(fakeCellPrefab, fakeColumn.transform);

            Vector3 pos = new Vector3(CellSize.x * x + y % 2 * CellSize.x / 2, CellSize.y * y, 0);
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
