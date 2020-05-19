using Canute.Module;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Canute.BattleSystem
{
    [Serializable]
    public class Resonance : Status
    {
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

            foreach ((BattleArmy army, Resonance item) in from army in battleArmies
                                                          from item in GameData.ResonanceSheet.GetResonance(army.Type, resonanceInfo[army.Type])
                                                          select (army, item))
            {
                item.Effect.SetSource(army);
                item.Effect.SetTarget(army);
                army.StatList.Add(item);
            }
        }
    }
}

