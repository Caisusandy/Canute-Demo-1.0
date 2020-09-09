using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class CellColoration
    {
        public MapEntity cellEntities;

        public CellColoration(MapEntity cellEntities)
        {
            this.cellEntities = cellEntities;
        }

        public void Color()
        {
            foreach (var column in cellEntities)
            {
                foreach (var cell in column)
                {
                    if (cell.data.terrain != Terrain.Plain)
                        cell.SetCellSprite();
                    else
                        cell.GetComponent<SpriteRenderer>().sprite =
                            GameData.SpriteLoader.Get(SpriteAtlases.cells, Terrain.Plain.ToString() + PerlinInt(cell.x, cell.y));
                }
            }
        }

        public int PerlinInt(float x, float y)
        {
            return (int)(Perlin(x, y, cellEntities.seed) * 12);
        }

        public float Perlin(float x, float y, int seed)
        {
            float x1 = seed / 2048f + x / 16.2f;
            float y1 = seed / 2048f + y / 16.2f;
            //Debug.Log(x1);
            //Debug.Log(y1);
            return Mathf.PerlinNoise(x1, y1);
        }
    }
}
