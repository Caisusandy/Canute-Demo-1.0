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


        public override void SkillExecute(Effect effect)
        {
            var target = ArmyAttack.GetLowestHealthPointTarget(this, false);
            if (!(target is Entity))
            {
                return;
            }
            effect.Target = target as Entity;

            int damage = target.Data.Health.Bonus(20 - 100) + (target.Data.MaxHealth - target.Data.Health).Bonus(40 - 100);
            effect.Parameter = damage;
            base.SkillExecute(effect);
        }

        public override void Attack(params object[] vs)
        {
            base.Attack(vs);

            Effect effect = vs[0] as Effect;
            //GameObject gameObject = new GameObject();
            //gameObject.transform.SetParent(transform);
            //gameObject.AddComponent<Image>();
            //gameObject.transform.localScale = Vector3.one;
            //var motion = gameObject.AddComponent<LinearMotion>();
            //motion.MotionEndEvent += Destroy;
            //motion.finalPos = effect.Target.transform.position;
            //motion.second = 1;


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
                double damageDecayPerLevel = 0.8 / (2 * data.Properties.AttackRange - 1);

                for (int i = 0; i < effect.Count; i++)
                {
                    Debug.Log(effect.Parameter);
                    Debug.Log((int)(effect.Parameter * (0.2 + damageDecayPerLevel * (2 * data.Properties.AttackRange - d))));
                    AttackAction(target, (int)(effect.Parameter * (0.2 + damageDecayPerLevel * (2 * data.Properties.AttackRange - d))));
                }
            }
        }

        public override void GetAttackTarget(ref Effect effect)
        {
            IEnumerable<Entity> entities = effect.Targets;

            Vector3Int d = (effect.Target as OnMapEntity).HexCoord - HexCoord;

            var cellEntities = Game.CurrentBattle.MapEntity.GetRay(OnCellOf, (effect.Target as OnMapEntity).OnCellOf);
            Debug.Log("Cell Count: " + cellEntities.Count);

            foreach (var item in cellEntities)
            {
                if (item.GetPointDistanceOf(this) > (2 * data.Properties.AttackRange))
                {
                    continue;
                }
                if (item.HasArmyStandOn)
                    if (item.HasArmyStandOn.Owner == effect.Target.Owner)
                        entities = entities.Union(new List<Entity>() { item.HasArmyStandOn });
            }

            if (StatList.GetTag("name:rifleman2Resonance") != null)
            {
                d /= GetCommonDivisor(d.x, d.y);
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        CellEntity item = Game.CurrentBattle.MapEntity[HexCoord + d * i];
                        if (item.GetPointDistanceOf(this) > 2 * data.Properties.AttackRange)
                        {
                            break;
                        }
                        if (item.HasArmyStandOn)
                            if (item.HasArmyStandOn.Owner == effect.Target.Owner)
                                entities = entities.Union(new List<Entity>() { item.HasArmyStandOn });
                    }
                    catch { }
                }
            }



            effect.Targets = entities.ToList();
            Debug.Log("Target Count: " + effect.Targets.Count);

            int GetCommonDivisor(int num1, int num2)
            {
                num1 = Mathf.Abs(num1);
                num2 = Mathf.Abs(num2);

                if (num1 == 0 || num2 == 0)
                {
                    return 1;
                }
                //辗转相除法
                int remainder = 1;
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