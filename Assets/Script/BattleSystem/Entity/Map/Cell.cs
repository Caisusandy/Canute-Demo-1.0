using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public class Cell : OnMapEntityData
    {
        public bool canStandOn = true;
        public bool hide = false;
        public string terrain;

        public override string Name => base.Name + Coordinate;
        public override Cell OnCellOf => this;
        public override StatusList GetAllStatus() => StatList;

        public List<BattleBuilding> Buildings => Game.CurrentBattle.GetBuildings(Coordinate);
        public List<BattleArmy> Armies => Game.CurrentBattle.GetArmies(Coordinate);
        public Terrain Terrain => (Terrain)Enum.Parse(typeof(Terrain), terrain);

        public static Vector2Int V32V2(Vector3Int pos)
        {
            return new Vector2Int(pos.x + pos.y / 2, pos.y);
        }
    }
}