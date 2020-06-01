using System;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public struct PropertyBounes
    {
        [SerializeField] private PropertyType type;
        [SerializeField] private BonusType bounesType;
        [SerializeField] private float value;
        [SerializeField] private float growth;

        public PropertyType Type => type;
        public BonusType BounesType => bounesType;
        public int GetValue(int level) => (int)(Mathf.Pow(1 + growth / 100, level) * value);

        public static bool operator ==(PropertyBounes a, PropertyBounes b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(PropertyBounes a, PropertyBounes b)
        {
            return !(a == b);
        }

        public PropertyBounes(PropertyType type, BonusType bounesType, int value, float growth)
        {
            this.type = type;
            this.bounesType = bounesType;
            this.value = value;
            this.growth = growth;
        }

        public override bool Equals(object obj)
        {
            if (obj is PropertyBounes)
            {
                PropertyBounes equipmentProperty = (PropertyBounes)obj;
                return equipmentProperty.bounesType == bounesType && value == equipmentProperty.value && type == equipmentProperty.type;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "type: " + type.ToString() + ";\n" +
                "bounes type: " + bounesType.ToString() + ";\n" +
                "value: " + value + ";\n" +
                "growth: " + growth;
        }

        public int Bounes(double @base, int level)
        {
            return @base.Bonus(GetValue(level), BounesType);
        }

        public void Bounes(ref double @base, int level)
        {
            @base = @base.Bonus(GetValue(level), BounesType);
        }

        public int RemoveBounes(double @base, int level)
        {
            return @base.RemoveBonus(GetValue(level), BounesType);
        }

        public void RemoveBounes(ref double @base, int level)
        {
            @base = @base.RemoveBonus(GetValue(level), BounesType);
        }
    }

    [Flags]
    public enum PropertyType
    {
        none,
        damage,
        health,
        defense = 4,

        moveRange = 8,
        attackRange = 16,

        critRate = 32,
        critBounes = 64,

        pop = 128,
    }


    public static class PropertyTypes
    {
        public static PropertyType[] Types => Enum.GetValues(typeof(PropertyType)) as PropertyType[];
        public static bool IsTypeOf(this PropertyType propertyType, PropertyType other)
        {
            return (propertyType & other) == other;
        }
    }

    [Serializable]
    public struct Percentage
    {
        [SerializeField] double value;

        public Percentage(double value) : this()
        {
            this.value = value;
        }

        public static Percentage operator *(Percentage a, Percentage b)
        {
            return new Percentage() { value = a.value * b.value };
        }
        public static Percentage operator +(Percentage a, Percentage b)
        {
            return new Percentage() { value = a.value + b.value };
        }
        public static Percentage operator -(Percentage a, Percentage b)
        {
            return new Percentage() { value = a.value - b.value };
        }
        public static Percentage operator /(Percentage a, Percentage b)
        {
            return new Percentage() { value = a.value / b.value };
        }

        public static implicit operator double(Percentage percentage)
        {
            return percentage.value;
        }

        public static implicit operator Percentage(double percentage)
        {
            return new Percentage(percentage);
        }

        public override bool Equals(object obj)
        {
            if (obj is Percentage)
            {
                return ((Percentage)obj).value == value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public static bool operator ==(Percentage left, Percentage right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Percentage left, Percentage right)
        {
            return !(left == right);
        }
    }

}