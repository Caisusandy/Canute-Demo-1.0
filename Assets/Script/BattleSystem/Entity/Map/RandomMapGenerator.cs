using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.BattleSystem
{

    public class RandomMapGenerator
    {
        const float scale = (1f / int.MaxValue) / 2f;
        const float riverweight = 0;//0.025f;
        const float sand = 0;//0.1f;
        const float mountains = 0.1f;// 0.2f;
        const float hill = 0.3f;
        const float plain = 0.6f;
        const float forest = 1.0f;//0.9f;
        const float swamp = 0;//1.0f;

        int seed;
        MapEntity mapEntity;


        public RandomMapGenerator(Transform anchor, int x, int y)
        {
            var generator = anchor.gameObject.AddComponent<MapGenerator>();
            generator.rect_x = x;
            generator.rect_y = y;
            mapEntity = generator.CreateMap();
            mapEntity.isRandomMap = true;
            mapEntity.seed = seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        public RandomMapGenerator(MapGenerator generator)
        {
            mapEntity = generator.mapEntity;
            mapEntity.isRandomMap = true;
            mapEntity.seed = seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);

        }


        public void Randomize(int seed)
        {
            this.seed = seed;
            foreach (var column in mapEntity)
            {
                foreach (var cell in column)
                {
                    cell.data.terrain = GetTerrain(cell.x, cell.y);
                }
            }
        }
        public void Randomize()
        {
            foreach (var column in mapEntity)
            {
                foreach (var cell in column)
                {
                    //Debug.Log(cell.Coordinate);
                    cell.data.terrain = GetTerrain(cell.x, cell.y);
                    cell.SetCellSprite();
                }
            }
        }

        public Terrain GetTerrain(int x, int y)
        {
            float p = Perlin(x, y, seed);
            p = p < 0 ? 0 : (p > 1 ? 1 : p);
            Debug.Log(p);
            return (GetTerrain(p));
        }

        public Terrain GetTerrain(float value)
        {
            if (value < sand)
                return Terrain.Sand;
            if (value < sand + riverweight)
                return Terrain.River;

            if (value < mountains)
                return Terrain.Mountains;
            //if (value < mountains + riverweight)
            //    return Terrain.River;

            if (value < hill)
                return Terrain.Hills;
            if (value < hill + riverweight)
                return Terrain.River;

            if (value < plain)
                return Terrain.Plain;
            if (value < plain + riverweight * 2)
                return Terrain.River;

            if (value < forest)
                return Terrain.Forest;
            if (value < forest + riverweight * 2)
                return Terrain.River;

            if (value < swamp)
                return Terrain.Swamp;
            if (value < swamp + riverweight)
                return Terrain.River;

            throw new ArgumentOutOfRangeException(nameof(value), value, "value: " + value + " is not a valid value");
        }

        public static float Perlin(float x, float y, int seed)
        {
            float x1 = seed / 2048f + x / 8.1f;
            float y1 = seed / 2048f + y / 8.1f;
            //Debug.Log(x1);
            //Debug.Log(y1);
            return Mathf.PerlinNoise(x1, y1);
        }
    }
}
