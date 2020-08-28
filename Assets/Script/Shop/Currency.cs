using Canute.Languages;
using System;

namespace Canute.Shops
{
    [Serializable]
    public struct Currency
    {
        public enum Type
        {
            fedgram,
            manpower,
            Aethium,
            mantleAlloy,
        }

        public Type name;
        public int count;

        public Currency(Type type, int count)
        {
            this.name = type;
            this.count = count;
        }

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
