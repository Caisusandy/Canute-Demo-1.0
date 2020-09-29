using Canute.BattleSystem;
using UnityEngine;

namespace Canute.UI
{
    public abstract class ArmyPropertyInfoIcon : Icon
    {
        [HideInInspector] public IArmy army;
        public virtual void SetArmyItem(IArmy armyItem)
        {
            this.army = armyItem;
        }
    }
}