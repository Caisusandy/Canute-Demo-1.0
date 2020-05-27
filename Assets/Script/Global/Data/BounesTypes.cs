namespace Canute.BattleSystem
{
    public enum BonusType
    {
        /// <summary>
        /// a + b
        /// </summary>
        additive,
        /// <summary>
        /// a * (1 + b / 100)  
        /// </summary>
        percentage,
    }
}