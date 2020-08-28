using UnityEngine;
using Canute.Module;
using Motion = Canute.Module.Motion;
using Canute.BattleSystem;

namespace Canute.BattleSystem.UI
{
    public abstract class EnemyUIBase : BattleUIBase
    {
        public override Player Player => Game.CurrentBattle?.Enemy;
    }
    public abstract class BattleUIBase : MonoBehaviour
    {
        public bool isShown;
        public Vector3 originalPos;
        public virtual Player Player => Game.CurrentBattle?.Player;
        public static Battle Battle => Game.CurrentBattle;


        public virtual void Awake()
        {
            if (Game.CurrentBattle is null)
            {
                Destroy(this);
            }
            isShown = true;
            originalPos = transform.position;
        }

        public virtual void Hide()
        {
            if (!isShown)
            {
                return;
            }
            gameObject.SetActive(false);
            isShown = false;
        }

        public virtual void Show()
        {
            if (isShown)
            {
                return;
            }
            isShown = true;
            gameObject.SetActive(true);
            Motion.SetMotion(gameObject, originalPos + Camera.main.transform.position, true);
        }
    }
}
