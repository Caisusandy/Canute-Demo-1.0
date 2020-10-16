using UnityEngine;
namespace Canute.BattleSystem
{
    [CreateAssetMenu(fileName = "Building", menuName = "Prototype/Building Prototype")]
    public class BuildingPrototypeContainer : PrototypeContainer<Building>
    {


        [ContextMenu("Add To Prototype Factory")]
        public override void AddToPrototypeFactory()
        {
            base.AddToPrototypeFactory();
        }
    }
}
