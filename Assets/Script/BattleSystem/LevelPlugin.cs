namespace Canute.BattleSystem
{
    public abstract class LevelPlugin
    {
        public abstract bool WinCondition();
        public abstract bool LostCondition();
    }
}
