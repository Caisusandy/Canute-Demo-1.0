using System;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public class StartPointPair
    {
        public Vector2Int pos;
        public MapDirection direction;
        public CellEntity CellEntity => Game.CurrentBattle.MapEntity[pos];
    }
}