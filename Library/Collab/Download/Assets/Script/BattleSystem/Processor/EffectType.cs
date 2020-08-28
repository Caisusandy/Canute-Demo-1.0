namespace Canute.BattleSystem
{
    public partial class Effect
    {
        #region Basic Arg of Effect/Status

        public const string isStatus = "isStatus";
        public const string statusAddingEffect = "SAE";
        public const string tc = "tc";
        public const string sc = "sc";
        public const string value = "value";
        public const string propertyBounes = "propertyBounes";
        public const string bounesType = "bounesType";
        public const string triggerCondition = "triggerCondition";

        #endregion

        public enum Types
        {
            none,               //nothing

            enterAttack,
            attack,             //param ["type"]
            enterMove,
            move,

            realDamage,         //param
            addArmor,           //param     

            propertyBounes,     //param ["bounesType"] ["propertyType"]    a bounes to the properties (not directly add in, but as a status)
            skill,              //-     skill (source must be battleEntity)

            occupy,             //-     occupy a building
            createArmy,         //-     create an army on the field
            hasWeather,         //["weather"] not impliment right now

            drawCard,           //param 
            addActionPoint,     //param 

            armySwitch,         //- 

            //Effect related status
            damageIncreasePercentage,
            damageDecreasePercentage,
            penetrate,          //param


            //lessActionPointNextTime,
        }
    }
}