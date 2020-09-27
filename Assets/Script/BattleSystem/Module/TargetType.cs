using System;

namespace Canute.BattleSystem
{
    [Flags]
    public enum TargetType
    {
        /// <summary> no target limit </summary>
        any,
        /// <summary> same as the people who play the card </summary>
        self = 1,
        /// <summary> different than the people who play the card </summary>
        enemy = 2,
        /// <summary> anything own by a player (target is a player) </summary>
        player = 4,
        /// <summary> an army </summary>
        armyEntity = 8,
        playerArmyEntity = 9,
        enemyArmyEntity = 10,
        /// <summary> a building </summary>
        buildingEntity = 16,
        /// <summary> a cell </summary>
        cellEntity = 32,
        /// <summary> a card </summary>
        cardEntity = 64,

        /// <summary> anything passive </summary>
        passiveEntity = 128,
        /// <summary> anything aggressive </summary>
        aggressiveEntity = 256,

        none = 511
    }
}