using Canute.BattleSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public class ArmyItem : Item, IPrototypeCopy<Army>, ICareerLabled
    {
        public const int MaxArmyEquipmentCount = 2;
        public static ArmyItem Empty => new ArmyItem() { protoName = "Empty" };

        [SerializeField] protected int star = 1;
        [SerializeField] protected string LeaderName;
        [SerializeField] protected ArmyItemEquipmentSlot equipments;

        public Army Prototype { get => GameData.Prototypes.GetArmyPrototype(protoName); private set => protoName = value?.Name; }
        public override Prototype Proto => Prototype;
        public override int Level => GetLevel(100, 1.1f, Exp);
        public override Type ItemType => Item.Type.Army;

        public int Star => star < 1 ? 1 : star > 3 ? 3 : star;
        public bool HasLeader => !(Leader is null);
        public LeaderItem Leader { get => Game.PlayerData.GetLeaderItem(LeaderName); set => LeaderName = value?.Name; }
        public ArmyItemEquipmentSlot Equipments => equipments;


        #region Properties
        /// <summary>
        /// bounes of level and star
        /// </summary>
        private double LevelBounes => Mathf.Pow(1.3f, Star - 1) * Mathf.Pow(1.005f, Level) * 100; //2.2795
        private BattleProperty PrototypeProperty => Prototype.Properties[Star - 1];


        public int MaxHealth => Prototype.Health.Bonus(LevelBounes);
        public int MaxDamage => Prototype.Damage.Bonus(LevelBounes);

        public new Army.Types Type => Prototype.Type;
        public Career Career => Prototype.Career;




        public int Defense => PrototypeProperty.Defense.Bonus(LevelBounes);
        public double CritRate => PrototypeProperty.CritRate;
        public double CritBounes => PrototypeProperty.CritBonus;
        public int AttackRange => PrototypeProperty.AttackRange;
        public int MoveRange => PrototypeProperty.MoveRange;
        public int Pop => PrototypeProperty.Pop;
        public BattleProperty.Position StandPosition => PrototypeProperty.StandPosition;
        public BattleProperty.Position AttackPosition => PrototypeProperty.AttackPosition;

        public int AttackArea => PrototypeProperty.AttackArea;
        public int TargetCount => PrototypeProperty.TargetCount;
        public BattleProperty.AttackType AttackType => PrototypeProperty.Attack;
        public HalfEffect SkillPack => PrototypeProperty.Skill;
        public ArgList Addition => PrototypeProperty.Addition;


        public BattleProperty ArmyProperty => new BattleProperty(this);



        #endregion


        #region Constructor
        public ArmyItem()
        {
            this.NewUUID();
        }
        public ArmyItem(Army army) : this()
        {
            Prototype = army;
        }

        public ArmyItem(Army army, int exp = 0) : this(army)
        {
            this.exp = exp;
        }


        #endregion 

        protected override Sprite GetIcon()
        {
            if (HasPrototype)
            {
                return Proto.Icon;
            }
            else
            {
                return GameData.SpriteLoader.Get(SpriteAtlases.armyIcon, protoName);
            }
        }

        protected override Sprite GetPortrait()
        {
            if (HasPrototype)
            {
                return Proto.Portrait;
            }
            else
            {
                return GameData.SpriteLoader.Get(SpriteAtlases.armyPortrait, protoName);
            }
        }



        public void AddFloatExp(int floatExp)
        {
            this.floatExp += floatExp;
        }
        public void AddExp(int exp)
        {
            this.exp += exp;
        }

        public void AddStar()
        {
            if (star < 3)
            {
                star++;
            }
        }


    }
    [Serializable]
    public class ArmyItemEquipmentSlot : IEnumerable<EquipmentItem>
    {
        public List<UUID> equipmentUUID;
        public List<Equipment.EquipmentType> equipmentLimit;

        public List<EquipmentItem> Equipments => GetEquipments();
        public EquipmentItem this[int index]
        {
            get => Equipments[index];
            set => SetEquipment(index, value);
        }

        private void SetEquipment(int index, EquipmentItem value)
        {
            if ((equipmentLimit[index] & value.Prototype.Type) == Equipment.EquipmentType.none)
            {
                return;
            }
            equipmentUUID[index] = value.UUID;
        }

        public IEnumerator<EquipmentItem> GetEnumerator()
        {
            return ((IEnumerable<EquipmentItem>)Equipments).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable<EquipmentItem>)Equipments).GetEnumerator();
        }

        private List<EquipmentItem> GetEquipments()
        {
            List<EquipmentItem> equipmentItems = new List<EquipmentItem>();
            foreach (UUID uuid in equipmentUUID)
            {
                equipmentItems.Add(Game.PlayerData.GetEquipmentItem(uuid));
            }
            return equipmentItems;
        }

        public int GetBounesOf(PropertyType type, BonusType bounesTypes)
        {
            int bounes = 0;
            foreach (var equipmentItem in Equipments)
            {
                foreach (var bounesPack in equipmentItem.Bounes)
                {
                    if (bounesPack.BounesType == bounesTypes && bounesPack.Type == type)
                    {
                        bounes += bounesPack.GetValue(equipmentItem.Level);
                    }
                }
            }
            return bounes;
        }

    }
}


