using System;

namespace Canute.BattleSystem
{
    [Flags]
    public enum TargetType
    {
        any,
        self = 1,
        enemy = 2,
        player = 4,
        armyEntity = 8,
        buildingEntity = 16,
        cellEntity = 32,
        cardEntity = 64,

        passiveEntity = 128,
        aggressiveEntity = 256,

        none = 511
    }
}