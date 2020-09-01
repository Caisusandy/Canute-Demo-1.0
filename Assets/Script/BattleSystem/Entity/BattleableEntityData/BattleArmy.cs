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

        public BattleArmy() : base()
        {
            allowMove = true;
        }

        /// <summary>
        /// clone a battleArmy and give it a different UUID
        /// </summary>
        /// <param name="army"></param>
        public BattleArmy(BattleArmy army) : base(army.Prototype)
        {
            name = army.name;
            prefab = army.prefab;
            ownerUUID = army.ownerUUID;

            allowMove = true;
            Coordinate = army.Coordinate;

            autonomousType = army.autonomousType;
            if (army.localLeader)
            {
                localLeader = new BattleLeader(army.localLeader);
                localLeader?.Lead(this);
            }

            maxHealth = army.maxHealth;
            damage = army.damage;
            properties = army.properties;
            properties.Defense = army.Defense;
            type = army.Type;
            career = army.Career;
            stats = army.stats.Clone() as StatusList;

            Health = army.Health;
        }
        /// <summary>
        /// clone a battleArmy and give it a different UUID
        /// </summary>
        /// <param name="army"></param>
        public BattleArmy(Army prototype, Player owner, int level, int star) : base(prototype)
        {
            ownerUUID = owner.UUID;

            var army = new ArmyItem(prototype, (int)(ArmyItem.ExpBase * Mathf.Pow(ArmyItem.LevelMultiple, level)));
            while (army.Star < star)
            {
                army.AddStar();
            }
            name = army.Name;
            maxHealth = army.MaxHealth;
            damage = army.MaxDamage;
            properties = army.Properties;
            properties.Defense = army.Defense;
            type = army.Type;
            career = army.Career;
            prefab = prototype.Prefab;
            health = maxHealth;
            allowMove = true;
        }

        private BattleArmy(ArmyItem army) : base(army.Prototype)
        {
            name = army.Name;
            maxHealth = army.MaxHealth;
            damage = army.MaxDamage;
            properties = army.Properties;
            properties.Defense = army.Defense;
            type = army.Type;
            career = army.Career;
            prefab = army.Prototype.Prefab;

            if (army.Leader)
            {
                localLeader = new BattleLeader(army.Leader);
                localLeader?.Lead(this);
                AddBounes(localLeader);
            }
            AddBounes(army.Equipments.Equipments.ToArray());
            health = maxHealth;
            allowMove = true;
        }

        /// <summary> create a battle army from a armyItem and a player</summary>
        /// <param name="army">Army Item</param>
        /// <param name="player">Owner of the army</param>
        public BattleArmy(ArmyItem army, Player player) : this(army)
        {
            ownerUUID = player.UUID;
            if (player.ViceCommander)
            {
                AddBounes(player.ViceCommander);
            }
            health = maxHealth;
        }

        public override object Clone()
        {
            return new BattleArmy(this);
        }


        protected override string GetDisplayingName()
        {
            if (HasValidPrototype)
            {
                return base.GetDisplayingName();
            }
            else
            {
                return ("Canute.BattleSystem.Army." + name + ".name").Lang();
            }
        }
    }
}