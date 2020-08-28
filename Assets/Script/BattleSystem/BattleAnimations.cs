using UnityEngine;


namespace Canute.BattleSystem
{
    public static class BattleAnimations
    {
        public static void AddToBattle(this Animator animator)
        {
            if (Game.CurrentBattle is null)
            {
                return;
            }
            Game.CurrentBattle.OngoingAnimation.Add(animator);
            Game.CurrentBattle.InAnimation();
        }

        public static void RemoveFromBattle(this Animator animator)
        {
            if (Game.CurrentBattle is null)
            {
                return;
            }
            Game.CurrentBattle.OngoingAnimation.Remove(animator);
            Game.CurrentBattle.TryEndAnimation();
        }

        public static bool IsDone(this Animator animator)
        {
            return animator.GetBool("isIdle");
        }

        public static bool TryRemoveFromBattle(this Animator animator)
        {
            bool isDone = animator.IsDone();
            if (isDone)
            {
                animator.RemoveFromBattle();
            }
            return isDone;
        }
    }
}
