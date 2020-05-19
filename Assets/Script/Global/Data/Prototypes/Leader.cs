using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    [Serializable]
    public class Leader : Prototype
    {
        [SerializeField] protected Career career;
        [SerializeField] protected List<PropertyBounes> bounes;


        public Career Career => career;
        public override GameObject Prefab => null;
        public List<PropertyBounes> Bounes { get => bounes; set => bounes = value; }
    }



    //[Serializable]
    //public struct LeaderBounes
    //{
    //    [SerializeField] private int defenseBounesRate;
    //    [SerializeField] private int damageBounesRate;
    //    [SerializeField] private int healthBounesRate;
    //    [SerializeField] private int critRateBounesRate;
    //    [SerializeField] private int critBounesBounesRate;

    //    [SerializeField] private int attackRangeBounesPoint;
    //    [SerializeField] private int moveRangeBounesPoint;
    //    [SerializeField] private int popBounesPoint;

    //    public LeaderBounes(int defenseBounesRate, int damageBounesRate, int healthBounesRate, int critRateBounesRate, int critBounesBounesRate, int attackRangeBounesPoint, int moveRangeBounesPoint, int popBounesPoint)
    //    {
    //        this.defenseBounesRate = defenseBounesRate;
    //        this.damageBounesRate = damageBounesRate;
    //        this.healthBounesRate = healthBounesRate;
    //        this.critRateBounesRate = critRateBounesRate;
    //        this.critBounesBounesRate = critBounesBounesRate;
    //        this.attackRangeBounesPoint = attackRangeBounesPoint;
    //        this.moveRangeBounesPoint = moveRangeBounesPoint;
    //        this.popBounesPoint = popBounesPoint;
    //    }

    //    public int DefenseBounesRate => defenseBounesRate;
    //    public int DamageBounesRate => damageBounesRate;
    //    public int HealthBounesRate => healthBounesRate;
    //    public int CritRateBounesRate => critRateBounesRate;
    //    public int CritBounesBounesRate => critBounesBounesRate;


    //    public int AttackRangeBounesPoint => attackRangeBounesPoint;
    //    public int MoveRangeBounesPoint => moveRangeBounesPoint;
    //    public int PopBounesPoint => popBounesPoint;

    //    public static LeaderBounes operator +(LeaderBounes l, LeaderBounes r)
    //    {
    //        LeaderBounes leaderBounes;

    //        leaderBounes = new LeaderBounes()
    //        {
    //            defenseBounesRate = l.defenseBounesRate.Bounes(r.defenseBounesRate),
    //            damageBounesRate = l.damageBounesRate.Bounes(r.damageBounesRate),
    //            healthBounesRate = l.healthBounesRate.Bounes(r.healthBounesRate),
    //            critRateBounesRate = l.critRateBounesRate.Bounes(r.critRateBounesRate),
    //            critBounesBounesRate = l.critBounesBounesRate.Bounes(r.critBounesBounesRate),

    //            attackRangeBounesPoint = l.attackRangeBounesPoint + r.attackRangeBounesPoint,
    //            moveRangeBounesPoint = l.defenseBounesRate + r.defenseBounesRate,
    //            popBounesPoint = l.defenseBounesRate + r.defenseBounesRate
    //        };
    //        return leaderBounes;
    //    }
    //}

}