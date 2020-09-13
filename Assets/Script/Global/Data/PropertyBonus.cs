using System;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public struct PropertyBonus : IComparable<PropertyBonus>
    {
        [SerializeField] private PropertyType type;
        [SerializeField] private BonusType bounesType;
        [SerializeField] private float value;
        [SerializeField] private float growth;

        public PropertyType Type => type;
        public BonusType BonusType => bounesType;
        public int GetValue(int level) => (int)(Mathf.Pow(1 + growth / 100, level) * value);

        public static bool operator ==(PropertyBonus a, PropertyBonus b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(PropertyBonus a, PropertyBonus b)
        {
            return !(a == b);
        }

        public PropertyBonus(PropertyType type, BonusType bounesType, int value, float growth)
        {
            this.type = type;
            this.bounesType = bounesType;
            this.value = value;
            this.growth = growth;
        }

        public override bool Equals(object obj)
        {
            if (obj is PropertyBonus)
            {
                PropertyBonus equipmentProperty = (PropertyBonus)obj;
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

        public int Bonus(double @base, int level)
        {
            return @base.Bonus(GetValue(level), BonusType);
        }

        public void Bonus(ref double @base, int level)
        {
            @base = @base.Bonus(GetValue(level), BonusType);
        }

        public int RemoveBonus(double @base, int level)
        {
            return @base.RemoveBonus(GetValue(level), BonusType);
        }

        public void RemoveBonus(ref double @base, int level)
        {
            @base = @base.RemoveBonus(GetValue(level), BonusType);
        }

        public int CompareTo(PropertyBonus other)
        {
            if (other.BonusType == BonusType)
            {
                return other.GetValue(1) - GetValue(1);
            }
            else if (BonusType == BonusType.additive)
            {
                return -1;
            }
            return 1;
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
        critBonus = 64,

        pop = 128,
        armor = 256,
        anger = 512
    }


    public static class PropertyTypes
    {
        public static PropertyType[] Types => Enum.GetValues(typeof(PropertyType)) as PropertyType[];
    }

    //好像没用?
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