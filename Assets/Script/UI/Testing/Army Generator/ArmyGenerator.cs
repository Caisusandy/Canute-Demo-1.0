using Canute.BattleSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Canute.Testing
{


    public class ArmyGenerator : MonoBehaviour
    {
        public InputField armyName;
        public InputField health;
        public InputField damage;
        public InputField defense;

        public InputField moveRange;
        public InputField attackRange;
        public InputField critRate;
        public InputField critBounes;

        public Dropdown career;
        public Dropdown armyType;
        public Dropdown attackType;
        public Dropdown attackPosition;
        public Dropdown standPosition;

        public Button CreateButton;

        public Army army;

        private void Awake()
        {
            career.AddOptions(GenerateOptions<Career>());
            standPosition.AddOptions(GenerateOptions<BattleProperty.Position>());
            attackPosition.AddOptions(GenerateOptions<BattleProperty.Position>());
            armyType.AddOptions(GenerateOptions<Army.Types>());
            attackType.AddOptions(GenerateOptions<BattleProperty.AttackType>());

            army = new Army();
        }

        public List<Dropdown.OptionData> GenerateOptions<T>() where T : Enum
        {
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (T item in Enum.GetValues(typeof(T)))
            {
                options.Add(new Dropdown.OptionData(item.ToString()));
            }
            return options;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            CreateButton.interactable = IsAllInfoFilled();
            if (IsAllInfoFilled())
                try
                {
                    int health = int.Parse(this.health.text);
                    int damage = int.Parse(this.damage.text);
                    int defense = int.Parse(this.defense.text);
                    int critRate = int.Parse(this.critRate.text);
                    int critBounes = int.Parse(this.critBounes.text);
                    int moveRange = int.Parse(this.moveRange.text);
                    int attackRange = int.Parse(this.attackRange.text);



                    BattleProperty property1 = army.Properties[0];
                    property1.CritRate = critRate;
                    property1.CritBonus = critBounes;
                    property1.MoveRange = moveRange;
                    property1.AttackRange = attackRange;
                    property1.Defense = defense;

                    BattleProperty property2 = army.Properties[1];
                    property2.CritRate = critRate;
                    property2.CritBonus = critBounes;
                    property2.MoveRange = moveRange;
                    property2.AttackRange = attackRange;
                    property2.Defense = defense;

                    BattleProperty property3 = army.Properties[2];
                    property3.CritRate = critRate;
                    property3.CritBonus = critBounes;
                    property3.MoveRange = moveRange;
                    property3.AttackRange = attackRange;
                    property3.Defense = defense;


                    army = new Army(armyName.text, health, damage, army.Type, army.Career, new List<BattleProperty>() { property1, property2, property3 });
                }
                catch { }
                finally
                {
                }
        }
        public void SelectCareer(int value)
        {
            Career career = (Career)value;
            army = new Army(army.Name, army.Health, army.Damage, army.Type, career, army.Properties);
        }
        public void SelectArmyType(int value)
        {
            Army.Types type = (Army.Types)value;
            army = new Army(army.Name, army.Health, army.Damage, type, army.Career, army.Properties);
        }
        public void SelectArmyPosition(int value)
        {
            BattleProperty.Position type = (BattleProperty.Position)value;
            BattleProperty property1 = army.Properties[0];
            property1.StandPosition = type;
            BattleProperty property2 = army.Properties[1];
            property2.StandPosition = type;
            BattleProperty property3 = army.Properties[2];
            property3.StandPosition = type;

            army = new Army(army.Name, army.Health, army.Damage, army.Type, army.Career, new List<BattleProperty>() { property1, property2, property3 });
        }
        public void SelectAttackPosition(int value)
        {
            BattleProperty.Position type = (BattleProperty.Position)value;
            BattleProperty property1 = army.Properties[0];
            property1.AttackPosition = type;
            BattleProperty property2 = army.Properties[1];
            property2.AttackPosition = type;
            BattleProperty property3 = army.Properties[2];
            property3.AttackPosition = type;

            army = new Army(army.Name, army.Health, army.Damage, army.Type, army.Career, new List<BattleProperty>() { property1, property2, property3 });
        }
        public void SelectAttackType(int value)
        {
            BattleProperty.AttackType type = (BattleProperty.AttackType)Enum.Parse(typeof(BattleProperty.AttackType), attackType.options[value].text);
            BattleProperty property1 = army.Properties[0];
            property1.Attack = type;
            BattleProperty property2 = army.Properties[1];
            property2.Attack = type;
            BattleProperty property3 = army.Properties[2];
            property3.Attack = type;

            army = new Army(army.Name, army.Health, army.Damage, army.Type, army.Career, new List<BattleProperty>() { property1, property2, property3 });
        }

        public void IsAnswerANumber(InputField input)
        {
            bool isVaild = false;
            try
            {
                int a = int.Parse(input.text);
                isVaild = true;
            }
            catch { }
            finally { input.textComponent.color = !isVaild ? Color.red : Color.black; }
        }

        public bool IsAllInfoFilled()
        {
            if (string.IsNullOrEmpty(armyName.text)) return false;
            if (string.IsNullOrEmpty(health.text)) return false;
            if (string.IsNullOrEmpty(damage.text)) return false;
            if (string.IsNullOrEmpty(defense.text)) return false;
            if (string.IsNullOrEmpty(critRate.text)) return false;
            if (string.IsNullOrEmpty(critBounes.text)) return false;
            if (string.IsNullOrEmpty(moveRange.text)) return false;
            if (string.IsNullOrEmpty(attackRange.text)) return false;

            return true;
        }

        public void CreateArmyPrototype()
        {
            PrototypeLoader.Export(army);
            GameData.Prototypes.TestingArmies.Add(army);

            foreach (var i in new int[4])
            {
                ArmyItem newArmy = new ArmyItem(army, 2500000);
                Game.PlayerData.AddArmyItem(newArmy);
                PlayerFile.SaveCurrentData();
            }
        }
    }
}