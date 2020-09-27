using System;
using System.Collections.Generic;

namespace Canute
{
    [Serializable]
    public class GameStatistic
    {
        public List<string> armiesUnlocked = new List<string>();
        public List<string> equipmentsUnlocked = new List<string>();
        public List<string> leadersUnlocked = new List<string>();
        public List<string> eventCardUnlocked = new List<string>();

        public List<string> ArmiesUnlocked => armiesUnlocked;
        public CheckList ArmiesUnlockedList => new CheckList(GameData.Prototypes.Armies, ArmiesUnlocked);
        public List<string> LeadersUnlocked => leadersUnlocked;
        public CheckList LeadersUnlockedList => new CheckList(GameData.Prototypes.Equipments, EquipmentsUnlocked);
        public List<string> EquipmentsUnlocked => equipmentsUnlocked;
        public CheckList EquipmentsUnlockedList => new CheckList(GameData.Prototypes.Leaders, LeadersUnlocked);
        public List<string> EventCardUnlocked => eventCardUnlocked;
        public CheckList EventCardUnlockedList => new CheckList(GameData.Prototypes.TestingEventCards, EventCardUnlocked);
    }
}