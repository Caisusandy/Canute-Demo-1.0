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
        public void Start()
        {
            GetComponent<SpriteRenderer>().sprite = GameData.SpriteLoader.Get(SpriteAtlases.cells, Terrain.Plain.ToString() + UnityEngine.Random.Range(0, 11));
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
