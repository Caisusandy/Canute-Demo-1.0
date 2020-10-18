using System;

namespace Canute.Shops
{
    [Serializable]
    public struct Currency
    {
        public enum Type
        {
            [Obsolete]
            fedgram,
            [Obsolete]
            manpower,
            [Obsolete]
            aethium,
            [Obsolete]
            mantleAlloy,
        }

        public Type name;
        public int count;

        [Obsolete]
        public Currency(Type type, int count)
        {
            this.name = type;
            this.count = count;
        }

        [Obsolete]
        public static implicit operator int(Currency currency)
        {
            return currency.count;
        }
        public override string ToString()
        {
            return name.Lang() + ": " + count;
        }
    }
}
