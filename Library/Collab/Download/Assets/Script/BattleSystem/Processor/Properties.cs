using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Canute.BattleSystem
{
    public static partial class EffectExecute
    {


        #region Property
        public static BattleProperty GetProperty(IBattleableEntityData battleableEntity)
        {
            BattleProperty property = battleableEntity.RawProperties;

            foreach (var stat in battleableEntity.StatList.GetAllStatus(Effect.Types.propertyBonus))
            {
                for (int i = 0; i < stat.Effect.Count; i++)
                {
                    string type = stat.Effect[Effect.propertyBonusType];
                    if (!Enum.TryParse<PropertyType>(type, out _))
                    {
                        property = PropertyBonus(property, stat);
                    }
                    else property = SinglePropertyBonus(property, stat);
                }
            }
            foreach (var stat in battleableEntity.StatList.GetAllStatus(Effect.Types.propertyPanalty))
            {
                for (int i = 0; i < stat.Effect.Count; i++)
                {
                    string type = stat.Effect[Effect.propertyBonusType];
                    if (!Enum.TryParse<PropertyType>(type, out _))
                    {
                        property = PropertyPanalty(property, stat);
                    }
                    else property = SinglePropertyPanalty(property, stat);
                }
            }
            return property;
        }

        private static BattleProperty PropertyBonus(BattleProperty property, Status stat)
        {
            string type = stat.Effect[Effect.propertyBonusType];
            //Debug.Log(type);
            switch (type)
            {
                case "dragonDefense":
                    property.Defense *= 8;
                    break;
                case "dragonAttack":
                    property.AttackRange += 2;
                    property.MoveRange += 4;
                    break;
                case "dragonAttackBonus":
                    property.AttackRange += 1;
                    property.MoveRange -= 2;
                    break;
                case "dragonMoveBonus":
                    //Debug.Log("moveBonus");
                    property.MoveRange += 1;
                    property.AttackRange -= 2;
                    break;
                case "dragonAirAttack":
                    property.AttackPosition = BattleProperty.Position.air;
                    break;
                case "dragonCritAttack":
                    property.CritRate = property.CritRate.Bonus(100);
                    break;
                default:
                    break;
            }
            return property;
        }

        private static BattleProperty SinglePropertyBonus(BattleProperty property, Status stat)
        {
            PropertyType propertyType = stat.Effect.Args.GetEnumParam<PropertyType>(Effect.propertyBonusType);
            BonusType bounesType = stat.Effect.Args.GetEnumParam<BonusType>(Effect.bonusType);
            var checkTypeValues = Enum.GetValues(typeof(PropertyType));

            foreach (PropertyType item in checkTypeValues)
            {
                switch (propertyType & item)
                {
                    case PropertyType.defense:
                        property.Defense = property.Defense.Bonus(stat.Effect.Parameter, bounesType);
                        break;
                    case PropertyType.moveRange:
                        property.MoveRange = property.MoveRange.Bonus(stat.Effect.Parameter, bounesType);
                        break;
                    case PropertyType.attackRange:
                        property.AttackRange = property.AttackRange.Bonus(stat.Effect.Parameter, bounesType);
                        break;
                    case PropertyType.critRate:
                        property.CritRate = property.CritRate.Bonus(stat.Effect.Parameter, bounesType);
                        break;
                    case PropertyType.critBounes:
                        property.CritBonus = property.CritBonus.Bonus(stat.Effect.Parameter, bounesType);
                        break;
                    case PropertyType.pop:
                        property.Pop = property.Pop.Bonus(stat.Effect.Parameter, bounesType);
                        break;
                    default:
                        break;
                }
            }
            return property;
        }

        private static BattleProperty PropertyPanalty(BattleProperty property, Status stat)
        {
            string type = stat.Effect[Effect.propertyBonusType];
            switch (type)
            {
                case "dragonStrike":
                    property.AttackRange = 0;
                    break;
                default:
                    break;
            }
            return property;
        }

        private static BattleProperty SinglePropertyPanalty(BattleProperty property, Status stat)
        {
            PropertyType propertyType = stat.Effect.Args.GetEnumParam<PropertyType>(Effect.propertyBonusType);
            BonusType bounesType = stat.Effect.Args.GetEnumParam<BonusType>(Effect.bonusType);
            var checkTypeValues = Enum.GetValues(typeof(PropertyType));

            foreach (PropertyType item in checkTypeValues)
            {
                switch (propertyType & item)
                {
                    case PropertyType.moveRange:
                        property.MoveRange = property.MoveRange.Bonus(-stat.Effect.Parameter, bounesType);
                        break;
                    case PropertyType.attackRange:
                        property.AttackRange = property.AttackRange.Bonus(-stat.Effect.Parameter, bounesType);
                        break;
                    case PropertyType.critRate:
                        property.CritRate = property.CritRate.Bonus(-stat.Effect.Parameter, bounesType);
                        break;
                    case PropertyType.critBounes:
                        property.CritBonus = property.CritBonus.Bonus(-stat.Effect.Parameter, bounesType);
                        break;
                    case PropertyType.pop:
                        property.Pop = property.Pop.Bonus(-stat.Effect.Parameter, bounesType);
                        break;
                    default:
                        break;
                }
            }
            return property;
        }

        #endregion
    }
}
