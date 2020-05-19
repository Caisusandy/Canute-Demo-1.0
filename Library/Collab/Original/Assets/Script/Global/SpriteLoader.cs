using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace Canute
{
    [CreateAssetMenu(fileName = "SpriteLoader", menuName = "Game Data/Sprite Loader", order = 1)]
    public class SpriteLoader : ScriptableObject
    {
        public SpriteList sprites;
        public List<Sprite> loneSprites;

        public Sprite Get(string atlasName, string name)
        {
            Sprite sprite = sprites.Get(atlasName, name);
            return sprite;
        }

        public Sprite GetIcon(string name)
        {
            return Get("Icon", name);
        }
    }
    [Serializable]
    public class SpriteList : DList<SpriteAtlas>
    {
        public Sprite Get(string atlasName, string name)
        {
            if (list == null)
            {
                return default;
            }
            foreach (SpriteAtlas item in list)
            {
                if (item.name.ToLower() == atlasName.ToLower())
                {
                    Sprite[] sprites = new Sprite[item.spriteCount];
                    item.GetSprites(sprites);
                    Debug.Log(sprites.Length);
                    foreach (var sprite in sprites)
                    {
                        if (sprite.name.Replace("(Clone)", "").ToLower() == name.ToLower())
                        {
                            return sprite;
                        }
                    }
                }
            }
            Debug.LogWarning(atlasName + ", " + name + " does not exist");
            return default;
        }
    }

    public static class SpriteAtlases
    {
        public const string careerIcon = "CareerIcon";
        public const string cells = "Cells";
        public const string armyTypeIcon = "ArmyTypeIcon";
    }
}