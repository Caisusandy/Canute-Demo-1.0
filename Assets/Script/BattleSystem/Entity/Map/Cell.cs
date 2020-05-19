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

        public override string Name => base.Name + Position;
        public override Cell OnCellOf => this;
        public override StatusList GetAllStatus() => StatList;

        public List<BattleBuilding> Buildings => Game.CurrentBattle.GetBuildings(Position);
        public List<BattleArmy> Armies => Game.CurrentBattle.GetArmies(Position);
        public Terrain Terrain => (Terrain)Enum.Parse(typeof(Terrain), terrain);

        public static Vector2Int V32V2(Vector3Int pos)
        {
            return new Vector2Int(pos.x + pos.y / 2, pos.y);
        }
    }
}