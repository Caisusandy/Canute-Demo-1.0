using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public class Legion
    {
        public List<UUID> armiesUUID;
        public List<ArmyItem> Armies => Game.PlayerData.GetArmyItems(armiesUUID.ToArray());

        public bool HasArmyItem(ArmyItem armyItem)
        {
            return armiesUUID.Contains(armyItem.UUID);
        }

        public int IndexOf(ArmyItem armyItem)
        {
            if (armyItem is null)
            {
                throw new ArgumentNullException("Cannot get an null army item");
            }
            return armiesUUID.IndexOf(armyItem.UUID);
        }

        public void Replace(ArmyItem original, ArmyItem replaceTo)
        {
            if (armiesUUID.Count < 5)
            {
                for (int i = armiesUUID.Count; i < 5; i++)
                {
                    armiesUUID.Add(UUID.Empty);
                }
            }

            if (HasArmyItem(replaceTo)) //switch
            {
                int oIndex = armiesUUID.IndexOf(original.UUID);
                int fIndex = armiesUUID.IndexOf(replaceTo.UUID);

                if (oIndex == -1)
                {
                    armiesUUID[fIndex] = replaceTo.UUID;    //Reasign from empty
                }
                else
                {
                    armiesUUID[oIndex] = replaceTo.UUID;
                    armiesUUID[fIndex] = original.UUID;
                }
            }
            else
            {
                int index = IndexOf(original);
                if (index == -1)
                {
                    for (int i = 0; i < armiesUUID.Count; i++)
                    {
                        if (armiesUUID[i] == UUID.Empty)
                        {
                            index = i;
                        }
                    }
                    if (index == -1)
                        return;
                }

                if (replaceTo is null)
                {
                    armiesUUID[index] = UUID.Empty;
                }
                else
                {
                    armiesUUID[index] = replaceTo.UUID;
                }
            }
        }

        public void SetArmy(int index, ArmyItem armyItem)
        {
            if (armyItem is null)
            {
                throw new ArgumentNullException("Cannot set an null army item");
            }
            Debug.Log(index);
            armiesUUID[index] = armyItem.UUID;
        }

        public void Left(ArmyItem armyItem)
        {
            for (int i = 0; i < armiesUUID.Count; i++)
            {
                if (armiesUUID[i] == armyItem.UUID)
                {
                    armiesUUID[i] = UUID.Empty;
                }
            }
        }
    }

    [Serializable]
    public class EventCardPile : IEnumerable<EventCardItem>
    {
        public const int CardLimit = 24;
        public List<UUID> eventCardUUID = new List<UUID>();

        public List<EventCardItem> EventCards => Game.PlayerData.GetEventCardItems(eventCardUUID.ToArray());
        public int CardCount => GetCardCount();

        public bool HasEventCardItem(EventCardItem eventCardItem)
        {
            return eventCardUUID.Contains(eventCardItem.UUID);
        }

        public void Left(EventCardItem eventCardItem)
        {
            eventCardUUID.Remove(eventCardItem.UUID);
            PlayerFile.SaveCurrentData();
        }

        public bool Add(EventCardItem eventCardItem)
        {
            if (CardCount + eventCardItem.Prototype.Count > CardLimit)
            {
                Debug.Log(CardCount);
                return false;
            }

            if (eventCardUUID.Contains(eventCardItem.UUID))
            {
                Debug.Log(CardCount);
                return false;
            }

            eventCardUUID.Add(eventCardItem.UUID);
            PlayerFile.SaveCurrentData();
            return true;
        }

        public IEnumerator<EventCardItem> GetEnumerator()
        {
            return ((IEnumerable<EventCardItem>)EventCards).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<EventCardItem>)EventCards).GetEnumerator();
        }

        private int GetCardCount()
        {
            int ans = 0;
            foreach (var item in EventCards)
            {
                ans += item.Prototype.Count;
            }
            return ans;
        }
    }
}

namespace Canute.BattleSystem
{
    public struct LegionSet
    {
        public string name;
        public Legion legion;
        public EventCardPile eventCardPile;
        public UUID leaderUUID;

        public LegionSet(Legion legion, EventCardPile eventCardPile, UUID leaderUUID, string name)
        {
            this.legion = legion;
            this.eventCardPile = eventCardPile;
            this.leaderUUID = leaderUUID;
            this.name = name;
        }

        public LeaderItem Leader => Game.PlayerData.GetLeaderItem(leaderUUID);


    }
}