using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.BattleSystem
{
    /// <summary>
    /// Name of the event (Effect.Types.event)
    /// </summary>
    public static class EventName
    {
        public const string realDamage = "realDamage";
        public const string addArmor = "addArmor";
        public const string damage = "damage";
        public const string move = "move";
        public const string armySwitch = "armySwitch";
        public const string drawCard = "drawCard";
        public const string addActionPoint = "addActionPoint";
        public const string noActionPointRequire = "noActionPointRequire";
        public const string occupy = "occupy";
        public const string protection = "protection";
        public const string aircraftFighterReturn = "aircraftFighterReturn";
        public const string addDragonCard = "addDragonCard";
        public const string dragonCritAttack = "dragonCritAttack";
        public const string createArmy = "createArmy";
    }


    public static partial class EffectExecute
    {

        private static bool Event(Effect effect)
        {
            string name = effect[Effect.name];
            bool result = true;
            switch (name)
            {
                case EventName.addArmor:
                    result = AddArmor(effect);
                    break;
                case EventName.realDamage:
                    result = RealDamage(effect);
                    break;
                case EventName.damage:
                    result = EventDamage(effect);
                    break;
                case EventName.move:
                    result = EventMove(effect);
                    break;
                case EventName.armySwitch:
                    result = ArmySwitch(effect);
                    break;
                case EventName.drawCard:
                    result = DrawCard(effect);
                    break;
                case EventName.addActionPoint:
                    result = AddActionPoint(effect);
                    break;
                case EventName.noActionPointRequire:
                    result = NoActionPointrequire(effect);
                    break;
                case EventName.occupy:
                    result = Occupy(effect);
                    break;
                case EventName.protection:
                    result = Protection(effect);
                    break;
                case EventName.createArmy:
                    result = EventCreateArmy(effect);
                    break;

                //special
                case EventName.aircraftFighterReturn:
                    result = FighterAircraftReturn(effect);
                    break;
                case EventName.addDragonCard:
                    result = AddDragonCard(effect);
                    break;
                case EventName.dragonCritAttack:
                    result = DragonCritAttack(effect);
                    break;
                case "airship" + EventName.createArmy:
                    result = EventCreateArmy(effect);
                    break;
                //case "":
                //    result =
                //    break;
                default:
                    Debug.LogError("a event without {name} arg is trying to execute");
                    break;
            }

            return result;
        }

        #region Events

        private static bool AddDragonCard(Effect effect)
        {
            List<EventCard> eventCards = new List<EventCard>();
            foreach (var a in GameData.Prototypes.DragonEventCards)
            {
                eventCards.Add(a);
            }

            int getCardCount = effect.Count > 6 ? 6 : effect.Count;
            for (int i = 0; i < getCardCount; i++)
            {
                int random = UnityEngine.Random.Range(0, eventCards.Count);
                Debug.Log(random);
                EventCard eventCard = eventCards[random];
                eventCards.RemoveAt(random);

                Card card = new Card(Card.Types.special, eventCard, 1)
                {
                    Owner = effect.Target.Owner
                };
                CardEntity.Create(card);
            }

            return true;
        }

        private static bool FighterAircraftReturn(Effect effect)
        {
            ArmyEntity armyEntity = effect.Source as ArmyEntity;

            if (!armyEntity)
            {
                return false;
            }

            if (armyEntity.data.Type != Army.Types.aircraftFighter)
            {
                return false;
            }

            CellEntity cellEntity = effect.GetCellParam();
            cellEntity.data.canStandOn = true;
            Effect returnEffect = new Effect(Effect.Types.@event, armyEntity, cellEntity, 1, 0, "name:move");
            returnEffect.Execute(false);

            return true;
        }

        private static bool RealDamage(Effect effect)
        {
            for (int i = 0; i < effect.Count; i++)
            {
                foreach (Entity item in effect.Targets)
                {
                    IPassiveEntityData passiveEntity = item.Data as IPassiveEntityData;
                    if (passiveEntity is null)
                    {
                        continue;
                    }
                    passiveEntity.RealDamage(effect.Parameter);
                }
            }
            return true;
        }

        private static bool DrawCard(Effect effect)
        {

            for (int i = 0; i < effect.Count; i++)
            {
                Game.CurrentBattle.GetHandCard(effect.Source.Owner, effect.Parameter);
            }
            return true;
        }

        private static bool AddActionPoint(Effect effect)
        {
            foreach (var item in effect.Targets)
            {
                for (int i = 0; i < effect.Count; i++)
                {
                    if (item.Data is IActionPointUser)
                    {
                        ((IActionPointUser)item.Data).ActionPoint += effect.Parameter;
                    }
                    else item.Owner.ActionPoint += effect.Parameter;
                }
            }
            return true;
        }

        private static bool AddArmor(Effect effect)
        {

            foreach (var item in effect.Targets)
            {
                if (!(item is IPassiveEntity))
                {
                    return false;
                }
            }

            foreach (var item in effect.Targets)
            {
                IPassiveEntity passiveEntity = item as IPassiveEntity;
                for (int i = 0; i < effect.Count; i++)
                {
                    passiveEntity.Data.Armor += effect.Parameter;
                }
            }
            return true;
        }

        private static bool EventDamage(Effect effect)
        {
            foreach (var item in effect.Targets)
            {
                if (!(item is IPassiveEntity))
                {
                    return false;
                }
            }

            IAggressiveEntity aggressiveEntity = effect.Source as IAggressiveEntity;
            foreach (var item in effect.Targets)
            {
                IPassiveEntity passiveEntity = item as IPassiveEntity;
                var executingEffect = effect.Clone();
                executingEffect.Target = item;
                RewriteAttackEffect(ref executingEffect);

                for (int i = 0; i < executingEffect.Count; i++)
                {
                    if (aggressiveEntity is null) passiveEntity.Hurt(executingEffect.Parameter);
                    else passiveEntity.Hurt(executingEffect.Parameter, aggressiveEntity);
                }
            }

            return true;
        }

        private static bool EventMove(Effect effect)
        {
            IBattleableEntity movingEntity = effect.Source as IBattleableEntity;
            CellEntity destination = (effect.Target as OnMapEntity)?.OnCellOf;

            //Debug.Log(movingArmy);
            //Debug.Log(destination);

            if (movingEntity is null || destination is null)
            {
                Debug.Log("invalid effect");
                return false;
            }
            else if (!movingEntity.AllowMove)
            {
                Debug.LogWarning("the moving entity should not moving but it is trying to move");
                return false;
            }
            else if (destination.IsValidDestination(movingEntity))
            {
                return false;
            }

            PathFinder.FinderParam finderParam = PathFinder.FinderParam.ignoreDestinationArmy | PathFinder.FinderParam.ignoreDestinationBuilding;
            if (movingEntity is ArmyEntity)
            {
                finderParam |= PathFinder.FinderParam.ignoreBuilding;
            }
            else if (movingEntity is BuildingEntity)
            {
                finderParam |= PathFinder.FinderParam.ignoreArmy;
            }

            List<CellEntity> path = PathFinder.GetPath(movingEntity.OnCellOf, destination, -1, finderParam);

            if (path is null)
            {
                Debug.Log(destination);
                return false;
            }
            else if (path.Count == 0)
            {
                Debug.Log(destination);
                return false;
            }
            else
            {
                movingEntity.Move(path, effect);
                return true;
            }
        }


        private static bool ArmySwitch(Effect effect)
        {
            ArmyEntity target = effect.Target as ArmyEntity;
            if (!target)
            {
                return false;
            }
            if (effect.Source is ArmyEntity)
            {
                ArmyEntity source = effect.Source as ArmyEntity;

                List<CellEntity> path1 = PathFinder.GetPath(source.OnCellOf, target.OnCellOf, -1, PathFinder.FinderParam.ignoreAirArmy & PathFinder.FinderParam.ignoreLandArmy);
                List<CellEntity> path2 = PathFinder.GetPath(target.OnCellOf, source.OnCellOf, -1, PathFinder.FinderParam.ignoreAirArmy & PathFinder.FinderParam.ignoreLandArmy);

                if (path1.Count == 0 || path2.Count == 0)
                {
                    return false;
                }

                Effect move1 = new Effect(Effect.Types.move, source, target.OnCellOf, 1);
                Effect move2 = new Effect(Effect.Types.move, target, source.OnCellOf, 1);

                source.Move(path1, move1);
                target.Move(path2, move2);
                //ArmyMotion.SetMotion(source, path1, effect);
                //ArmyMotion.SetMotion(target, path2, effect);

                Game.CurrentBattle.Round.Normal();
                return true;

            }
            else if (effect.Source is CardEntity)
            {
                if (ArmyEntity.GetArmies(target.OnCellOf, target.data.Properties.MoveRange).Count == 0)
                {
                    return false;
                }

                AddSelectEvent(Switch);
                ArmyMovement.movingArmy = target;
                ArmyMovement.ShowMoveRange();
                return true;
            }
            else return false;

            void Switch(Entity entity)
            {
                if (new Effect(Effect.Types.@event, ArmyMovement.movingArmy, entity, 1, 0, "name:armySwitch").Execute(true))
                {
                    RemoveSelectEvent(Switch);
                    ArmyMovement.EndShowMoveRange();
                    ArmyMovement.movingArmy = null;
                }
            }
        }

        private static bool NoActionPointrequire(Effect effect)
        {
            if (Card.LastCard == null)
            {
                return false;
            }

            Card.LastCard.ActionPoint = 0;
            return true;
        }

        private static bool Protection(Effect effect)
        {
            if (effect.Target is CellEntity)
            {
                CellEntity cellEntity = effect.Target as CellEntity;
                if (cellEntity.HasArmyStandOn)
                {
                    if (cellEntity.HasArmyStandOn.Owner != effect.Source.Owner)
                    {
                        return false;
                    }

                    if (cellEntity.HasArmyStandOn.data.Type == Army.Types.shielder)
                    {
                        return false;
                    }

                    Effect protection = new Effect(Effect.Types.tag, effect.Source, cellEntity.HasArmyStandOn, 1, 0, "name:protection");
                    protection.SetSpecialName("protection");
                    cellEntity.HasArmyStandOn.StatList.Add(new Status(protection, -1, -1, Status.StatType.perminant));

                    Effect removeProtection = new Effect(Effect.Types.removeStatus, effect.Source, cellEntity.HasArmyStandOn, 1);
                    removeProtection["uuid"] = protection.UUID.ToString();

                    Status item = new Status(removeProtection, 0, 1, Status.StatType.countBase, false, TriggerCondition.OnExitCell);
                    cellEntity.HasArmyStandOn.StatList.Add(item);
                    (effect.Source as IStatusContainer)?.StatList.Add(item);
                }
            }

            return true;
        }

        private static bool SwitchDragonStatus(Effect effect)
        {
            Debug.Log("Siwtch status");
            var dragon = effect.Target as ArmyEntity;
            if (dragon.data.Type != Army.Types.dragon)
            {
                return false;
            }

            Status dragonBonus = dragon.StatList.GetStatus(Effect.Types.propertyBonus, "name:dragonBonus");
            string stat = dragonBonus.Effect["dragonStatus"];
            dragon.StatList.Remove(dragonBonus);


            if (stat == "defense")
            {
                dragon.StatList.Add(GetStatusOnFast(dragon));
            }
            else
            {
                dragon.StatList.Add(GetStatusOnSlow(dragon));
            }

            Debug.Log("Siwtch status end");
            return true;
            Status GetStatusOnFast(ArmyEntity armyEntity)
            {
                Effect attack = new Effect(PropertyType.attackRange, BonusType.additive, armyEntity, armyEntity, 1, 2, "name:dragonBonus", "dragonStatus:attack");
                attack[Effect.propertyBonusType] = "dragonAttack";
                return new Status(attack, -1, -1, Status.StatType.resonance, true);
            }

            Status GetStatusOnSlow(ArmyEntity armyEntity)
            {
                Effect defense = new Effect(PropertyType.defense, BonusType.percentage, armyEntity, armyEntity, 1, 800, "name:dragonBonus", "dragonStatus:defense");
                defense[Effect.propertyBonusType] = "dragonDefense";
                return new Status(defense, -1, -1, Status.StatType.resonance, true);
            }
        }

        private static bool DragonCritAttack(Effect effect)
        {
            ArmyEntity armyEntity = effect.Target as ArmyEntity;

            if (armyEntity.data.Type == Army.Types.dragon)
            {
                SwitchDragonStatus(new Effect(Effect.Types.@event, effect.Source, effect.Target, 1, 0));
                return true;
            }

            Effect damage = new Effect(Effect.Types.@event, effect.Source, armyEntity, 1, 10, "name:damage", "attackType:percentage");
            var v = damage.Execute();
            Debug.Log(v);

            Effect addStatus = new Effect(Effect.Types.addStatus, effect.Source, armyEntity, 1, 10, effect.Args.ToArray());
            addStatus.Execute();

            return true;
        }

        private static bool Occupy(Effect effect)
        {
            foreach (var item in effect.Targets)
            {
                IOwnable ownable = item;
                if (ownable is null)
                {
                    return false;
                }
                if (ownable.Owner == effect.Source.Owner)
                {
                    return false;
                }
            }

            foreach (var item in effect.Targets)
            {
                BuildingEntity buildingEntity = item as BuildingEntity;
                buildingEntity.Owner = effect.Source.Owner;
            }

            return true;
        }

        private static bool EventCreateArmy(Effect effect)
        {
            /* 
             *  CellParam ["armyName"]["armyType"]["career"]["attackPosition"]["attackType"]["prefabPath"]
             *  ["attack"]["health"]["defense"]["critRate"]["critBonus"]["moveRange"]["moveRange"] 
             */
            CellEntity cellEntity = effect.GetCellParam();
            string armyName = effect["armyName"];


            int damage = effect.Args.GetIntParam("attack");
            int health = effect.Args.GetIntParam("health");
            int defense = effect.Args.GetIntParam("defense");
            int critRate = effect.Args.GetIntParam("critRate");
            int critBonus = effect.Args.GetIntParam("critBonus");
            int moveRange = effect.Args.GetIntParam("moveRange");
            int attackRange = effect.Args.GetIntParam("moveRange");

            damage = damage != -1 ? damage : 0;
            health = health != -1 ? health : 0;
            defense = defense != -1 ? defense : 0;
            critRate = critRate != -1 ? critRate : 20;
            critBonus = critBonus != -1 ? critBonus : 20;
            moveRange = moveRange != -1 ? moveRange : 2;
            attackRange = attackRange != -1 ? attackRange : 4;

            Army.Types type = effect.Args.GetEnumParam<Army.Types>("armyType");
            Career career = effect.Args.GetEnumParam<Career>("career");

            BattleProperty.Position attackPosition = effect.Args.GetEnumParam<BattleProperty.Position>("attackPosition");
            BattleProperty.Position standPosition = effect.Args.GetEnumParam<BattleProperty.Position>("standPosition");
            BattleProperty.AttackType attackType = effect.Args.GetEnumParam<BattleProperty.AttackType>("attackType");

            GameObject prefab = Resources.Load<GameObject>(effect["prefabPath"]);

            if (cellEntity is null)
            {
                return false;
            }


            BattleProperty battleProperty = new BattleProperty()
            {
                StandPosition = standPosition,
                AttackPosition = attackPosition,

                CritBonus = critBonus,
                CritRate = critRate,

                Attack = attackType,
                Defense = defense,

                AttackRange = attackRange,
                MoveRange = moveRange,
            };

            var army = new Army(armyName, health, damage, type, career, prefab, new List<BattleProperty>() { battleProperty });
            var armyItem = new ArmyItem(army, 0);
            var battleArmy = new BattleArmy(armyItem, effect.Source.Owner) { Coordinate = cellEntity.Coordinate, Prefab = prefab };


            Game.CurrentBattle.Armies.Add(battleArmy);
            ArmyEntity.Create(battleArmy);

            return true;
        }

        private static bool AirshipCreateArmy(Effect effect)
        {
            bool ans = EventCreateArmy(effect);
            if (!ans)
            {
                return ans;
            }

            ArmyEntity armyEntity = effect.Source as ArmyEntity;
            var motherStat = new Effect(Effect.Types.tag, effect.GetCellParam().HasArmyStandOn, armyEntity, 1, 0, "name:motherOf");
            var status = new Status(motherStat, -1, -1, Status.StatType.perminant, false);
            armyEntity.StatList.Add(status);

            return true;
        }

        #endregion
    }
}
