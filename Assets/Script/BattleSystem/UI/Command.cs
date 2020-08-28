using System;
using System.Collections.Generic;
using System.Linq;
using Canute;
using Canute.BattleSystem;
using Canute.StorySystem;
using UnityEngine;

namespace Canute.Testing
{
    public static class Command
    {
        /// <summary>
        /// Execute a command
        /// </summary>
        /// <param name="command">full string of the command <para>/<paramref name="command"/> [parameter: default value]</para></param>
        /// <returns></returns>
        public static bool Execute(string command)
        {
            if (!command.StartsWith("/"))
            {
                Console.WriteLine(command);
                return true;
            }

            string[] commandParams = command.Remove(0, 1).Split(',');

            try
            {
                switch (commandParams[0])
                {
                    //case "addStatus":
                    //    return AddStatus(commandParams);
                    case "battleStart":
                        return BattleStart();
                    case "createArmy":
                        return CreateArmy(commandParams);
                    case "execute":
                        return ExecuteEffect(commandParams);
                    case "findBattleEntity":
                        return FindBattleEntity(commandParams);
                    case "getArmyItem":
                        return GetArmyItem(commandParams);
                    case "getHexCoord":
                        return GetHexCoord();
                    case "getMapRadius":
                        return GetMapRadius();
                    case "getNeighbor":
                        return GetCellNeighbor();
                    case "getUUID":
                        return GetUUID();
                    case "kill":
                        return Kill(commandParams);
                    case "loadStory":
                        return LoadStory(commandParams);
                    case "openEmptyBattle":
                        return OpenEmptyBattle();
                    case "openBattle":
                        return OpenBattle(commandParams);
                    case "openTutorial":
                        return OpenTutorial();
                    case "print":
                        return Print(commandParams);
                    case "reloadResonance":
                        return ReloadResonance(commandParams);
                    case "reloadDefaultPrototype":
                        return ReloadDefaultPrototype();
                    case "reloadLanguagePack":
                        return ReloadLanguagePack();
                    case "save":
                        return Save();
                    case "setBattleStatus":
                        return BattleStatus(commandParams);
                    case "setProperty":
                        return SetProperty(commandParams);
                    case "arcefia":
                        return SecretlyAddAllItem();
                    default:
                        Console.WriteLine("Command not found: " + commandParams[0]);
                        break;
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                Debug.Log(e);
            }

            return false;
        }

        private static bool LoadStory(string[] commandParams)
        {
            string storyid = commandParams[1];
            if (Story.Get(storyid))
            {
                var story = Story.Get(storyid);
                StoryDisplayer.Load(story);
            }

            return true;
        }

        private static bool GetMapRadius()
        {
            Console.WriteLine(MapEntity.CurrentMap.MapRadius);
            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="commandParams"></param>
        /// <returns></returns>
        private static bool ReloadResonance(string[] commandParams)
        {
            Player owner = GetPlayerByName(commandParams[1]);
            if (owner is null)
            {
                Console.WriteLine("owner not found");
                return false;
            }

            Resonance.ClearResonate(owner.BattleArmies);
            Resonance.Resonate(owner.BattleArmies);

            return false;
        }

        private static bool GetCellNeighbor()
        {
            OnMapEntity onMapEntity = OnMapEntity.SelectingEntity;
            if (!HasSelectedOnMapEntity())
            {
                return false;
            }
            int count = onMapEntity.OnCellOf.NearByCells.Count;
            if (count == 0)
            {
                Console.WriteLine("No neibhbor cells are found");
                return true;
            }
            string ret = count + "Neighbor cell are :";
            foreach (var item in onMapEntity.OnCellOf.NearByCells)
            {
                ret += item.Coordinate.ToString();
            }
            Console.WriteLine(ret);
            return true;
        }
        private static bool GetHexCoord()
        {
            OnMapEntity onMapEntity = OnMapEntity.SelectingEntity;
            if (!HasSelectedOnMapEntity())
            {
                return false;
            }
            Console.WriteLine(onMapEntity.OnCellOf.HexCoord);
            return true;
        }

        private static bool BattleStatus(string[] commandParams)
        {
            Battle.Stat stat;
            if (!Enum.TryParse(commandParams[1], out stat))
            {
                return false;
            }

            var battle = Game.CurrentBattle;
            switch (stat)
            {
                case Battle.Stat.normal:
                    battle.TryInNormal();
                    break;
                case Battle.Stat.attack:
                    battle.InAttackAction();
                    break;
                case Battle.Stat.move:
                    battle.InMotionAction();
                    break;
                case Battle.Stat.waitForAnimationEnd:
                    battle.TryEndAnimation();
                    break;
                case Battle.Stat.win:
                    battle.Win();
                    break;
                case Battle.Stat.lose:
                    battle.Lost();
                    break;
                default:
                    break;
            }
            Console.WriteLine("Game Stat Changed");
            return true;
        }

        private static bool BattleStart()
        {
            foreach (var item in Game.CurrentBattle.AllPlayers)
            {
                Resonance.Resonate(item.BattleArmies);
            }
            if (Game.CurrentBattle.Round.CurrentStat != Round.Stat.gameStart)
                Game.CurrentBattle.Start();
            Console.WriteLine("Game Started");
            return true;
        }

        private static bool OpenBattle(string[] commandParams)
        {
            Game.LoadBattle(GameData.Chapters.ChapterTree.GetLevel(commandParams[1]), new LegionSet(Game.PlayerData.Legions[0], Game.PlayerData.EventCardPiles[0], Game.PlayerData.Leaders[0].UUID, Game.PlayerData.Leaders[0].Name));
            return true;
        }
        private static bool OpenTutorial()
        {
            Game.LoadBattle(GameData.Chapters.ChapterTree.GetLevel("Tutorial"), new LegionSet());
            return true;
        }
        private static bool OpenEmptyBattle()
        {
            Game.LoadBattle(GameData.Chapters.ChapterTree.GetLevel("Empty Battle"), new LegionSet());
            return true;
        }

        private static bool AddStatus(string[] commandParams)
        {
            if (Game.CurrentBattle is null)
            {
                Console.WriteLine("Command only available in battle");
            }
            /**
             * 1. string typeName
             * (@OnMapEntity target)
             * 2. UUID source
             * 3. int count
             * 4. int param
             * 5[] args
             * 
             */
            Effect.Types types; Enum.TryParse(commandParams[1], out types);
            var source = HasSelectedOnMapEntity().Exist() ?? HasSelectedCardEntity().Exist() as Entity ?? Game.CurrentBattle.Player.Entity;
            var target = Entity.Get(Guid.Parse(commandParams[2]));
            int count; ToInt(commandParams[3], out count);
            int param; ToInt(commandParams[4], out param);
            var args = commandParams.ToList();
            args.RemoveRange(0, 5);
            Arg[] args1 = args.Select((string a) => (Arg)a).ToArray();

            var effect = new Effect(types, source, target, count, param, args1);
            var s = effect.Execute();
            if (s)
                Console.WriteLine("Effect execute sucessful");
            else
                Console.WriteLine("Effect execute failed");
            return true;
        }

        /// <summary>
        /// 
        /// 1. @selectingEntity
        /// 2. string propertyType
        /// 3. double value
        /// </summary>
        /// <param name="commandParams"></param>
        /// <returns></returns>
        private static bool SetProperty(string[] commandParams)
        {
            var entity = OnMapEntity.SelectingEntity;

            PropertyType propertyType;
            if (!Enum.TryParse(commandParams[1], out propertyType))
            {
                Debug.Log("Property not found");
                return false;
            }

            double value;
            if (!ToDouble(commandParams[2], out value))
            {
                Debug.Log("int not found");
                return false;
            }

            if (entity is IBattleableEntity)
            {
                IBattleableEntityData entityData = (entity as IBattleableEntity).Data;
                BattleProperty properties = entityData.RawProperties;
                switch (propertyType)
                {
                    case PropertyType.defense:
                        properties.Defense = (int)value;
                        break;
                    case PropertyType.moveRange:
                        properties.MoveRange = (int)value;
                        break;
                    case PropertyType.attackRange:
                        properties.AttackRange = (int)value;
                        break;
                    case PropertyType.critRate:
                        properties.CritRate = (int)value;
                        break;
                    case PropertyType.critBounes:
                        properties.CritBonus = (int)value;
                        break;
                    case PropertyType.pop:
                        properties.Pop = (int)value;
                        break;
                    case PropertyType.anger:
                        entityData.Anger = (int)value;
                        break;
                    default:
                        break;
                }
                entityData.RawProperties = properties;
            }
            if (entity is IPassiveEntity)
            {
                IPassiveEntityData entityData = (entity as IPassiveEntity).Data;
                switch (propertyType)
                {
                    case PropertyType.health:
                        entityData.Health = (int)value;
                        break;
                    case PropertyType.armor:
                        entityData.Armor = (int)value;
                        break;
                    default:
                        break;
                }
            }
            if (entity is IAggressiveEntity)
            {
                IAggressiveEntityData entityData = (entity as IAggressiveEntity).Data;
                switch (propertyType)
                {
                    case PropertyType.damage:
                        entityData.RawDamage = (int)value;
                        break;
                    default:
                        break;
                }
            }
            return true;
        }

        private static bool Save()
        {
            PlayerFile.SaveCurrentData();
            Console.WriteLine("Player File Save!");
            return true;
        }

        /// <summary>
        /// create army
        /// 1. @selectingEntity
        /// 2. prototype name
        /// 3. exp
        /// 4. optional owner
        /// </summary>
        /// <param name="commandParams"></param>
        /// <returns></returns>
        private static bool CreateArmy(string[] commandParams)
        {
            CellEntity cellEntity = OnMapEntity.SelectingEntity.Exist()?.OnCellOf;
            if (!cellEntity)
            {
                Console.WriteLine("invalid create position");
                return false;
            }

            if (cellEntity.HasArmyStandOn)
            {
                Console.WriteLine("invalid create position");
                return false;
            }


            Player owner = GetPlayerByName(commandParams[3]);
            if (owner is null)
            {
                Console.WriteLine("owner not found");
                return false;
            }


            string name = commandParams[1];
            if (string.IsNullOrEmpty(commandParams[1]))
                name = "Basic Infantry";
            Army army = GameData.Prototypes.GetArmyPrototype(name);
            if (!army) { Console.WriteLine("the army prototype is not exist!"); return false; }


            int exp;
            if (string.IsNullOrEmpty(commandParams[2]))
                exp = 250000;
            else if (!ToInt(commandParams[2], out exp)) { Console.WriteLine("exp error"); return false; }


            var armyItem = new ArmyItem(army, 250000);
            var battleArmy = new BattleArmy(armyItem, owner);
            Game.CurrentBattle.Armies.Add(battleArmy);
            battleArmy.Coordinate = cellEntity.Coordinate;
            Debug.Log(battleArmy.Prefab);
            ArmyEntity.Create(battleArmy);
            return true;
        }

        /// <summary>
        /// create army
        /// 1. @selectingEntity
        /// 2. prototype name
        /// 3. exp
        /// 4. optional owner
        /// </summary>
        /// <param name="commandParams"></param>
        /// <returns></returns>
        private static bool CreateBuilding(string[] commandParams)
        {
            CellEntity cellEntity = OnMapEntity.SelectingEntity?.OnCellOf;
            if (!cellEntity)
            {
                Console.WriteLine("invalid create position");
                return false;
            }

            if (cellEntity.HasArmyStandOn)
            {
                Console.WriteLine("invalid create position");
                return false;
            }


            Player owner = GetPlayerByName(commandParams[3]);
            if (owner is null)
            {
                Console.WriteLine("owner not found");
                return false;
            }


            string name = commandParams[1];
            if (string.IsNullOrEmpty(commandParams[1]))
                name = "Basic Infantry";
            Army army = GameData.Prototypes.GetArmyPrototype(name);
            if (!army) { Console.WriteLine("the army prototype is not exist!"); return false; }


            int exp;
            if (string.IsNullOrEmpty(commandParams[2]))
                exp = 250000;
            else if (!ToInt(commandParams[2], out exp)) { Console.WriteLine("exp error"); return false; }


            var armyItem = new ArmyItem(army, 250000);
            var battleArmy = new BattleArmy(armyItem, owner);
            Game.CurrentBattle.Armies.Add(battleArmy);
            battleArmy.Coordinate = cellEntity.Coordinate;
            Debug.Log(battleArmy.Prefab);
            ArmyEntity.Create(battleArmy);
            return true;
        }

        public static bool SecretlyAddAllItem()
        {
            foreach (var item in GameData.Prototypes.Armies)
            {
                for (int i = 0; i < 6; i++)
                {
                    if (!item.Prototype)
                    {
                        continue;
                    }
                    Game.PlayerData.AddArmyItem(new ArmyItem(item));
                }
            }

            foreach (var item in GameData.Prototypes.Leaders)
            {
                if (!item.Prototype)
                {
                    continue;
                }
                Game.PlayerData.AddLeaderItem(new LeaderItem(item));
            }

            return true;
        }

        private static Player GetPlayerByName(string name)
        {
            Player owner;
            switch (name)
            {
                case "player":
                    owner = Game.CurrentBattle.Player;
                    break;
                case "enemy":
                    owner = Game.CurrentBattle.Enemy;
                    break;
                default:
                    owner = Game.CurrentBattle.AllPlayers.Where((Player player) => player.Name == name).First();
                    break;
            }

            return owner;
        }

        private static bool Print(string[] commandParams)
        {
            Console.WriteLine(commandParams[1]);
            Debug.Log(commandParams[1]);
            return true;
        }

        private static bool GetUUID()
        {
            Entity entity = (HasSelectedOnMapEntity() as Entity).Exist() ?? HasSelectedCardEntity();
            if (entity)
            {
                Console.WriteLine("UUID of " + entity.Data.DisplayingName + " is " + entity.UUID);
                GUIUtility.systemCopyBuffer = entity.UUID.ToString();
            }
            else
            {
                Console.WriteLine("no valid entity selected");
            }
            return true;
        }

        private static bool ExecuteEffect(string[] commandParams)
        {
            if (Game.CurrentBattle is null)
            {
                Console.WriteLine("Command only available in battle");
            }
            /**
             * 1. string typeName
             * (@OnMapEntity source)
             * 2. UUID target
             * 3. int count
             * 4. int param
             * 5[] args
             */
            Effect.Types types; Enum.TryParse(commandParams[1], out types);
            var source = HasSelectedOnMapEntity().Exist() ?? HasSelectedCardEntity().Exist() as Entity ?? Game.CurrentBattle.Player.Entity;
            var target = Entity.Get(Guid.Parse(commandParams[2]));
            int count; ToInt(commandParams[3], out count);
            int param; ToInt(commandParams[4], out param);
            var args = commandParams.ToList();
            args.RemoveRange(0, 5);
            Arg[] args1 = args.Select((string a) => (Arg)a).ToArray();

            var effect = new Effect(types, source, target, count, param, args1);
            var s = effect.Execute();
            if (s)
                Console.WriteLine("Effect execute sucessful");
            else
                Console.WriteLine("Effect execute failed");
            return true;
        }

        private static bool ReloadDefaultPrototype()
        {
            PrototypeLoader.ExportAllDefaultPrototype();
            Console.WriteLine("Default Prototype reloaded");
            return true;
        }

        private static bool Kill(string[] commandParams)
        {
            Entity entity = (HasSelectedOnMapEntity() as Entity).Exist() ?? HasSelectedCardEntity();
            if (entity is IDefeatable)
            {
                (entity as IDefeatable).DefeatedAnimation();
                return true;
            }
            else if (entity)
            {
                entity.Destroy();
                return true;
            }
            else return false;
        }

        private static bool GetArmyItem(string[] commandParams)
        {
            string name = commandParams[1];
            Army army = GameData.Prototypes.GetArmyPrototype(name);
            if (!army)
            {
                Console.WriteLine("the army prototype is not exist!");
                return false;
            }
            ArmyItem armyItem = new ArmyItem(army, 300000);
            Game.PlayerData.AddArmyItem(armyItem);
            PlayerFile.SaveCurrentData();
            Console.WriteLine("player get army");
            return true;
        }

        private static bool FindBattleEntity(string[] commandParams)
        {
            UUID uuid = Guid.Parse(commandParams[1]);
            Entity entity = Entity.Get(uuid);
            if (entity)
            {
                Console.WriteLine("Battle Entity \"" + entity.name + "\" found");
                if (entity is InteractableEntity)
                {
                    (entity as InteractableEntity).Highlight();
                }
            }
            return false;
        }

        private static bool ReloadLanguagePack()
        {
            GameData.Language.ForceLoadLang();
            return true;
        }

        #region Extra

        private static OnMapEntity HasSelectedOnMapEntity()
        {
            return OnMapEntity.SelectingEntity;
        }

        private static CardEntity HasSelectedCardEntity()
        {
            return CardEntity.SelectingCard;
        }

        private static bool ToBool(string input, out bool value)
        {
            try
            {
                value = bool.Parse(input);
                return true;
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }

        private static bool ToInt(string input, out int value)
        {
            try
            {
                value = int.Parse(input);
                return true;
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }

        private static bool ToDouble(string input, out double value)
        {
            try
            {
                value = double.Parse(input);
                return true;
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }

        private static bool ToObject<T>(string json, out T value)
        {
            try
            {
                value = JsonUtility.FromJson<T>(json);
                return true;
            }
            catch (Exception)
            {
                value = default;
                return false;
            }
        }
        #endregion

    }


    [Serializable]
    public struct CommandParameter : INameable
    {
        public enum ParameterType
        {
            /// <summary> single on map entity </summary>
            onMapEntity,
            /// <summary> single int param </summary>
            @int,
            /// <summary> single string param </summary>
            @string,
            /// <summary> single double param </summary>
            @double,
            /// <summary> single bool param </summary>
            @bool,
            /// <summary> multiple int param </summary>
            @ints,
            /// <summary> multiple string param </summary>
            @strings,
            /// <summary> multiple double param </summary>
            @doubles,
            /// <summary> multiple bool param </summary>
            @bools,
            /// <summary> json param </summary>
            json,
        }

        /// <summary> name of the parameter </summary>
        [SerializeField] private string key;
        /// <summary> type of the parameter </summary>
        [SerializeField] private ParameterType type;
        /// <summary> default value of the parameter </summary>
        [SerializeField] private string @default;
        /// <summary> default value of the parameter </summary>
        [SerializeField] private string[] possibleValue;

        public string Name => Key;

        public ParameterType Parameter { get => type; set => type = value; }
        public string Key { get => key; set => key = value; }
        public string Default { get => @default; set => this.@default = value; }
        public string[] PossibleValue { get => possibleValue; set => possibleValue = value; }

        public CommandParameter(ParameterType type, string key, string @default)
        {
            this.type = type;
            this.key = key;
            this.@default = @default;
            this.possibleValue = Array.Empty<string>();
        }


        public bool Equals(CommandParameter other)
        {
            return other.Default == Default && other.Key == Key && other.Parameter == Parameter;
        }

        public override bool Equals(object obj)
        {
            return obj is CommandParameter ? Equals((CommandParameter)obj) : false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(CommandParameter left, CommandParameter right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(CommandParameter left, CommandParameter right)
        {
            return !(left == right);
        }

        private static string Brackets(CommandParameter item, int side)
        {
            string intType = "[]";
            string stringType = "<>";
            string jsonType = "{}";
            switch (item.type)
            {
                case ParameterType.@int:
                case ParameterType.ints:
                case ParameterType.@double:
                case ParameterType.doubles:
                case ParameterType.@bool:
                case ParameterType.bools:
                    return intType[side].ToString();
                case ParameterType.@string:
                case ParameterType.strings:
                    return stringType[side].ToString();
                case ParameterType.json:
                    return jsonType[side].ToString();
                default:
                    return "";
            }
        }

        public override string ToString()
        {
            return Brackets(this, 0) + Parameter.ToString() + ": " + Key + (string.IsNullOrEmpty(Default) ? "" : (" = " + Default)) + Brackets(this, 1);
        }
    }
}