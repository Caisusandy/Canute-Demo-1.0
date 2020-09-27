using Canute.BattleSystem.Buildings;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class WaveControl
    {
        List<WaveInfo> waveInfos;
        Battle battle;
        Battle Battle => battle;

        public WaveControl()
        {
        }

        public WaveControl(List<WaveInfo> waveInfos, Battle battle)
        {
            this.waveInfos = waveInfos.Clone();
            this.battle = battle;
        }


        public void NextWave()
        {
            Battle.Round.wave++;
            Battle.Round.turn = 1;
            Battle.Round.round = 1;

            switch (Battle.BattleType)
            {
                case Battle.Type.special:
                    if (waveInfos.Count >= Battle.Round.wave)
                        GenerateEnemy(Battle.Round.wave);
                    break;

                case Battle.Type.normal:
                    if (waveInfos.Count >= Battle.Round.wave)
                        GenerateEnemy(Battle.Round.wave);
                    else
                        Battle.Win();
                    break;

                case Battle.Type.endless:
                    if (waveInfos.Count > Battle.Round.wave)
                        GenerateEnemy(waveInfos.Count);
                    else
                        GenerateExtraWaveEnemy(Battle.Round.wave);
                    break;

                case Battle.Type.rescueMission:
                    if (waveInfos.Count >= Battle.Round.wave)
                        GenerateEnemy(Battle.Round.wave);
                    else
                        Battle.Lost();
                    break;
            }
        }

        private void GetPosition(SpawnAnchor spawnAnchor)
        {
            int positionGetTime = 0;
            Beginning:
            CellEntity cellEntity;
            if (positionGetTime > 100)
            {
                return;
            }
            switch (spawnAnchor.spawnType)
            {
                case SpawnAnchor.SpawnType.coordinated:
                    return;
                case SpawnAnchor.SpawnType.range:
                    var center = Game.CurrentBattle.MapEntity[spawnAnchor.Coordinate];
                    var possible = Game.CurrentBattle.MapEntity.GetNearbyCell(center, spawnAnchor.radius);
                    if (possible.Count > 0)
                    {
                        cellEntity = possible[(int)(UnityEngine.Random.value * possible.Count)];
                        if (!cellEntity) { positionGetTime++; goto Beginning; }
                        if (!cellEntity.data.canStandOn) { positionGetTime++; goto Beginning; }
                        if (cellEntity.HasArmyStandOn) { positionGetTime++; goto Beginning; }
                        if (CampusEntity.GetCampus(Game.CurrentBattle.Player).PossibleCells.Contains(cellEntity)) { positionGetTime++; goto Beginning; }
                        spawnAnchor.Coordinate = cellEntity.Coordinate;
                        break;
                    }
                    goto Beginning;
                case SpawnAnchor.SpawnType.totalRandom:
                    int x = UnityEngine.Random.Range(0, Game.CurrentBattle.MapEntity.columnEntities[0].cellEntities.Count);
                    int y = UnityEngine.Random.Range(0, Game.CurrentBattle.MapEntity.columnEntities.Count);
                    var pos = new Vector2Int(x, y);
                    cellEntity = Game.CurrentBattle.MapEntity.GetCell(pos);
                    if (!cellEntity) { positionGetTime++; goto Beginning; }
                    if (!cellEntity.data.canStandOn) { positionGetTime++; goto Beginning; }
                    if (cellEntity.HasArmyStandOn) { positionGetTime++; goto Beginning; }
                    if (CampusEntity.GetCampus(Game.CurrentBattle.Player).PossibleCells.Contains(cellEntity)) { positionGetTime++; goto Beginning; }
                    spawnAnchor.Coordinate = cellEntity.Coordinate;
                    break;
                default:
                    break;

            }
            if (spawnAnchor is BattleArmySpawnAnchor)
            {
                if (CampusEntity.GetCampus(Game.CurrentBattle.Player))
                    if (CampusEntity.GetCampus(Game.CurrentBattle.Player).Range.Contains(Game.CurrentBattle.MapEntity.GetCell(spawnAnchor.Coordinate)))
                    { positionGetTime++; goto Beginning; }
            }
        }

        private void CreateArmy(BattleArmySpawnAnchor item)
        {
            if (!item.battleArmy.HasValidPrototype && item.Prototype && !TempPrototypes.tempArmyPrototypes.Contains(item.Prototype as ArmyPrototypeContainer))
                TempPrototypes.tempArmyPrototypes.Add(item.Prototype as ArmyPrototypeContainer);

            GetPosition(item);
            Battle.Armies.Add(item);

            ArmyEntity.Create(item);

            //multiple create
            for (int i = 1; i < item.generateCount; i++)
            {
                item.battleArmy = (BattleArmy)item.battleArmy.Clone();
                item.battleArmy.UUID = Guid.NewGuid();
                GetPosition(item);
                Battle.Armies.Add(item);
                ArmyEntity.Create(item);
            }
        }

        private void CreateBuilding(BattleBuildingSheet item)
        {
            if (!item.battleBuilding.HasValidPrototype && item.Prototype && !TempPrototypes.tempBuildingPrototypes.Contains(item.Prototype as BuildingPrototypeContainer))
                TempPrototypes.tempBuildingPrototypes.Add(item.Prototype as BuildingPrototypeContainer);

            GetPosition(item);
            Battle.Buildings.Add(item);

            BuildingEntity.Create(item);

            //multiple create
            for (int i = 1; i < item.generateCount; i++)
            {
                item.battleBuilding.UUID = Guid.NewGuid();
                GetPosition(item);
                Battle.Buildings.Add(item);
                BuildingEntity.Create(item);
            }
        }

        public void GenerateEnemy(int wave)
        {
            if (waveInfos.Count < wave)
            {
                Debug.LogError("a wave that out of bound is tryed to use");
                return;
            }

            Debug.Log("generate wave " + wave);
            //generate wave buildings
            foreach (var item in waveInfos[wave - 1].BattleBuildings)
            {
                CreateBuilding(item);
            }

            //generate wave armies
            foreach (var item in waveInfos[wave - 1].BattleArmies)
            {
                CreateArmy(item);
            }
        }

        public void GenerateExtraWaveEnemy(int wave)
        {
            wave %= waveInfos.Count;
            int bonus = wave / waveInfos.Count;

            //generate first wave buildings
            foreach (var item in waveInfos[wave - 1].BattleBuildings)
            {
                CreateBuilding(item);
            }

            //generate first wave armies
            foreach (var item in waveInfos[wave - 1].BattleArmies)
            {
                item.battleArmy.Health = item.battleArmy.Health.Bonus(20 * bonus);
                item.battleArmy.RawDamage = item.battleArmy.RawDamage.Bonus(20 * bonus);

                CreateArmy(item);
            }
        }
    }
}