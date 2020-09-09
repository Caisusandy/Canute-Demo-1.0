namespace Canute.BattleSystem
{
    public enum BattleEvent
    {

    }

    public enum BattleEventError
    {
        CardNoSelectingEntity,  //Card did not played: no selecting entity
        CardNotValidTarget,     //Card did not played: not a valid target
        CardPlayerHasNoActionPoint,      //Card did not played: no enough action point

        ArmyNoTargetInAttackRage,           //no enemy is in attack range
        ArmyUnderShielderProtection,        //Cannot attack: target is under protection
        ArmyCannotAttack,       //it can't attack anyone

        ////target issue
        //NotValidTarget,         //the target is not a valid target
        //NoTarget,               //no target was selected
        //NoAvailableTarget,      //no target can be attack
        ////army attack issue




        //notEnoughActionPoint,   //the action point is not enough
        //battleActionProcessing, //something is doing
        //animationProcessing,


    }
}
