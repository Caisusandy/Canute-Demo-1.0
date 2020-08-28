using System.Collections.Generic;

namespace Canute.BattleSystem
{
    public class Resonance : Status
    {
        public Resonance(Effect e) : base(e, -1, -1) { }
        public Resonance(Effect e, TriggerConditions tc) : base(e, -1, -1, tc) { }
        public Resonance(Effect e, TriggerCondition tc) : base(e, -1, -1, tc) { }

        public static Resonance Get(Army.Types types, int count)
        {

            switch (types)
            {
                case Army.Types.infantry:
                    if (count < 2)
                    {
                        return new Resonance(new Effect(Effect.Types.damageIncreasePercentage, 1, 20), TriggerCondition.OnAttack);
                    }
                    else
                    {
                        return new Resonance(new Effect(Effect.Types.damageIncreasePercentage, 1, 30), TriggerCondition.OnAttack);
                    }

                case Army.Types.rifleman:
                    if (count < 2)
                    {
                        return new Resonance(new Effect());
                    }
                    else
                    {
                        return new Resonance(new Effect());
                    }

                case Army.Types.sapper:
                    if (count < 2)
                    {
                        return new Resonance(new Effect());
                    }
                    else
                    {
                        return new Resonance(new Effect());
                    }

                case Army.Types.cavalry:
                    if (count < 2)
                    {
                        return new Resonance(new Effect());
                    }
                    else
                    {
                        return new Resonance(new Effect());
                    }

                case Army.Types.scout:
                    if (count < 2)
                    {
                        return new Resonance(new Effect());
                    }
                    else
                    {
                        return new Resonance(new Effect());
                    }

                case Army.Types.warMachine:
                    if (count < 2)
                    {
                        return new Resonance(new Effect());
                    }
                    else
                    {
                        return new Resonance(new Effect());
                    }

                case Army.Types.airship:
                    if (count < 2)
                    {
                        return new Resonance(new Effect());
                    }
                    else
                    {
                        return new Resonance(new Effect());
                    }

                case Army.Types.shielder:
                    if (count < 2)
                    {
                        Effect e = new Effect();
                        e.SetSpecialName("shielderPenetrate");
                        e.Parameter = 10;
                        return new Resonance(e);
                    }
                    else
                    {
                        return new Resonance(new Effect());
                    }

                case Army.Types.aircraftFighter:
                    if (count < 2)
                    {
                        return new Resonance(new Effect());
                    }
                    else
                    {
                        return new Resonance(new Effect());
                    }

                case Army.Types.dragon:
                    if (count < 2)
                    {
                        return new Resonance(new Effect());
                    }
                    else
                    {
                        return new Resonance(new Effect());
                    }

                case Army.Types.mage:
                    if (count < 2)
                    {
                        return new Resonance(new Effect());
                    }
                    else
                    {
                        return new Resonance(new Effect());
                    }

                default:
                    return null;
            }

        }

        public static void Resonate(ref List<BattleArmy> battleArmies)
        {
            Dictionary<Army.Types, int> resonanceInfo = new Dictionary<Army.Types, int>();

            foreach (BattleArmy army in battleArmies)
            {
                if (resonanceInfo.ContainsKey(army.Type))
                {
                    resonanceInfo[army.Type] += army.Properties.Pop;
                }
                else
                {
                    resonanceInfo.Add(army.Type, army.Properties.Pop);
                }
            }

            foreach (BattleArmy army in battleArmies)
            {
                army.StatList.Add(Get(army.Type, resonanceInfo[army.Type]));
            }
        }
    }
}

