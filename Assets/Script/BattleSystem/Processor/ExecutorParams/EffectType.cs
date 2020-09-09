namespace Canute.BattleSystem
{
    public partial class Effect
    {
        #region Basic Arg of Effect/Status

        public const string statusShowToPlayer = "showToPlayer";
        public const string bonusType = "bonusType";
        public const string name = "name";
        public const string propertyType = "propertyType";
        public const string value = "value";

        /// <summary> The status Count of the status when the effect is converting to Status </summary>
        public const string statusCount = "sc";
        /// <summary> The turn Count of the status when the effect is converting to Status </summary>
        public const string turnCount = "tc";
        /// <summary> The trigger Condition of the status when the effect is converting to Status </summary>
        public const string triggerCondition = "triggerCondition";
        /// <summary> The status type of the status when the effect is converting to Status </summary>
        public const string statusType = "statType";
        /// <summary> The new effectType of the status when the effect is converting to Status </summary>
        public const string effectType = "effectType";
        /// <summary> The special name used when the effect is converting to Status </summary>
        public const string effectSpecialName = "specialName";
        /// <summary> Is Effect a status </summary>
        public const string isStatus = "isStatus";

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