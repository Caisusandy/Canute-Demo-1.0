using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    public static class PassiveEntities
    {
        public static void Damage(this IPassiveEntity passiveEntity, int damage)
        {
            damage = (int)(damage * Random.Range(0.95f, 1.05f));

            int armorHit = passiveEntity.Data.DamageArmor(damage);
            damage -= armorHit;
            damage = passiveEntity.Data.GetDamageAfterDefensePoint(damage);

            passiveEntity.Data.Damage(damage);
            passiveEntity.DisplayDamage(damage);

            if (armorHit != 0) passiveEntity.DisplayDamage(armorHit);
        }

        public static void DisplayDamage(this IPassiveEntity passiveEntity, int damage)
        {
            Debug.Log(damage);
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
        void GetAttackTarget(ref Effect effect);
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
        BattleProperty.Position StandPostion { get; }

        int GetPointDistanceOf(OnMapEntity other);
        int GetPointDistanceOf(Vector3Int v3);
    }
}
