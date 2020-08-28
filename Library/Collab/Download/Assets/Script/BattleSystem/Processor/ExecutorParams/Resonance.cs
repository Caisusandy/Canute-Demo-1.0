using Canute.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public class Resonance : Status
    {
        [Flags]
        public enum ResonanceTarget
        {
            selfType = 1,
            legion = 2,
            player = 4,
            landArmy = 8,
            airArmy = 16
        }

        public Resonance(Effect e) : base(e, -1, -1, StatType.resonance) { }
        public Resonance(Effect e, TriggerConditions tc) : base(e, -1, -1, StatType.resonance, tc) { }
        public Resonance(Effect e, TriggerCondition tc) : base(e, -1, -1, StatType.resonance, tc) { }

        public static List<Resonance> Get(Army.Types types, int count)
        {
            List<Resonance> resonances = new List<Resonance>();
            switch (types)
            {
                //    case Army.Types.shielder:
                //        if (count < 2)
                //        {
                //            Effect e = new Effect();
                //            e.SetSpecialName("shielderPenetrate");
                //            e.Parameter = 10;
                //            return new Resonance(e); 
                //        }
                default:
                    return null;
            }

        }

        public static void Resonate(List<BattleArmy> battleArmies)
        {
            Debug.Log(battleArmies.Count);
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

            foreach (var info in resonanceInfo)
            {
                foreach (var resonancePair in GameData.ResonanceSheet.GetResonance(info.Key, info.Value))
                {
                    Resonance resonance = resonancePair.Resonance;
                    switch (resonancePair.Target)
                    {
                        case ResonanceTarget.selfType:
                            foreach (var army in battleArmies)
                            {
                                if (army.Type != resonancePair.ArmyType)
                                {
                                    continue;
                                }
                                resonance.Effect.SetSource(army);
                                resonance.Effect.SetTarget(army);
                                army.StatList.Add(resonance);
                            }
                            break;
                        case ResonanceTarget.legion:
                            foreach (var army in battleArmies)
                            {
                                resonance.Effect.SetSource(army);
                                resonance.Effect.SetTarget(army);
                                army.StatList.Add(resonance);
                            }
                            break;
                        case ResonanceTarget.player:
                            resonance.Effect.SetSource(battleArmies[0].Owner);
                            resonance.Effect.SetTarget(battleArmies[0].Owner);
                            battleArmies[0].Owner.StatList.Add(resonance);
                            break;
                        case ResonanceTarget.landArmy:
                            foreach (var battleArmy in battleArmies)
                            {
                                if (battleArmy.StandPosition != BattleProperty.Position.land)
                                {
                                    continue;
                                }
                                resonance.Effect.SetSource(battleArmy);
                                resonance.Effect.SetTarget(battleArmy);
                                battleArmy.StatList.Add(resonance);
                            }
                            break;
                        case ResonanceTarget.airArmy:
                            foreach (var battleArmy in battleArmies)
                            {
                                if (battleArmy.StandPosition != BattleProperty.Position.air)
                                {
                                    continue;
                                }
                                resonance.Effect.SetSource(battleArmy);
                                resonance.Effect.SetTarget(battleArmy);
                                battleArmy.StatList.Add(resonance);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }
}

