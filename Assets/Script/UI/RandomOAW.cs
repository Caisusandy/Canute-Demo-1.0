using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public class RandomOAW : MonoBehaviour
    {
        public Image bg;
        public float time;


        // Start is called before the first frame update
        void Start()
        {
            bg.sprite = GetSprite();
        }

        // Update is called once per frame
        void Update()
        {
            time += Time.deltaTime;
            if (time > 3)
            {
                bg.sprite = GetSprite();
            }
        }

        public Sprite GetSprite()
        {
            var s = GameData.SpriteLoader.sprites.Get("OAW");
            Sprite[] sprites = new Sprite[s.spriteCount];
            s.GetSprites(sprites);

            return sprites[Random.Range(0, s.spriteCount)];
        }
    }
}