using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.BattleSystem
{

    public class RandomTerrainGenerator
    {
        //const float scale = (1f / int.MaxValue) / 2f;
        const float riverweight = 0;//0.025f;
        const float sand = 0;//0.1f;
        const float mountains = 0.15f;// 0.2f;
        const float hill = 0.35f;
        const float plain = 0.65f;
        const float forest = 1.0f;//0.9f;
        const float swamp = 0;//1.0f;

        int seed;
        MapEntity mapEntity;


        public RandomTerrainGenerator(Transform anchor, int x, int y)
        {
            var generator = anchor.gameObject.AddComponent<MapGenerator>();
            generator.rect_x = x;
            generator.rect_y = y;
            mapEntity = generator.CreateMap();
            mapEntity.isRandomMap = true;
            mapEntity.seed = seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        public RandomTerrainGenerator(MapEntity mapEntity)
        {
            this.mapEntity = mapEntity;
            mapEntity.isRandomMap = true;
            mapEntity.seed = seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
        }

        public RandomTerrainGenerator(MapEntity mapEntity, int seed)
        {
            this.mapEntity = mapEntity;
            mapEntity.isRandomMap = true;
            mapEntity.seed = this.seed = seed;
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


        public void RandomizeFakeCell()
        {
            foreach (var cell in mapEntity.fakeCells)
            {
                cell.terrain = GetTerrain(cell.pos.x, cell.pos.y);
            }
        }

        public Terrain GetTerrain(int x, int y)
        {
            float p = Perlin(x, y, seed);
            p = p < 0 ? 0 : (p > 1 ? 0.9999f : p);
            //Debug.Log(p);
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

        public float Perlin(float x, float y, int seed)
        {
            if (Mathf.Abs(seed) > Mathf.Pow(2, 20))
            {
                seed %= (int)Mathf.Pow(2, 20);
            }
            //            Debug.Log(seed);
            float scale = (mapEntity.Size.x + mapEntity.Size.y) / 10;
            float x1 = seed + x / mapEntity.Size.x;
            float y1 = seed + y / mapEntity.Size.y;
            //Debug.Log(x1);
            //Debug.Log(y1);
            return (Mathf.PerlinNoise(x1 * scale, y1 * scale));
        }
    }
}
