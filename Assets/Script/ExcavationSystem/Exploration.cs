using Canute.BattleSystem;
using Canute.ExplorationSystem;
using Canute.Shops;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.ExplorationSystem
{
    public static class Exploration
    {
        public static void GoOut(ExplorationTeam team, Fund fund)
        {
            List<PrizeBox> boxes = new List<PrizeBox>();
            ConditionalPrize prize;
            int count = team.Leaders.Count;
            for (int i = 0; i < (count > 4 ? 4 : count); i++)
            {
                #region EnemyFaceChance
                double chanceFaceEnemy = 0.5 - 0.1 * team.Army.Star - 0.05 * (int)team.Army.Rarity;
                if (UnityEngine.Random.value < chanceFaceEnemy)
                {
                    bool win = ArmyMatch(team.Army);
                    if (!win)
                    {
                        prize = WeightItem.Get(UnityEngine.Random.value, GameData.ExcavationPrice.FailedPrize.ToArray());
                        boxes.Add(new PrizeBox(prize));
                        continue;
                    }
                }

                #endregion

                float range = UnityEngine.Random.value;
                if (range < 0.3)
                {
                    //story
                    prize = StoryPrize(team);
                    boxes.Add(new PrizeBox(prize));

                }
                else if (range < 0.6)
                {
                    //Item Prize
                    #region Rarity
                    WeightItem common = new WeightItem((int)(4500 - 2500 * fund.ToFull), "common");
                    WeightItem rare = new WeightItem((int)(3500 + 1000 * fund.ToFull), "rare");
                    WeightItem epic = new WeightItem((int)(1500 + 1000 * fund.ToFull), "epic");
                    WeightItem legnedary = new WeightItem((int)(500 + 500 * fund.ToFull), "legendary");
                    IWeightable actualRarity = WeightItem.Get(UnityEngine.Random.value, common, rare, epic, legnedary);
                    Rarity rarity = (Rarity)Enum.Parse(typeof(Rarity), actualRarity.Name);
                    #endregion 
                    prize = GetPrize(team, fund, rarity);
                    boxes.Add(new PrizeBox(prize));
                }
                else
                {
                    //useless Prize
                    prize = WeightItem.Get(UnityEngine.Random.value, GameData.ExcavationPrice.UselessPrize.ToArray());
                    boxes.Add(new PrizeBox(prize));
                }
            }

            PrizeBoxes prizeBoxes = new PrizeBoxes(boxes);
            team.CurrentPrize = prizeBoxes;
            PlayerFile.SaveCurrentData();
        }

        private static bool ArmyMatch(ArmyItem army)
        {
            int A = UnityEngine.Random.Range(8, 48);
            int H = (int)(25 * Mathf.Pow(Mathf.Sqrt(6), UnityEngine.Random.value * 2));
            int D = UnityEngine.Random.Range(0, 20);

            int armyA = army.RawDamage;
            int armyH = army.MaxHealth;
            int armyD = army.Defense;

            while (true)
            {
                int damage = armyA - D;
                if (damage < 0) damage = 1;
                H -= damage;

                if (H <= 0)
                {
                    return true;
                }


                damage = A - armyD;
                if (damage < 0) damage = 1;
                armyH -= damage;

                if (armyH <= 0)
                {
                    return false;
                }
            }
        }

        private static ConditionalPrize StoryPrize(ExplorationTeam team)
        {
            Story:
            while (true)
            {
                StoryPrize story = WeightItem.Get(UnityEngine.Random.value, GameData.ExcavationPrice.StoryPrizes.ToArray());
                if (story.LeaderRequirement != null)
                    foreach (var name in story.LeaderRequirement)
                    {
                        if (team.Leaders.Where(leader => leader.Name == name).Count() == 0)
                        {
                            goto Story;
                        }
                    }
                if (team.Army.Name != story.ArmyRequirement && !string.IsNullOrEmpty(story.ArmyRequirement))
                {
                    continue;
                }
                return story;
            }
        }

        private static ConditionalPrize GetPrize(ExplorationTeam team, Fund fund, Rarity rarity)
        {
            Debug.Log(rarity);
            Re:
            IWeightable prize = WeightItem.Get(UnityEngine.Random.value, fund.Equipment, fund.Army, fund.Leader);
            IEnumerable<ItemPrize> possiblePrize;
            switch (prize.Name)
            {
                case "Army":
                    Army:
                    possiblePrize = GameData.ExcavationPrice.ArmyPrizes.Where(p => p.Rarity == rarity);
                    //Debug.Log(possiblePrize.Count());
                    while (true)
                    {
                        ItemPrize armyPrize = WeightItem.Get(UnityEngine.Random.value, possiblePrize.ToArray());
                        //Debug.Log(armyPrize);
                        if (armyPrize.LeaderRequirement != null)
                            foreach (var name in armyPrize.LeaderRequirement)
                            {
                                if (team.Leaders.Where(leader => leader.Name == name).Count() == 0)
                                {
                                    goto Re;
                                }
                            }
                        if (team.Army.Name != armyPrize.ArmyRequirement && !string.IsNullOrEmpty(armyPrize.ArmyRequirement))
                        {
                            continue;
                        }
                        return armyPrize;
                    }
                case "Equipment":
                    possiblePrize = GameData.ExcavationPrice.EquipmentPrizes.Where(p => p.Rarity == rarity);
                    //Debug.Log(possiblePrize.Count());
                    Equipment:
                    while (true)
                    {
                        ItemPrize equipment = WeightItem.Get(UnityEngine.Random.value, possiblePrize.ToArray());
                        //Debug.Log(equipment);
                        if (equipment.LeaderRequirement != null)
                            foreach (var name in equipment.LeaderRequirement)
                            {
                                if (team.Leaders.Where(leader => leader.Name == name).Count() == 0)
                                {
                                    goto Re;
                                }
                            }
                        if (team.Army.Name != equipment.ArmyRequirement && !string.IsNullOrEmpty(equipment.ArmyRequirement))
                        {
                            continue;
                        }
                        return equipment;
                    }
                case "Leader":
                    possiblePrize = GameData.ExcavationPrice.LeaderPrizes.Where(p => p.Rarity == rarity && !Game.PlayerData.IsLeaderUnlocked(p.Name));

                    Debug.Log("LeaderPrizes amount: " + GameData.ExcavationPrice.LeaderPrizes.Count);
                    foreach (var item in GameData.ExcavationPrice.LeaderPrizes)
                    {
                        Debug.Log(item.Name + " " + item.Rarity + ", " + !Game.PlayerData.IsLeaderUnlocked(item.Name));
                    }// Debug.Log(possiblePrize.Count());
                    Leader:
                    while (true)
                    {  //if (possiblePrize.Count() == 0)

                        ItemPrize leaderPrize = WeightItem.Get(UnityEngine.Random.value, possiblePrize.ToArray());
                        //Debug.Log(leaderPrize);
                        if (leaderPrize.LeaderRequirement != null)
                            foreach (var name in leaderPrize.LeaderRequirement)
                            {
                                if (team.Leaders.Where(leader => leader.Name == name).Count() == 0)
                                {
                                    goto Re;
                                }
                            }
                        if (team.Army.Name != leaderPrize.ArmyRequirement && !string.IsNullOrEmpty(leaderPrize.ArmyRequirement))
                        {
                            continue;
                        }
                        return leaderPrize;
                    }
            }
            return null;
        }
    }

    [Serializable]
    public class PrizeBoxes : IEnumerable<PrizeBox>
    {
        public List<PrizeBox> prizeBoxes;

        public void OpenNow()
        {
            if (prizeBoxes.Count > 0)
            {
                if (prizeBoxes[0].ToReadyTime.TotalSeconds == 0)
                {
                    prizeBoxes[0].prize.Fulfill();
                    prizeBoxes.RemoveAt(0);
                }
            }
            PlayerFile.SaveCurrentData();
        }

        public IEnumerator<PrizeBox> GetEnumerator()
        {
            return ((IEnumerable<PrizeBox>)prizeBoxes).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<PrizeBox>)prizeBoxes).GetEnumerator();
        }

        public PrizeBoxes(List<PrizeBox> prizeBoxes)
        {
            this.prizeBoxes = prizeBoxes;
            if (prizeBoxes.Count == 0)
            {
                return;
            }
            prizeBoxes[0].startingTime = DateTime.Now;
            if (prizeBoxes.Count == 1)
            {
                return;
            }
            for (int i = 1; i < prizeBoxes.Count; i++)
            {
                prizeBoxes[i].startingTime = DateTime.Now + prizeBoxes[i - 1].ToReadyTime;
            }
        }
    }

    [Serializable]
    public class PrizeBox
    {
        public WorldTime startingTime;
        public ConditionalPrize prize;

        public PrizeBox(ConditionalPrize prize)
        {
            this.prize = prize;
        }

        public TimeSpan ToReadyTime
        {
            get
            {
                try
                {
                    TimeSpan timeSpan = ((DateTime)startingTime).Add(prize.Time) - DateTime.Now;
                    if (timeSpan > TimeSpan.Zero)
                    {

                        return timeSpan;
                    }
                }
                catch { }
                return TimeSpan.Zero;
            }
        }
    }
}

namespace Canute.ExplorationSystem
{

    public struct Fund
    {
        public const int maxFedgram = 1000;
        public const int maxManpower = 1000;
        public const int maxMantleAlloy = 10;

        private int fedgram;
        private int manpower;
        private int mantleAlloy;

        public int Fedgram { get => fedgram; set => fedgram = value > maxFedgram ? maxFedgram : value; }
        public int Manpower { get => manpower; set => manpower = value > maxManpower ? maxManpower : value; }
        public int MantleAlloy { get => mantleAlloy; set => mantleAlloy = value > maxMantleAlloy ? maxMantleAlloy : value; }

        public Fund(int fedgram, int manpower, int mantleAlloy)
        {
            this.fedgram = fedgram;
            this.manpower = manpower;
            this.mantleAlloy = mantleAlloy;
        }

        public WeightItem Army => new WeightItem(manpower * 1000 / maxManpower, "Army");
        public WeightItem Leader => new WeightItem(fedgram * 1000 / maxFedgram, "Leader");
        public WeightItem Equipment => new WeightItem(mantleAlloy * 1000 / maxMantleAlloy, "Equipment");

        public float ToFull => (manpower * 1f / maxManpower + fedgram * 1f / maxFedgram + mantleAlloy * 1f / maxMantleAlloy) / 3f;
    }

    [Serializable]
    public class ExplorationTeam
    {
        [SerializeField] private List<UUID> leadersUUID;
        [SerializeField] private UUID armyUUID;
        [SerializeField] private PrizeBoxes currentPrize;

        public TimeSpan ToBackTime => GetBackTime();
        public List<UUID> LeadersUUID { get => leadersUUID; set => leadersUUID = value; }
        public List<LeaderItem> Leaders => GetLeader();
        public UUID ArmyUUID { get => armyUUID; set => armyUUID = value; }
        public ArmyItem Army => Game.PlayerData.GetArmyItem(armyUUID);
        public List<LeaderItem> RealLeader
        {
            get
            {
                var leaders = Leaders;
                for (int i = leaders.Count - 1; i >= 0; i--)
                {
                    LeaderItem item = leaders[i];
                    if (!item)
                    {
                        leaders.RemoveAt(i);
                    }
                }
                return leaders;
            }
        }
        public PrizeBoxes CurrentPrize { get => currentPrize; set => currentPrize = value; }


        public ExplorationTeam()
        {
        }

        private List<LeaderItem> GetLeader()
        {
            if (leadersUUID is null)
            {
                leadersUUID = new List<UUID>() { UUID.Empty, UUID.Empty, UUID.Empty, UUID.Empty };
            }
            if (leadersUUID.Count != 4)
            {
                leadersUUID = new List<UUID>() { UUID.Empty, UUID.Empty, UUID.Empty, UUID.Empty };
            }
            return Game.PlayerData.GetLeaderItems(leadersUUID.ToArray());
        }

        public bool HasLeaderItem(LeaderItem armyItem)
        {
            return leadersUUID.Contains(armyItem.UUID);
        }

        public int IndexOf(LeaderItem armyItem)
        {
            if (armyItem is null)
            {
                throw new ArgumentNullException("Cannot get an null army item");
            }
            return leadersUUID.IndexOf(armyItem.UUID);
        }

        public void Replace(LeaderItem original, LeaderItem replaceTo)
        {
            if (leadersUUID.Count < 5)
            {
                for (int i = leadersUUID.Count; i < 5; i++)
                {
                    leadersUUID.Add(UUID.Empty);
                }
            }

            if (HasLeaderItem(replaceTo)) //switch
            {
                int oIndex = leadersUUID.IndexOf(original.UUID);
                int fIndex = leadersUUID.IndexOf(replaceTo.UUID);

                if (oIndex == -1)
                {
                    leadersUUID[fIndex] = replaceTo.UUID;    //Reasign from empty
                }
                else
                {
                    leadersUUID[oIndex] = replaceTo.UUID;
                    leadersUUID[fIndex] = original.UUID;
                }
            }
            else
            {
                int index = IndexOf(original);
                if (index == -1)
                {
                    for (int i = 0; i < leadersUUID.Count; i++)
                    {
                        if (leadersUUID[i] == UUID.Empty)
                        {
                            index = i;
                        }
                    }
                    if (index == -1)
                        return;
                }

                if (replaceTo is null)
                {
                    leadersUUID[index] = UUID.Empty;
                }
                else
                {
                    leadersUUID[index] = replaceTo.UUID;
                }
            }
            Reorganize();
        }

        public void SetLeader(int index, LeaderItem leaderItem)
        {
            if (leaderItem is null)
            {
                throw new ArgumentNullException("Cannot set an null army item");
            }
            Debug.Log(index);
            leadersUUID[index] = leaderItem.UUID;
            Reorganize();
        }

        public void Left(LeaderItem armyItem)
        {
            for (int i = 0; i < leadersUUID.Count; i++)
            {
                if (leadersUUID[i] == armyItem.UUID)
                {
                    leadersUUID[i] = UUID.Empty;
                }
            }
            Reorganize();
        }

        private void Reorganize()
        {
            for (int i = leadersUUID.Count - 1; i >= 0; i--)
            {
                if (!Game.PlayerData.GetLeaderItem(leadersUUID[i])) //item not exist
                {
                    leadersUUID.RemoveAt(i);
                    leadersUUID.Add(UUID.Empty);
                }
            }
        }

        private TimeSpan GetBackTime()
        {
            if (!(CurrentPrize is null)) if (!(CurrentPrize.prizeBoxes is null)) if (!(CurrentPrize.prizeBoxes.Count == 0))
                        return CurrentPrize.Last().ToReadyTime;
            return TimeSpan.Zero;
        }

    }
}