using Canute.BattleSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Canute
{
    [Serializable]
    public class ArmyItem : Item, IPrototypeCopy<Army>, ICareerLabled, IBattlePropertyContainer, IArmy
    {
        public const float LevelMultiple = 1.1f;
        public const int ExpBase = 100;
        public const int MaxArmyEquipmentCount = 2;
        public static ArmyItem Empty => new ArmyItem() { protoName = "Empty" };

        [SerializeField] protected int star = 1;
        [SerializeField] protected string LeaderName;
        [SerializeField] protected ArmyItemEquipmentSlot equipments;

        public Army Prototype { get => GameData.Prototypes.GetArmyPrototype(protoName); private set => protoName = value?.Name; }
        public override Prototype Proto => Prototype;
        public override int Level => GetLevel(ExpBase, LevelMultiple, Exp);
        public int NextLevelExp => (int)(ExpBase * Mathf.Pow(LevelMultiple, Level + 1) - ExpBase * Mathf.Pow(LevelMultiple, Level));
        public override Type ItemType => Item.Type.army;

        /// <summary> Star(1,2,3) </summary>
        public int Star => star < 1 ? 1 : star > 3 ? 3 : star;
        public bool HasLeader => !(Leader is null);
        public LeaderItem Leader { get => Game.PlayerData.GetLeaderItem(LeaderName); set => LeaderName = value?.Name; }
        public ArmyItemEquipmentSlot Equipments => equipments ?? (equipments = new ArmyItemEquipmentSlot());


        #region Properties
        /// <summary>
        /// bonus of level and star
        /// </summary>
        public double LevelBonus => Mathf.Pow(1.3f, Star - 1) * Mathf.Pow(1.005f, Level) * 100; //2.2795
        public BattleProperty BaseProperty => Prototype.Properties[Star - 1];


        public int MaxHealth => Prototype.Health.Bonus(LevelBonus);
        public int RawDamage => Prototype.Damage.Bonus(LevelBonus);
        public int Defense => Properties.Defense.Bonus(LevelBonus);

        public new Army.Types Type => Prototype.Type;
        public Career Career => HasLeader ? Leader.Career : Prototype.Career;



        #region Army Properties
        public BattleProperty Properties => new BattleProperty(this);
        #endregion


        #region Properties For display before battle
        public BattleProperty PropertiesAfterEquipment => new BattleProperty(this, equipments);
        public int DefenseForDisplay => (int)PropertiesAfterEquipment.Defense;
        public int MaxHealthForDisplay => Prototype.Health.Bonus(LevelBonus).Bonus(equipments.GetBonusOf(PropertyType.health, BonusType.additive), BonusType.additive).Bonus(equipments.GetBonusOf(PropertyType.health, BonusType.percentage), BonusType.percentage);
        public int DamageForDisplay => Prototype.Damage.Bonus(LevelBonus).Bonus(equipments.GetBonusOf(PropertyType.damage, BonusType.additive), BonusType.additive).Bonus(equipments.GetBonusOf(PropertyType.damage, BonusType.percentage), BonusType.percentage);

        #endregion


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

        public ArmyItemEquipmentSlot()
        {
            InitializeEquipmentSlot();
        }

        public List<EquipmentItem> Equipments => GetEquipments();
        public EquipmentItem this[int index]
        {
            get => GetEquipment(index);
            set => SetEquipment(index, value);
        }

        private EquipmentItem GetEquipment(int index)
        {
            if (equipmentUUID.Count != 3)
            {
                InitializeEquipmentSlot();
            }
            return Game.PlayerData.GetEquipmentItem(equipmentUUID[index]);
        }

        private void SetEquipment(int index, EquipmentItem value)
        {
            //if (equipmentUUID is null || equipmentLimit is null)
            //{
            //    InitializeEquipmentSlot();
            //}
            //if (equipmentUUID.Count != 3 || equipmentLimit.Count != 3)
            //{
            //    InitializeEquipmentSlot();
            //}
            //if ((equipmentLimit[index] & value.Prototype.Type) == Equipment.EquipmentType.none)
            //{
            //    return;
            //}
            if (value)
                equipmentUUID[index] = value.UUID;
            else
                equipmentUUID[index] = UUID.Empty;
        }

        private void InitializeEquipmentSlot()
        {
            equipmentUUID = new List<UUID> { Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid() };
            equipmentLimit = new List<Equipment.EquipmentType>() { Equipment.EquipmentType.any, Equipment.EquipmentType.any, Equipment.EquipmentType.any };
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
                EquipmentItem item = Game.PlayerData.GetEquipmentItem(uuid);
                if (item)
                    equipmentItems.Add(item);
            }
            return equipmentItems;
        }

        public int GetBonusOf(PropertyType type, BonusType bounesTypes)
        {
            int bounes = 0;
            foreach (var equipmentItem in Equipments)
            {
                foreach (var bounesPack in equipmentItem.Bonus)
                {
                    if (bounesPack.BonusType == bounesTypes && bounesPack.Type == type)
                    {
                        bounes += bounesPack.GetValue(equipmentItem.Level);
                    }
                }
            }
            return bounes;
        }

    }
}


