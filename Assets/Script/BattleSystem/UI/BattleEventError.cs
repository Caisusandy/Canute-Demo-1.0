namespace Canute.BattleSystem
{
    public enum BattleEventError
    {
        //target issue
        notValidTarget,         //the target is not a valid target
        noTarget,               //no target was selected
        noAvailableTarget,      //no target can be attack
        //army attack issue
        armyCannotAttack,       //it can't attack anyone
        armyNoTarget,           //no enemy is in attack range



        notEnoughActionPoint,   //the action point is not enough
        battleActionProcessing, //something is doing
        animationProcessing,


    }
}
