using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public class LeaderItem : Item, IPrototypeCopy<Leader>, ICareerLabled, IBattleBounesItem
    {
        public override Prototype Proto => Prototype;
        public override int Level => GetLevel(100, 1.1f, Exp);
        public override Type ItemType => Type.leader;
        public static LeaderItem Empty => new LeaderItem() { protoName = "Empty" };
        public List<PropertyBonus> Bonus => Prototype.Bounes;
        public Leader Prototype { get => GameData.Prototypes.GetLeaderPrototype(protoName); private set => protoName = value?.Name; }
        public Career Career => Prototype.Career;


        public LeaderItem()
        {
        }

        public LeaderItem(Leader leader)
        {
            Prototype = leader;
        }
    }
}

namespace Canute.BattleSystem
{
    [Serializable]
    public class BattleLeader : OnMapEntityData, IEquatable<BattleLeader>, ICareerLabled, IBattleBounesItem
    {
        [SerializeField] protected Career career;
        [SerializeField] protected UUID leadingArmyUUID;
        [SerializeField] protected bool isViceCommander;
        [SerializeField] protected int level;
        [SerializeField] protected List<PropertyBonus> bounes = new List<PropertyBonus>();

        public BattleArmy LeadingArmy => Game.CurrentBattle.GetArmy(leadingArmyUUID);
        public OnMapEntityData ControllingEntityData => isViceCommander ? null : LeadingArmy as OnMapEntityData;

        public override Vector2Int Coordinate { get => coordinate = ControllingEntityData.Coordinate; set { ControllingEntityData.x = value.x; ControllingEntityData.y = value.y; } }
        public override Vector3Int HexCoord => ControllingEntityData.HexCoord;
        public override bool AllowMove { get => ControllingEntityData.AllowMove; set => ControllingEntityData.AllowMove = value; }
        public override Cell OnCellOf => ControllingEntityData.OnCellOf;
        public Career Career { get => career; set => career = value; }
        public List<PropertyBonus> Bonus { get => bounes; set => bounes = value; }

        public static BattleLeader Empty => new BattleLeader();

        public int Level => level;

        public BattleLeader() : base() { }
        public BattleLeader(BattleLeader battleLeader) : base()
        {
            name = battleLeader.name;
            prefab = battleLeader.prefab;
            ownerUUID = battleLeader.ownerUUID;

            allowMove = false;
            Coordinate = battleLeader.Coordinate;
            leadingArmyUUID = battleLeader.leadingArmyUUID;
        }

        public BattleLeader(LeaderItem leaderItem) : this()
        {
            if (leaderItem == null)
            {
                return;
            }

            level = leaderItem.Level;
            name = leaderItem.Name;
            bounes = leaderItem.Bonus.Clone();
        }

        protected override string GetDisplayingName()
        {
            if (HasValidPrototype)
            {
                return base.GetDisplayingName();
            }
            else
            {
                return ("Canute.BattleSystem.Leader." + name + ".name").Lang();
            }
        }

        public void Lead(BattleArmy army)
        {
            if (army)
                leadingArmyUUID = army.UUID;
        }

        #region Bounes



        #endregion
        public bool Equals(BattleLeader other)
        {
            return other is null
                ? false
                : Bonus.Equals(other.Bonus) && name == other.name && career == other.career && (HasValidPrototype ? Prototype?.Equals(other.Prototype) == true : other.HasValidPrototype);
        }
    }
}