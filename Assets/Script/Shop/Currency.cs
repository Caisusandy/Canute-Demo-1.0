using System;

namespace Canute.Shops
{
    [Serializable]
    public struct Currency
    {
        public enum Type
        {
            federgram,
            manpower,
            MantleFluid,
            Aethium,
            mantleAlloy,
        }

        public Type type;
        public int count;

        public Currency(Type type, int count)
        {
            this.type = type;
            this.count = count;
        }

        public static implicit operator int(Currency currency)
        {
            return currency.count;
        }
    }
}
