namespace Canute.BattleSystem
{
    public interface ISkillable
    {
        /// <summary>
        /// trigger skill
        /// </summary>
        /// <param name="vs"> skill effect </param>
        void Skill(params object[] vs);
    }
}