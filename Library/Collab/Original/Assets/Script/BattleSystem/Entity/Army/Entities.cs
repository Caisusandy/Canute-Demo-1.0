using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    public static class PassiveEntities
    {
        public static void Damage(this IPassiveEntity passiveEntity, int damage)
        {
            damage = passiveEntity.Data.GetFinalDamage(damage);
            int actualDamage = passiveEntity.Data.Damage(damage);

            if (damage != actualDamage)//have amor
            {
                passiveEntity.DisplayDamage(actualDamage);
            }
            else passiveEntity.DisplayDamage(actualDamage);
        }

        public static void DisplayDamage(this IPassiveEntity passiveEntity, int damage)
        {
            GameObject displayer = Object.Instantiate(GameData.Prefabs.ArmyDamageDisplayer, passiveEntity.transform);
            displayer.GetComponent<ArmyDamageDisplayer>().damage = damage;
            Debug.Log("Display Damage");
        }
    }

    public static class AgressiveEntities
    {

    }

    public interface IPassiveEntity : IBattleableEntity
    {
        new IPassiveEntityData Data { get; }
        float DefeatedDuration { get; }
        float HurtDuration { get; }

        void Hurt(params object[] vs);
    }

    public interface IAggressiveEntity : IBattleableEntity
    {
        new IAggressiveEntityData Data { get; }
        float AttackAtionDuration { get; }

        void Attack(params object[] vs);
    }

    public interface IBattleEntity : IPassiveEntity, IAggressiveEntity, ISkillable, IDefeatable
    {
        new IBattleEntityData Data { get; }

    }

    public interface IBattleableEntity : IOnMapEntity
    {
        IBattleableEntityData Data { get; }
        float SkillDuration { get; }
        float WinningDuration { get; }

        void Winning(params object[] vs);
    }


    public interface IDefeatable
    {
        void ReadyToDie(params object[] vs);
        void Defeated(params object[] vs);
        void Remove(params object[] vs);
    }


    public interface IOnMapEntity : IInteractableEntity
    {
        CellEntity OnCellOf { get; }
        Vector3Int HexCoord { get; }
        Vector2Int Position { get; }
        int x { get; }
        int y { get; }
        OnMapEntityData OnMapData { get; }
        bool AllowMove { get; set; }

        int GetPointDistance(OnMapEntity other);
        int GetPointDistance(Vector3Int v3);
    }
}
