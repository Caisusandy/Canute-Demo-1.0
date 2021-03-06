﻿using System;
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
            if (!mapEntity.isDrew)
                foreach (var column in mapEntity)
                {
                    foreach (var cell in column)
                    {
                        int max = cell.data.terrain != Terrain.Plain ? 4 : 12;
                        cell.SetCellSprite(GameData.SpriteLoader.Get(SpriteAtlases.cells, cell.data.terrain.ToString() + PerlinInt(cell.x, cell.y, max)));
                    }
                }
            foreach (var cell in mapEntity.fakeCells)
            {
                int max = cell.terrain != Terrain.Plain ? 4 : 12;
                cell.SetCellSprite(GameData.SpriteLoader.Get(SpriteAtlases.cells, cell.terrain.ToString() + PerlinInt(cell.x, cell.y, max)));
            }
            yield return null;
        }

        public int PerlinInt(float x, float y, int max)
        {
            return (int)(Perlin(x, y, mapEntity.seed) * max);
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
            a = a < 0 ? 0 : (a > 1 ? 1f : a);
            return a;
        }
    }
}
