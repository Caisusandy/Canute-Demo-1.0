using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{

    [Serializable]
    public class Leader : Prototype
    {
        [SerializeField] protected Sprite portrait;
        [SerializeField] protected Career career;
        [SerializeField] protected List<PropertyBonus> bounes;
        [SerializeField] protected HalfSkillEffect skill;
        [SerializeField] protected bool hasAssociateCharacter;


        public Career Career => career;
        public List<PropertyBonus> Bounes { get => bounes; set => bounes = value; }
        public Character Character => GameData.Prototypes.GetCharacter(name);
        public bool HasAssociateCharacter { get => hasAssociateCharacter; set => hasAssociateCharacter = value; }
        public Sprite Portrait => Game.Configuration.UseCustomDefaultPrototype && !icon ? GameData.Prototypes.GetLeaderPrototype(name)?.portrait : portrait;

        protected override Sprite GetIcon()
        {
            if (hasAssociateCharacter)
                return Character?.Icon;
            if (Game.Configuration.UseCustomDefaultPrototype && !icon)
                return GameData.Prototypes.GetPrototype(name)?.Icon;
            return icon;
        }
    }
}