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
        public enum ResonanceTarget
        {
            selfType,
            legion,
            player,
            global,
            landArmy,
            airArmy,
        }

        public Resonance(Effect e) : base(e, -1, -1, StatType.resonance) { }
        public Resonance(Effect e, TriggerConditions tc) : base(e, -1, -1, StatType.resonance, tc) { }
        public Resonance(Effect e, TriggerCondition tc) : base(e, -1, -1, StatType.resonance, tc) { }


        public static void ClearResonate(List<BattleArmy> battleArmies)
        {
            foreach (BattleArmy army in battleArmies)
            {
                for (int i = army.StatList.Count - 1; i >= 0; i++)
                {
                    if (army.StatList[i].IsResonance)
                    {
                        army.StatList.RemoveAt(i);
                    }
                }
            }
        }
        public static void Resonate(List<BattleArmy> battleArmies)
        {
            Debug.Log(battleArmies.Count);
            Dictionary<Army.Types, int> resonanceInfo = GetResonance(battleArmies);

            foreach (var info in resonanceInfo)
            {
                Debug.Log(info);
                foreach (var resonancePair in GameData.ResonanceSheet.GetResonance(info.Key, info.Value))
                {
                    Resonance resonance = resonancePair.Resonance;
                    //foreach (ResonanceTarget item in Enum.GetValues(typeof(ResonanceTarget)))
                    switch (resonancePair.Target)
                    {
                        case ResonanceTarget.selfType:
                            foreach (var army in battleArmies)
                            {
                                if (army.Type != resonancePair.ArmyType)
                                {
                                    continue;
                                }
                                AddResonance(army, resonance, army, army);
                            }
                            break;
                        case ResonanceTarget.legion:
                            foreach (var army in battleArmies)
                            {
                                AddResonance(army, resonance, army, army);
                            }
                            break;
                        case ResonanceTarget.player:
                            AddResonance(battleArmies[0].Owner, resonance, battleArmies[0].Owner, battleArmies[0].Owner);
                            break;
                        case ResonanceTarget.global:
                            foreach (var army in battleArmies)
                            {
                                AddResonance(Game.CurrentBattle, resonance, army, army);
                            }
                            break;
                        case ResonanceTarget.landArmy:
                            foreach (var army in battleArmies)
                            {
                                if (army.StandPosition != BattleProperty.Position.land)
                                {
                                    continue;
                                }
                                AddResonance(army, resonance, army, army);
                            }
                            break;
                        case ResonanceTarget.airArmy:
                            foreach (var army in battleArmies)
                            {
                                if (army.StandPosition != BattleProperty.Position.air)
                                {
                                    continue;
                                }
                                AddResonance(army, resonance, army, army);
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private static void AddResonance<T>(IStatusContainer reciever, Resonance resonance, T source, T target) where T : EntityData
        {
            var cloneResonance = resonance.Clone();
            cloneResonance.Effect.SetSource(source);
            cloneResonance.Effect.SetTarget(target);

            if ((cloneResonance.TriggerConditions.Count == 0) && (cloneResonance.Effect.Type == Effect.Types.@event))
            {
                cloneResonance.Effect.Execute();
                cloneResonance.Effect.Type = Effect.Types.tag;
            }
            reciever.StatList.Add(cloneResonance);
        }

        public static Dictionary<Army.Types, int> GetResonance(List<BattleArmy> battleArmies)
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

            return resonanceInfo;
        }
        public static Dictionary<Army.Types, int> GetResonance(List<ArmyItem> armyItem)
        {
            Dictionary<Army.Types, int> resonanceInfo = new Dictionary<Army.Types, int>();

            foreach (ArmyItem army in armyItem)
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

            return resonanceInfo;
        }
    }
}

