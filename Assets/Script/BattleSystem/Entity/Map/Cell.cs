using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public class Cell : OnMapEntityData
    {
        [Header("Cell Stand")]
        public Terrain terrain;
        [Tooltip("if anything(army, building, etc.) can stand on, map can still access it anyway")]
        public bool canStandOn = true;
        [Tooltip("not to show this cell when map is loaded, also there will be no access to this cell on the map")]
        public bool hide = false;

        public override string Name => base.Name + Coordinate;
        public override Cell OnCellOf => this;
        public override StatusList GetAllStatus() => StatList;

        public List<BattleBuilding> Buildings => Game.CurrentBattle.GetBuildings(Coordinate);
        public List<BattleArmy> Armies => Game.CurrentBattle.GetArmies(Coordinate);

        public BattleArmy HasArmyStandOn => Game.CurrentBattle?.GetArmy(Coordinate);
        public BattleBuilding HasBuildingStandOn => Game.CurrentBattle?.GetBuilding(Coordinate);
        public Terrain Terrain => terrain;//(Terrain)Enum.Parse(typeof(Terrain), terrain);


        public new CellEntity Entity => BattleSystem.Entity.Get<CellEntity>(uuid);


        public static Vector2Int V32V2(Vector3Int pos)
        {
            return new Vector2Int(pos.x + pos.y / 2, pos.y);
        }
    }
}