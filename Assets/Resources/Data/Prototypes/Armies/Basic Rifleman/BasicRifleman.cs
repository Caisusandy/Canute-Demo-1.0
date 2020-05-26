using Canute.Module;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.Armies
{
    public class BasicRifleman : ArmyEntity
    {
        public override float AttackAtionDuration => 2;

        public override float SkillDuration => 2;

        public override float DefeatedDuration => 2;

        public override float WinningDuration => 2;

        public override float HurtDuration => 2;

        public override void SkillExecute(Effect effect)
        {

            return;
        }

        public override void Attack(params object[] vs)
        {
            base.Attack(vs);

            Effect effect = vs[0] as Effect;
            GameObject gameObject = new GameObject();
            gameObject.transform.SetParent(transform);
            gameObject.AddComponent<Image>();
            gameObject.transform.localScale = Vector3.one;
            var motion = gameObject.AddComponent<LinearMotion>();
            motion.MotionEndEvent += Destroy;
            motion.finalPos = effect.Target.transform.position;
            motion.second = 1;


            void Destroy()
            {
                Object.Destroy(gameObject);
            }
        }

        public override void AttackExecute(Effect effect)
        {
            foreach (var item in effect.Targets)
            {
                IPassiveEntity target = item as IPassiveEntity;
                int d = GetPointDistanceOf(target as OnMapEntity);
                double damageDecayPerLevel = 0.8 / (data.Properties.AttackRange - 1);
                for (int i = 0; i < effect.Count; i++)
                {
                    AttackAction(target, (int)(effect.Parameter * damageDecayPerLevel * (data.Properties.AttackRange - d)));
                }
            }
        }

        public override void GetAttackTarget(ref Effect effect)
        {
            IEnumerable<Entity> entities = effect.Targets;

            Vector2Int d = (effect.Target as OnMapEntity).Coordinate - Coordinate;

            var cellEntities = Game.CurrentBattle.MapEntity.GetRay(OnCellOf, (effect.Target as OnMapEntity).OnCellOf);
            Debug.Log("Cell Count: " + cellEntities.Count);

            foreach (var item in cellEntities)
            {
                if (item.GetPointDistanceOf(this) > 2 * data.Properties.AttackRange)
                {
                    continue;
                }
                if (item.HasArmyStandOn)
                    if (item.HasArmyStandOn.Owner == effect.Target.Owner)
                        entities = entities.Union(new List<Entity>() { item.HasArmyStandOn });
            }

            d /= GetCommonDivisor(d.x, d.y);
            for (int i = 0; i < 10; i++)
            {
                try
                {
                    CellEntity item = Game.CurrentBattle.MapEntity[Coordinate + d * i];
                    if (item.GetPointDistanceOf(this) > 2 * data.Properties.AttackRange)
                    {
                        break;
                    }
                    if (item.HasArmyStandOn)
                        if (item.HasArmyStandOn.Owner == effect.Target.Owner)
                            entities = entities.Union(new List<Entity>() { item.HasArmyStandOn });
                }
                catch
                {

                }
            }


            effect.Targets = entities.ToList();
            Debug.Log("Target Count: " + effect.Targets.Count);

            int GetCommonDivisor(int num1, int num2)
            {
                //辗转相除法
                int remainder = 0;
                while (num1 % num2 > 0)
                {
                    remainder = num1 % num2;
                    num1 = num2;
                    num2 = remainder;
                }
                return remainder;
            }
        }
    }
}