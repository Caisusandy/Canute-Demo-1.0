using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
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
