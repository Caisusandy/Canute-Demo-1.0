using System.Collections.Generic;
using UnityEngine;
using Canute.Module;
using Motion = Canute.Module.Motion;

namespace Canute.BattleSystem.UI
{
    public class ArmyBar : BattleUIBase
    {
        public GameObject attackingArmyAttackButton;
        public GameObject infosAnchor;
        public GameObject infoPrefab;
        public List<ArmyInfoIcon> armyInfos = new List<ArmyInfoIcon>();

        public override void Awake()
        {
            attackingArmyAttackButton.SetActive(false);
            base.Awake();
        }
        // Use this for initialization
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {
        }

        public ArmyInfoIcon GetAvailableSlot()
        {
            var gameObject = Instantiate(infoPrefab, infosAnchor.transform);
            var armyInfoIcon = gameObject.GetComponent<ArmyInfoIcon>();
            armyInfos.Add(armyInfoIcon);
            return armyInfoIcon;
            //foreach (ArmyInfoIcon item in armyInfos)
            //{
            //    if (item.IsAvailable)
            //    {
            //        item.transform.SetParent(infosAnchor.transform);
            //        return item;
            //    }
            //    Debug.Log("not available");
            //}
            //Debug.LogError("No enough slot!");
            //return null;
        }

        public override void Hide()
        {
            Debug.Log("hide army bar");
            Motion.SetMotion(gameObject, BattleUI.UndersideAnchor, true);
            Motion.SetMotion(gameObject, BattleUI.UndersideAnchor, true);
            isShown = false;
        }

        private void OnMouseDown()
        {
            ToggleBar();
        }

        public void ToggleBar()
        {
            if (Battle.Round.CurrentStat == Round.Stat.gameStart)
            {
                return;
            }
            if (BattleUI.Raycaster.enabled == false)
            {
                return;
            }

            Debug.Log("mouse down");

            if (isShown) Hide();
            else { Show(); }

            BattleUI.ClientHandCardBar.HideCards(!isShown);
        }

        public void AttackingArmyAttack()
        {
            ArmyAttack.TryAttack();
        }
    }
}