using System;
using UnityEngine;

namespace Canute.BattleSystem
{
    public interface IEntityData : INameable, IUUIDLabeled, ICloneable, IOwnable
    {
        Entity Entity { get; }
        GameObject Prefab { get; set; }
        Prototype Prototype { get; set; }
        bool HasValidPrototype { get; }
    }
}