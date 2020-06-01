namespace Canute.BattleSystem
{
    public partial class Effect
    {
        #region Basic Arg of Effect/Status

        public const string bonusType = "bonusType";
        public const string effectType = "effectType";
        public const string effectSpecialName = "specialName";
        public const string isStatus = "isStatus";
        public const string name = "name";
        public const string propertyBonusType = "propertyBonus";
        public const string statusCount = "sc";
        public const string turnCount = "tc";
        public const string triggerCondition = "triggerCondition";
        public const string statusType = "statType";
        public const string value = "value";

        #endregion

        public enum Types
        {
            none,                   //nothing

            enterMove,
            move,

            enterAttack,
            attack,                 //param ["type"] {"isCritical"}
            realDamage,             //param
            skill,                  //-     skill (source must be battleEntity)

            createArmy,             //-     create an army on the field 

            propertyBonus,         //param ["bonusType"] ["propertyType"]    a bonus to the properties (not directly add in, but as a status)
            propertyPanalty,        //param ["bonusType"] ["propertyType"]

            weather,                //param ["weather"] not impliment right now

            addStatus,              //param ["effectType"] type of effect, ["tc"], ["sc"], ["statType"];
            removeStatus,           //param ["uuid"] uuid of the effect/status;

            @event,                 //param ["name"] name of event (action)
            effectRelated,          //param ["name"] name of the effect-related effect
            tag,                    //param ["name"] name of tag (never able to execute)
        }
    }
}