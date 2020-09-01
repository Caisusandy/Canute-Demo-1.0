﻿using Canute.BattleSystem.UI;
using Canute.Testing;
using System;
using UnityEngine;

namespace Canute.BattleSystem
{
    public class Control : MonoBehaviour
    {
        public static Control instance;
        public Vector3 inputPos;

        public Canvas UICanvas;

        public static Testing.GameDebug DebugPanel => BattleUI.DebugWindow;
        public static Testing.Console Console => BattleUI.Console;
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
            if (Input.GetKeyDown(KeyCode.Slash) && Game.Configuration.IsDebugMode)
            {
                ToggleConsole();
            }
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
            if (Map.transform.localScale.x > 3 && v > 0)
            {
                return;
            }
            if (Map.transform.localScale.x < 0.5 && v < 0)
            {
                return;
            }
            Map.transform.localScale *= 1 + (v / 3);
        }

        public void ToggleConsole()
        {
            if (Console.input.isFocused)
            {
                return;
            }
            BattleUI.ToggleWindow(Console);
        }

        public void ToggleDebugWindow()
        {
            DebugPanel.gameObject.SetActive(!DebugPanel.gameObject.activeSelf);
        }

        public void ToggleUICanvas()
        {
            UICanvas.enabled = !UICanvas.enabled;
        }

    }
}