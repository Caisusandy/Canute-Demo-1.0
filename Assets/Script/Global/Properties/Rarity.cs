namespace Canute
{
    public enum Rarity
    {
        common,
        rare,
        epic,
        legendary,
    }

    public interface IRarityLabled
    {
        Rarity Rarity { get; }
    }
}
