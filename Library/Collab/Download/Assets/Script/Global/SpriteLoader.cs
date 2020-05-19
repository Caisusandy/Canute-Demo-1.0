using System;
using System.Collections.Generic;
using UnityEngine;


namespace Canute
{
    [CreateAssetMenu(fileName = "SpriteLoader", menuName = "Game Data/Sprite Loader", order = 1)]
    public class SpriteLoader : ScriptableObject
    {
        public SpriteList Sprites;

        public Sprite Get(string name)
        {
            Sprite sprite = Sprites.Get(name);
            return sprite;
        }

        public Sprite GetIcon(string name)
        {
            return Get("Icon" + name);
        }
    }
    [Serializable]
    public class SpriteList : DList<Sprite>
    {
        public Sprite Get(string name)
        {
            if (list == null)
            {
                return default;
            }
            foreach (Sprite item in list)
            {
                if (item?.name.ToLower() == name.ToLower())
                {
                    return item;
                }
            }
            return default;
        }
    }

}