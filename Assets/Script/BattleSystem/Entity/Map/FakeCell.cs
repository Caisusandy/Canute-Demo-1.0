using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class FakeCell : MonoBehaviour
    {
        public int x => pos.x;
        public int y => pos.y;
        public Vector2Int pos;
        public Terrain terrain;


        [ContextMenu("reload cell sprite")]
        public void SetCellSprite()
        {
            GetComponent<SpriteRenderer>().sprite = GetCellSprite();
            GetComponent<SpriteRenderer>().size = new Vector2(40, 67.72008f);
        }

        public void SetCellSprite(Sprite sprite)
        {
            GetComponent<SpriteRenderer>().sprite = sprite;
            GetComponent<SpriteRenderer>().size = new Vector2(40, 67.72008f);
        }

        private Sprite GetCellSprite()
        {
            return GameData.SpriteLoader.Get(SpriteAtlases.cells, terrain.ToString() + UnityEngine.Random.Range(0, terrain == Terrain.Plain ? 11 : 3));
        }

        public void OnMouseDrag()
        {
            MapEntity.MoveMap();
        }

        public void OnMouseUp()
        {
            MapEntity.WasOnDrag = false;
        }
    }
}
