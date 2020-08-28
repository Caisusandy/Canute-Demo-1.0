using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    public static class PassiveEntities
    {
        public static void Damage(this IPassiveEntity passiveEntity, int damage)
        {
            int flow = (int)(damage);// * Random.Range(0.95f, 1.05f));
            int armorHit = passiveEntity.Data.DamageArmor(flow);

            flow -= armorHit;
            flow = passiveEntity.Data.GetDamageAfterDefensePoint(flow);

            passiveEntity.Data.Damage(flow);
            passiveEntity.DisplayDamage(flow);

            if (armorHit != 0) passiveEntity.DisplayDamage(armorHit);
        }

        public static void Damage(this IPassiveEntity passiveEntity, int damage, IAggressiveEntity sourceEntity)
        {
            int finalDamageIfCrit = damage.Bonus(Random.value < sourceEntity.Data.Properties.CritRate ? 0 : sourceEntity.Data.Properties.CritBonus, BonusType.percentage);
            int flow = (int)(finalDamageIfCrit);//* Random.Range(0.95f, 1.05f));
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
        /// <summary> Entity Data </summary>
        new IPassiveEntityData Data { get; }
        float DefeatedDuration { get; }
        float HurtDuration { get; }

        void Hurt(params object[] vs);
    }

    public interface IAggressiveEntity : IBattleableEntity
    {
        /// <summary> Entity Data </summary>
        new IAggressiveEntityData Data { get; }
        float AttackAtionDuration { get; }

        void Attack(params object[] vs);
        void GetAttackTarget(ref Effect effect);
    }

    public interface IBattleEntity : IPassiveEntity, IAggressiveEntity, ISkillable, IDefeatable
    {
        /// <summary> Entity Data </summary>
        new IBattleEntityData Data { get; }

    }

    public interface IBattleableEntity : IOnMapEntity
    {
        /// <summary>
        /// Entity Data
        /// </summary>
        IBattleableEntityData Data { get; }
        /// <summary>
        /// Duration of the Skill
        /// </summary>
        float SkillDuration { get; }
        /// <summary>
        /// Duration of Winning Action
        /// </summary>
        float WinningDuration { get; }

        /// <summary>
        /// Set Entity in moving action 
        /// </summary>
        /// <param name="vs"></param>
        void Move(params object[] vs);

        /// <summary>
        /// Set Entity in winning action 
        /// </summary>
        /// <param name="vs"></param>
        void Winning(params object[] vs);
    }


    public interface IDefeatable
    {
        /// <summary>
        /// kill entity (entity will be killed when no animation is occuring)
        /// </summary>
        /// <param name="vs"></param>
        void KillEntity(params object[] vs);
        /// <summary>
        /// Animation when entity is defeated
        /// </summary>
        /// <param name="vs"></param>
        void DefeatedAnimation(params object[] vs);
    }


    public interface IOnMapEntity : IInteractableEntity
    {
        /// <summary> 物体所在的格子 <para>The CellEntity this object stand on </para> </summary>
        CellEntity OnCellOf { get; }
        /// <summary> 六边形坐标 <para> Coordinate for hex-map </para></summary>
        Vector3Int HexCoord { get; }
        /// <summary> 坐标 <para> Coordinate </para></summary>
        Vector2Int Coordinate { get; }
        /// <summary> x <para></para></summary>
        int x { get; }
        /// <summary> y <para></para></summary>
        int y { get; }
        ///
        new OnMapEntity entity { get; }
        /// <summary>  <para></para></summary>
        OnMapEntityData OnMapData { get; }
        /// <summary> 允许移动 <para> Determine whether the entity allows to move</para></summary>
        bool AllowMove { get; set; }
        BattleProperty.Position StandPostion { get; }

        /// <summary>
        /// Get the straight distance to <paramref name="destination"/>
        /// </summary>
        /// <param name="destination"></param>
        /// <returns></returns>
        int GetPointDistanceOf(OnMapEntity other);

        /// <summary>
        /// Get the real distance to <paramref name="destination"/>
        /// </summary>
        /// <param name="destination"></param>
        /// <param name="movingEntity"></param>
        /// <returns></returns>
        int GetRealDistanceOf(OnMapEntity destination, OnMapEntity movingEntity = null);
    }
}
