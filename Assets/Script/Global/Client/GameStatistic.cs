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

        public List<string> ArmiesUnlocked => armiesUnlocked;
        public CheckList ArmiesUnlockedList => new CheckList(GameData.Prototypes.TestingArmies, ArmiesUnlocked);
        public List<string> LeadersUnlocked => leadersUnlocked;
        public CheckList LeadersUnlockedList => new CheckList(GameData.Prototypes.TestingEquipments, EquipmentsUnlocked);
        public List<string> EquipmentsUnlocked => equipmentsUnlocked;
        public CheckList EquipmentsUnlockedList => new CheckList(GameData.Prototypes.TestingLeaders, LeadersUnlocked);
    }
}