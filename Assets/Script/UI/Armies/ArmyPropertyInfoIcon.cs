using Canute.BattleSystem;
using UnityEngine;

namespace Canute.UI
{
    [Obsolete]
    public abstract class ArmyPropertyInfoIcon : Icon
    {
        [HideInInspector] public IArmy army;
        [Obsolete]
        public virtual void SetArmyItem(IArmy armyItem)
        {
            this.army = armyItem;
        }
    }
}