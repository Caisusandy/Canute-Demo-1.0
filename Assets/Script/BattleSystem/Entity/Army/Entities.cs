using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    public static class PassiveEntities
    {
        public static void Damage(this IPassiveEntity passiveEntity, int damage)
        {
            int flow = (int)(damage * Random.Range(0.95f, 1.05f));
            int armorHit = passiveEntity.Data.DamageArmor(flow);

            flow -= armorHit;
            flow = passiveEntity.Data.GetDamageAfterDefensePoint(flow);

            passiveEntity.Data.Damage(flow);
            passiveEntity.DisplayDamage(flow);

            if (armorHit != 0) passiveEntity.DisplayDamage(armorHit);
        }

        public static void Damage(this IPassiveEntity passiveEntity, int damage, IAggressiveEntity sourceEntity)
        {
            int finalDamageIfCrit = damage.Bounes(Random.value < sourceEntity.Data.Properties.CritRate ? 0 : sourceEntity.Data.Properties.CritBounes, BounesType.percentage);
            int flow = (int)(finalDamageIfCrit * Random.Range(0.95f, 1.05f));
            int armorHit = passiveEntity.Data.DamageArmor(flow);

            flow -= armorHit;
            flow = passiveEntity.Data.GetDamageAfterDefensePoint(flow);

            passiveEntity.Data.Damage(flow);
            passiveEntity.DisplayDamage(flow);

            if (armorHit != 0) passiveEntity.DisplayDamage(armorHit);
        }

        public static void DisplayDamage(this IPassiveEntity passiveEntity, int damage)
        {
            Debug.Log(damage);
            GameObject displayer = Object.Instantiate(GameData.Prefabs.Get("armyDamageDisplayer"), passiveEntity.transform);
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

        void Move(params object[] vs);
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
        Vector2Int Coordinate { get; }
        int x { get; }
        int y { get; }
        OnMapEntityData OnMapData { get; }
        bool AllowMove { get; set; }
        BattleProperty.Position StandPostion { get; }

        int GetPointDistanceOf(OnMapEntity other);
        int GetPointDistanceOf(Vector3Int v3);
    }
}
