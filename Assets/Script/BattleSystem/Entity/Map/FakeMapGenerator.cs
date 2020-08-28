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
        public void CreateFakeCells()
        {
            fakeColumn = new GameObject("fakeCells");
            fakeColumn.transform.SetParent(MapEntity.CurrentMap.transform);

            for (int i = -fakeLevel; i < MapEntity.CurrentMap.columnEntities[0].cellEntities.Count + fakeLevel; i++)
                //y
                for (int j = -fakeLevel; j < MapEntity.CurrentMap.columnEntities.Count + fakeLevel; j++)
                {
                    if (!MapEntity.CurrentMap.GetCell(i, j))
                        CreateFakeCell(i, j);
                }

            ////IV Quadrant
            //{
            //    //x
            //    for (int i = 0; i < MapEntity.CurrentMap.columnEntities[0].cellEntities.Count + fakeLevel; i++)
            //        //y
            //        for (int j = -1; j > -1 - fakeLevel; j--)
            //            CreateFakeCell(i, j);
            //}

            ////III Quadrant
            //{
            //    //x
            //    for (int i = -1; i > -1 - fakeLevel; i--)
            //        //y
            //        for (int j = -1; j > -1 - fakeLevel; j--)
            //            CreateFakeCell(i, j);

            //}

            ////II Quadrant
            //{
            //    //x
            //    for (int i = -1; i > -1 - fakeLevel; i--)
            //        //y
            //        for (int j = 0; j < MapEntity.CurrentMap.columnEntities.Count + fakeLevel; j++)
            //            CreateFakeCell(i, j);
            //}

            ////I Quadrant
            //{
            //    //x
            //    for (int i = 0; i < MapEntity.CurrentMap.columnEntities[0].cellEntities.Count + fakeLevel; i++)
            //        //y
            //        for (int j = 0; j < MapEntity.CurrentMap.columnEntities.Count + fakeLevel; j++)
            //        {
            //            if (!MapEntity.CurrentMap.GetCell(i, j))
            //                CreateFakeCell(i, j);
            //        }
            //}

            fakeColumn.transform.localScale = Vector3.one;
            fakeColumn.transform.position = MapEntity.CurrentMap.Origin.transform.position;
            //Debug.Log(fakeColumn.transform.position);
            //Debug.Log(MapEntity.CurrentMap.GetCell(0, 0).transform.position);
        }


        private void CreateFakeCell(int x, int y)
        {
            var cellEntity = Instantiate(fakeCellPrefab, fakeColumn.transform);

            Vector3 pos = new Vector3(CellSizeDefault.dist_x * x + y % 2 * CellSizeDefault.dist_x / 2, CellSizeDefault.dist_y * y, 0);
            cellEntity.transform.localPosition = pos;
            //cellEntity.transform.localPosition = new Vector3(cellEntity.transform.localPosition.x, cellEntity.transform.localPosition.y, 0);
            //Debug.Log(pos); Debug.Log(cellEntity.transform.localPosition);
            cellEntity.name = "FakeCell(" + x + "," + y + ")";
            cellEntity.GetComponent<SpriteRenderer>().sortingOrder = -x;
        }
    }
}
