using Canute.BattleSystem.UI;
using Canute.Testing;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class BattleControl : MonoBehaviour
    {
        private const double maxMapScale = 1.6;
        private const double minMaxScale = 0.8;

        public static BattleControl instance;
        public Vector3 inputPos;

        public static GameDebug DebugPanel => BattleUI.DebugWindow;
        public static PausePanel GamePausePanel => BattleUI.PausePanel;
        public static MapEntity Map => Game.CurrentBattle.MapEntity;
        public static Vector3 UserInputPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Start is called before the first frame update
        private void Awake()
        {
            instance = this;
        }
        // Update is called once per frame
        private void Update()
        {
            inputPos = UserInputPosition;
            FunctionKeys();
        }

        /// <summary>
        /// Get Target
        /// </summary> 
        public void GetRaycastTarget()
        {
            // Cast a ray straight down.
            List<RaycastHit2D> hitObj = new List<RaycastHit2D>(10);
            ContactFilter2D contactFilter2D = new ContactFilter2D();
            Physics2D.Raycast(transform.position, -Vector2.zero, contactFilter2D.NoFilter(), hitObj);
            // If it hits something...

            for (int i = 0; i < hitObj.Count; i++)
            {
                RaycastHit2D hit = hitObj[i];
                if (hit.collider == null) //if collider found nothing
                {
                    continue;
                }

                Transform hitTransform = hit.transform;

                CardEntity possibleCardEntity = hitTransform.GetComponent<CardEntity>();

                if (possibleCardEntity) //if it is not a CellEntity, ignore
                {
                    //possibleCardEntity.OnMouseDown();
                    //possibleCardEntity.OnMouseUp();
                }
            }
        }

        public void FunctionKeys()
        {
            if (Input.GetKeyDown(KeyCode.F1) && !BattleUI.PausePanel.IsPausing)
            {
                ToggleUICanvas();
            }
            if (Input.GetKeyDown(KeyCode.F3) && Game.Configuration.IsDebugMode)
            {
                ToggleDebugWindow();
            }
            //if (Input.GetKeyDown(KeyCode.Slash) && Game.Configuration.IsDebugMode)
            //{
            //    ToggleConsole();
            //}
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                ScaleMap(Input.GetAxis("Mouse ScrollWheel"));
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TryCloseCurrentWindow();

            }
        }

        private void TryCloseCurrentWindow()
        {
            Debug.Log("Esc");
            if (BattleUI.currentWindow is null)
            {
                Debug.Log("Open pause");
                BattleUI.ToggleWindow(GamePausePanel);
            }
            else
            {
                Debug.Log("Close current window");
                BattleUI.CloseCurrentWindow();
            }
        }

        public void ScaleMap(float v)
        {
            if (Module.Motion.ongoingMotions.Count > 0) return;
            if (Map.transform.localScale.x > maxMapScale && v > 0) return;
            if (Map.transform.localScale.x < minMaxScale && v < 0) return;
            if (StorySystem.StoryDisplayer.instance) return;
            if (PausePanel.instance.Exist()?.enabled == true) return;

            Map.transform.localScale *= 1 + (v / 3);
        }

        public void ToggleDebugWindow()
        {
            DebugPanel.gameObject.SetActive(!DebugPanel.gameObject.activeSelf);
        }

        public void ToggleUICanvas()
        {
            BattleUI.ToggleUICanvas();
        }

    }
}