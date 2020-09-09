using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute
{

    [Serializable]
    public class Leader : Prototype
    {
        [SerializeField] protected Career career;
        [SerializeField] protected List<PropertyBonus> bounes;
        [SerializeField] protected HalfSkillEffect skill;
        [SerializeField] protected bool hasAssociateCharacter;


        public override GameObject Prefab => null;
        public Career Career => career;
        public List<PropertyBonus> Bounes { get => bounes; set => bounes = value; }
        public Character Charater => HasAssociateCharacter ? GameData.Prototypes.GetCharacter(name) : default;
        public bool HasAssociateCharacter { get => hasAssociateCharacter; set => hasAssociateCharacter = value; }

        protected override Sprite GetIcon()
        {
            if (hasAssociateCharacter)
                return Charater?.Icon;
            if (Game.Configuration.UseCustomDefaultPrototype && !icon)
                return GameData.Prototypes.GetPrototype(name)?.Icon;
            return icon;
        }


        protected override Sprite GetPortrait()
        {
            if (hasAssociateCharacter)
                return GameData.Prototypes.GetCharacter(name)?.Portrait;
            if (Game.Configuration.UseCustomDefaultPrototype && !icon)
                return GameData.Prototypes.GetPrototype(name)?.Portrait;
            return portrait;
        }
    }
}