using Canute.Shops;
using System.Collections.Generic;
using System.Linq;

namespace Canute
{
    public static class PrizeLists
    {
        public static IEnumerable<Prize> GetPrizes(this IEnumerable<Prize> prizes, Item.Type type)
        {
            return prizes.Where((p) => p.PrizeType == type);
        }
        public static int GetCurrencyCount(this IEnumerable<Prize> prizes, Currency.Type type)
        {
            if (prizes == null) return 0;
            int v = prizes.Where((p) => p.PrizeType == Item.Type.currency && p.Name == type.ToString()).Sum((p) => p.Parameter);
            return v > 0 ? v : 0;

        }
    }
}