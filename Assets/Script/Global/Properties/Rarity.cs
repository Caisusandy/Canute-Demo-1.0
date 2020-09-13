namespace Canute
{
    public enum Rarity
    {
        common,
        rare,
        epic,
        legendary,
        none,
    }

    public interface IRarityLabled
    {
        Rarity Rarity { get; }
    }
}
