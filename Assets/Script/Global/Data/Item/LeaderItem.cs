using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public class LeaderItem : Item, IPrototypeCopy<Leader>, ICareerLabled, IBattleBounesItem
    {
        public override Prototype Proto => Prototype;
        public override int Level => GetLevel(100, 1.1f, Exp);
        public List<PropertyBounes> Bounes => Prototype.Bounes;
        public Leader Prototype { get => GameData.Prototypes.GetLeaderPrototype(protoName); private set => protoName = value?.Name; }
        public Career Career => Prototype.Career;


        public LeaderItem(Leader leader)
        {
            Prototype = leader;
        }
    }

    [Serializable]
    public class BattleLeader : OnMapEntityData, IEquatable<BattleLeader>, ICareerLabled, IBattleBounesItem
    {
        [SerializeField] protected Career career;
        [SerializeField] protected UUID leadingArmyUUID;
        [SerializeField] protected bool isViceCommander;
        [SerializeField] protected int level;
        [SerializeField] protected List<PropertyBounes> bounes = new List<PropertyBounes>();

        public BattleArmy LeadingArmy => Game.CurrentBattle.GetArmy(leadingArmyUUID);
        public OnMapEntityData ControllingEntityData => isViceCommander ? Owner.Campus.data : LeadingArmy as OnMapEntityData;

        public override Vector2Int Coordinate { get => ControllingEntityData.Coordinate; set { ControllingEntityData.x = value.x; ControllingEntityData.y = value.y; } }
        public override Vector3Int HexCoord => ControllingEntityData.HexCoord;
        public override bool AllowMove { get => ControllingEntityData.AllowMove; set => ControllingEntityData.AllowMove = value; }
        public override Cell OnCellOf => ControllingEntityData.OnCellOf;
        public Career Career { get => career; set => career = value; }
        public List<PropertyBounes> Bounes { get => bounes; set => bounes = value; }

        public static BattleLeader Empty => new BattleLeader();

        public int Level => level;

        public BattleLeader() : base() { }

        public BattleLeader(LeaderItem leaderItem) : this()
        {
            if (leaderItem == null)
            {
                return;
            }

            level = leaderItem.Level;
            name = leaderItem.Name;
            bounes = leaderItem.Bounes.Clone();
        }

        #region Bounes



        #endregion
        public bool Equals(BattleLeader other)
        {
            return other is null
                ? false
                : Bounes.Equals(other.Bounes) && name == other.name && career == other.career && (HasPrototype ? Prototype?.Equals(other.Prototype) == true : other.HasPrototype);
        }
    }
}