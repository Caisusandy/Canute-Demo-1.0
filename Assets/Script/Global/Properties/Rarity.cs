using UnityEngine;

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

    public static class RarityLabled
    {
        public static Sprite GetRaritySprite(this IRarityLabled item)
        {
            return GameData.SpriteLoader.Get(SpriteAtlases.rarity, item.Rarity.ToString());
        }
    }
}
