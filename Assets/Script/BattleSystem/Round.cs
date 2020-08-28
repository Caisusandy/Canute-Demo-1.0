using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    /// <summary>
    /// 回合控制器
    /// <para>拥有一个状态机 Stat</para>
    /// </summary>
    [Serializable]
    public class Round
    {
        public enum Stat
        {
            gameStart,
            turnBegin,
            normal,
            action,
            turnEnd,
            gameEnd,
        }

        [SerializeField] public int wave = 1;
        [SerializeField] public int round = 1;
        [SerializeField] public int turn = 1;

        [SerializeField] private Stat stat;
        [SerializeField] private UUID currentPlayerUUID;

        [NonSerialized] public Battle battle;

        public List<UUID> PlayersUUID => battle.AllPlayers?.ToUUIDList();
        public Player CurrentPlayer { get => battle.GetPlayer(currentPlayerUUID); set => currentPlayerUUID = value is null ? UUID.Empty : value.UUID; }
        public Player NextPlayer { get => GetNextPlayer(); }


        public Stat CurrentStat => stat;








        public Stat Normal() => stat = Stat.normal;

        public Stat InAction() => stat = Stat.action;

        public Stat TurnBegin()
        {
            stat = Stat.turnBegin;
            CurrentPlayer.Trigger(TriggerCondition.Conditions.turnBegin);
            foreach (var item in CurrentPlayer.BattleArmies)
            {
                item.Trigger(TriggerCondition.Conditions.turnBegin);
            }
            foreach (var item in CurrentPlayer.Buildings)
            {
                item.Trigger(TriggerCondition.Conditions.turnBegin);
            }
            return stat;
        }

        public Stat TurnEnd()
        {
            stat = Stat.turnEnd;
            CurrentPlayer.Trigger(TriggerCondition.Conditions.turnEnd);
            foreach (var item in CurrentPlayer.BattleArmies)
            {
                item.Trigger(TriggerCondition.Conditions.turnEnd);
            }
            foreach (var item in CurrentPlayer.Buildings)
            {
                item.Trigger(TriggerCondition.Conditions.turnEnd);
            }
            return stat;
        }

        public Stat GameEnd() => stat = Stat.gameEnd;



        /// <summary>
        /// 将回合的记录切换到下一个人的回合
        /// </summary>
        private void TurnPass()
        {
            turn++;
            if (turn > battle.AllPlayers.Count)
            {
                turn = 1;
                round++;
            }
        }

        /// <summary>
        /// 刷新玩家
        /// </summary>
        private void RefreshPlayer()
        {
            CurrentPlayer = battle.AllPlayers[turn - 1];
        }

        private Player GetNextPlayer()
        {
            int turn = this.turn + 1;
            int round = this.round;
            if (turn > battle.AllPlayers.Count)
            {
                turn = 1;
                round++;
            }
            return battle.AllPlayers[turn - 1];
        }

        public void Next()
        {
            TurnPass();
            RefreshPlayer();
        }

        public Round(Battle battle)
        {
            this.battle = battle;
            stat = Stat.gameStart;
            CurrentPlayer = battle.AllPlayers[0];
        }
    }
}