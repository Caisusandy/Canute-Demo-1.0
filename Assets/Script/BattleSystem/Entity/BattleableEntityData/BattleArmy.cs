using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute.BattleSystem
{
    /// <summary>
    /// 战斗时的军队信息
    /// </summary>
    [Serializable]
    public class BattleArmy : BattleEntityData, ICloneable
    {
        [SerializeField] protected Army.Types type;
        public override GameObject Prefab { get => prefab ?? GameData.Prefabs.DefaultArmy; set => prefab = value; }
        public new ArmyEntity Entity => base.Entity as ArmyEntity;

        #region Properties
        public override Prototype Prototype { get => GameData.Prototypes.GetArmyPrototype(name); set => base.Prototype = value; }
        public Army.Types Type => type;
        #endregion

        private BattleArmy() : base()
        {
            allowMove = true;
        }

        /// <summary>
        /// clone a battleArmy and give it a different UUID
        /// </summary>
        /// <param name="army"></param>
        public BattleArmy(BattleArmy army) : base(army.Prototype)
        {
            ownerUUID = army.ownerUUID;

            maxHealth = army.maxHealth;
            damage = army.damage;
            defense = army.defense;
            type = army.Type;
            career = army.Career;
            skillPack = army.skillPack;
            properties = army.properties;
            prefab = army.prefab;
            Coordinate = army.Coordinate;
            stats = army.stats.Clone() as StatusList;

            Health = army.Health;
            allowMove = true;
        }

        private BattleArmy(ArmyItem army) : base(army.Prototype)
        {
            name = army.Name;
            localLeader = new BattleLeader(army.Leader);
            maxHealth = army.MaxHealth;
            damage = army.MaxDamage;
            defense = army.MaxDefense;
            type = army.Type;
            career = army.Career;
            properties = army.ArmyProperty;
            skillPack = army.SkillPack;
            prefab = army.Prototype.Prefab;
            AddBounes(army.Equipments.ToArray());
            AddBounes(localLeader);
            health = maxHealth;
            allowMove = true;
        }

        /// <summary> create a battle army from a armyItem and a player</summary>
        /// <param name="army">Army Item</param>
        /// <param name="player">Owner of the army</param>
        public BattleArmy(ArmyItem army, Player player) : this(army)
        {
            ownerUUID = player.UUID;
            AddBounes(player.ViceCommander);
            health = maxHealth;
        }

        public override object Clone()
        {
            return new BattleArmy(this);
        }
    }
}