using Canute.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class TerrainRange
    {
        public Vector2 min;
        public Vector2 max;
        public Terrain terrain;

        public TerrainRange(Vector2 min, Vector2 max, Terrain terrain)
        {
            this.min = min;
            this.max = max;
            this.terrain = terrain;
        }

        public bool IsInRange(Vector2 vector2)
        {
            return vector2.x >= min.x && vector2.y >= min.y && vector2.x < max.x && vector2.y < max.y;
        }
    }

    public class RandomTerrainGenerator
    {
        //x=>moisture
        //y=>temperature

        private TerrainRange swamp = new TerrainRange(new Vector2(0.75f, 0.5f), new Vector2(1f, 1f), Terrain.Swamp);
        private TerrainRange sand = new TerrainRange(new Vector2(0, 0.7f), new Vector2(0.3f, 1), Terrain.Sand);
        private TerrainRange mountains = new TerrainRange(new Vector2(0.25f, 0.1f), new Vector2(0.35f, 0.45f), Terrain.Mountains);
        private TerrainRange highland = new TerrainRange(new Vector2(0.25f, 0.25f), new Vector2(0.45f, 0.5f), Terrain.Highland);
        private TerrainRange hill = new TerrainRange(new Vector2(0.4f, 0.2f), new Vector2(0.6f, 0.5f), Terrain.Hills);
        private TerrainRange forest = new TerrainRange(new Vector2(0.3f, 0.3f), new Vector2(0.7f, 1.0f), Terrain.Forest);
        private TerrainRange plain = new TerrainRange(new Vector2(0.2f, 0.15f), new Vector2(0.4f, 0.8f), Terrain.Plain);
        private TerrainRange forest2 = new TerrainRange(new Vector2(0.6f, 0.6f), new Vector2(0.9f, 1f), Terrain.Forest);
        private TerrainRange snow = new TerrainRange(new Vector2(0.0f, 0.0f), new Vector2(0.2f, 0.2f), Terrain.Snow);

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



        public void RandomizeTerrain()
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


        public void RandomizeFakeCellTerrain()
        {
            foreach (var cell in mapEntity.fakeCells)
            {
                cell.terrain = GetTerrain(cell.pos.x, cell.pos.y);
            }
        }

        public Terrain GetTerrain(int x, int y)
        {
            var p = PerlinV2(x, y, seed);
            return (GetTerrain(p));
        }

        public Terrain GetTerrain(Vector2 value)
        {
            var terrain = new TerrainRange[] { swamp, sand, snow, highland, hill, plain, forest, mountains };

            foreach (var item in terrain)
            {
                if (item.IsInRange(value))
                {
                    return item.terrain;
                }
            }
            Debug.Log(value);
            return Terrain.Plain;
            throw new ArgumentOutOfRangeException(nameof(value), value, "value: " + value + " is not a valid value");
        }

        public float Perlin(float x, float y, int seed)
        {
            if (Mathf.Abs(seed) > Mathf.Pow(2, 20))
            {
                seed %= (int)Mathf.Pow(2, 20);
            }
            //            Debug.Log(seed);
            float scale = (mapEntity.Size.x + mapEntity.Size.y) / 11;
            float x1 = seed + x / mapEntity.Size.x;
            float y1 = seed + y / mapEntity.Size.y;
            //Debug.Log(x1);
            //Debug.Log(y1);
            return (Mathf.PerlinNoise(x1 * scale, y1 * scale));
        }

        public Vector2 PerlinV2(float x, float y, int seed)
        {
            var a = Perlin(x, y, seed);
            var b = Perlin(-x, -y, 1 - seed);
            a = a < 0 ? 0 : (a > 1 ? 0.9999f : a);
            b = b < 0 ? 0 : (b > 1 ? 0.9999f : b);
            return new Vector2(a, b);
        }
    }
    //public class RandomTerrainGenerator
    //{
    //    //const float scale = (1f / int.MaxValue) / 2f;
    //    private float riverweight = 0;//0.025f;
    //    const float sand = 0;//0.1f;
    //    const float mountains = 0.15f;// 0.2f;
    //    const float hill = 0.35f;
    //    const float plain = 0.65f;
    //    const float forest = 1.0f;//0.9f;
    //    const float swamp = 0;//1.0f;

    //    int seed;
    //    MapEntity mapEntity;


    //    public RandomTerrainGenerator(Transform anchor, int x, int y)
    //    {
    //        var generator = anchor.gameObject.AddComponent<MapGenerator>();
    //        generator.rect_x = x;
    //        generator.rect_y = y;
    //        mapEntity = generator.CreateMap();
    //        mapEntity.isRandomMap = true;
    //        mapEntity.seed = seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    //    }

    //    public RandomTerrainGenerator(MapEntity mapEntity)
    //    {
    //        this.mapEntity = mapEntity;
    //        mapEntity.isRandomMap = true;
    //        mapEntity.seed = seed = UnityEngine.Random.Range(int.MinValue, int.MaxValue);
    //    }

    //    public RandomTerrainGenerator(MapEntity mapEntity, int seed)
    //    {
    //        this.mapEntity = mapEntity;
    //        mapEntity.isRandomMap = true;
    //        mapEntity.seed = this.seed = seed;
    //    }



    //    public void RandomizeTerrain()
    //    {
    //        foreach (var column in mapEntity)
    //        {
    //            foreach (var cell in column)
    //            {
    //                //Debug.Log(cell.Coordinate);
    //                cell.data.terrain = GetTerrain(cell.x, cell.y);
    //                cell.SetCellSprite();
    //            }
    //        }
    //    }


    //    public void RandomizeFakeCellTerrain()
    //    {
    //        foreach (var cell in mapEntity.fakeCells)
    //        {
    //            cell.terrain = GetTerrain(cell.pos.x, cell.pos.y);
    //        }
    //    }

    //    public Terrain GetTerrain(int x, int y)
    //    {
    //        float p = Perlin(x, y, seed);
    //        p = p < 0 ? 0 : (p > 1 ? 0.9999f : p);
    //        //Debug.Log(p);
    //        return (GetTerrain(p));
    //    }

    //    public Terrain GetTerrain(float value)
    //    {
    //        if (value < sand)
    //            return Terrain.Sand;
    //        if (value < sand + riverweight)
    //            return Terrain.River;

    //        if (value < mountains)
    //            return Terrain.Mountains;
    //        //if (value < mountains + riverweight)
    //        //    return Terrain.River;

    //        if (value < hill)
    //            return Terrain.Hills;
    //        if (value < hill + riverweight)
    //            return Terrain.River;

    //        if (value < plain)
    //            return Terrain.Plain;
    //        if (value < plain + riverweight * 2)
    //            return Terrain.River;

    //        if (value < forest)
    //            return Terrain.Forest;
    //        if (value < forest + riverweight * 2)
    //            return Terrain.River;

    //        if (value < swamp)
    //            return Terrain.Swamp;
    //        if (value < swamp + riverweight)
    //            return Terrain.River;

    //        throw new ArgumentOutOfRangeException(nameof(value), value, "value: " + value + " is not a valid value");
    //    }

    //    public float Perlin(float x, float y, int seed)
    //    {
    //        if (Mathf.Abs(seed) > Mathf.Pow(2, 20))
    //        {
    //            seed %= (int)Mathf.Pow(2, 20);
    //        }
    //        //            Debug.Log(seed);
    //        float scale = (mapEntity.Size.x + mapEntity.Size.y) / 10;
    //        float x1 = seed + x / mapEntity.Size.x;
    //        float y1 = seed + y / mapEntity.Size.y;
    //        //Debug.Log(x1);
    //        //Debug.Log(y1);
    //        return (Mathf.PerlinNoise(x1 * scale, y1 * scale));
    //    }
    //}
}
