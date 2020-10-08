using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class CellColoration
    {
        public MapEntity mapEntity;

        public CellColoration(MapEntity cellEntities)
        {
            this.mapEntity = cellEntities;
        }

        public IEnumerator Color()
        {
            foreach (var column in mapEntity)
            {
                foreach (var cell in column)
                {
                    if (cell.data.terrain != Terrain.Plain)
                        cell.SetCellSprite();
                    else
                        cell.SetCellSprite(GameData.SpriteLoader.Get(SpriteAtlases.cells, Terrain.Plain.ToString() + PerlinInt(cell.x, cell.y)));
                }
            }
            foreach (var cell in mapEntity.fakeCells)
            {
                if (cell.terrain != Terrain.Plain)
                    cell.SetCellSprite();
                else
                    cell.SetCellSprite(GameData.SpriteLoader.Get(SpriteAtlases.cells, Terrain.Plain.ToString() + PerlinInt(cell.x, cell.y)));
            }
            yield return null;
        }

        public int PerlinInt(float x, float y)
        {
            return (int)(Perlin(x, y, mapEntity.seed) * 12);
        }

        public float Perlin(float x, float y, int seed)
        {
            if (Mathf.Abs(seed) > Mathf.Pow(2, 20))
            {
                seed %= (int)Mathf.Pow(2, 20);
            }
            //            Debug.Log(seed);
            float scale = (mapEntity.Size.x + mapEntity.Size.y) / 5.5f;
            float x1 = seed + x / mapEntity.Size.x;
            float y1 = seed + y / mapEntity.Size.y;
            //Debug.Log(x1);
            //Debug.Log(y1);
            var a = Mathf.PerlinNoise(x1 * scale, y1 * scale);
            a = a < 0 ? 0 : (a > 1 ? 0.9999f : a);
            return a;
        }
    }
}
