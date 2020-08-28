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
            standPosition.AddOptions(GenerateOptions<ArmyProperty.Position>());
            attackPosition.AddOptions(GenerateOptions<ArmyProperty.Position>());
            armyType.AddOptions(GenerateOptions<Army.Types>());
            attackType.AddOptions(GenerateOptions<ArmyProperty.AttackType>());

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



                    ArmyProperty property1 = army.Properties[0];
                    property1.CritRate = critRate;
                    property1.CritBounes = critBounes;
                    property1.MoveRange = moveRange;
                    property1.AttackRange = attackRange;

                    ArmyProperty property2 = army.Properties[1];
                    property2.CritRate = critRate;
                    property2.CritBounes = critBounes;
                    property2.MoveRange = moveRange;
                    property2.AttackRange = attackRange;

                    ArmyProperty property3 = army.Properties[2];
                    property3.CritRate = critRate;
                    property3.CritBounes = critBounes;
                    property3.MoveRange = moveRange;
                    property3.AttackRange = attackRange;


                    army = new Army(armyName.text, health, damage, defense, army.Type, army.Career, new List<ArmyProperty>() { property1, property2, property3 });
                }
                catch { }
                finally
                {
                }
        }
        public void SelectCareer(int value)
        {
            Career career = (Career)value;
            army = new Army(army.Name, army.Health, army.Damage, army.Defense, army.Type, career, army.Properties);
        }
        public void SelectArmyType(int value)
        {
            Army.Types type = (Army.Types)value;
            army = new Army(army.Name, army.Health, army.Damage, army.Defense, type, army.Career, army.Properties);
        }
        public void SelectArmyPosition(int value)
        {
            ArmyProperty.Position type = (ArmyProperty.Position)value;
            ArmyProperty property1 = army.Properties[0];
            property1.StandPosition = type;
            ArmyProperty property2 = army.Properties[1];
            property2.StandPosition = type;
            ArmyProperty property3 = army.Properties[2];
            property3.StandPosition = type;

            army = new Army(army.Name, army.Health, army.Damage, army.Defense, army.Type, army.Career, new List<ArmyProperty>() { property1, property2, property3 });
        }
        public void SelectAttackPosition(int value)
        {
            ArmyProperty.Position type = (ArmyProperty.Position)value;
            ArmyProperty property1 = army.Properties[0];
            property1.AttackPosition = type;
            ArmyProperty property2 = army.Properties[1];
            property2.AttackPosition = type;
            ArmyProperty property3 = army.Properties[2];
            property3.AttackPosition = type;

            army = new Army(army.Name, army.Health, army.Damage, army.Defense, army.Type, army.Career, new List<ArmyProperty>() { property1, property2, property3 });
        }
        public void SelectAttackType(int value)
        {
            ArmyProperty.AttackType type = (ArmyProperty.AttackType)Enum.Parse(typeof(ArmyProperty.AttackType), attackType.options[value].text);
            ArmyProperty property1 = army.Properties[0];
            property1.Attack = type;
            ArmyProperty property2 = army.Properties[1];
            property2.Attack = type;
            ArmyProperty property3 = army.Properties[2];
            property3.Attack = type;

            army = new Army(army.Name, army.Health, army.Damage, army.Defense, army.Type, army.Career, new List<ArmyProperty>() { property1, property2, property3 });
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
            }
        }
    }
}