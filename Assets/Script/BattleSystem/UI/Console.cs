using Canute.BattleSystem;
using Canute.Testing;
using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class Console : MonoBehaviour, IWindow
    {
        public CommandSheet commandSheet;

        public Text output;
        public Text tips;
        public InputField input;


        //tips and output will always not show together
        private void Start()
        {
            Focus();
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                TryAutoFill();
            }
        }

        /// <summary> Start User input </summary>
        public void Focus()
        {
            input.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        /// <summary> console output </summary>
        /// <param name="output"></param>
        public void WriteLine(string output)
        {
            this.output.gameObject.SetActive(true);
            this.output.text += output + "\n";
            tips.gameObject.SetActive(false);
        }

        /// <summary>
        /// set tips of command in the console
        /// </summary>
        /// <param name="tips"></param>
        public void SetTips(string tips)
        {
            this.tips.gameObject.SetActive(true);
            this.tips.text = tips;
            output.gameObject.SetActive(false);
        }

        /// <summary>
        /// when input changed
        /// </summary>
        /// <param name="input"></param>
        public void InputDelta(string input)
        {

        }

        /// <summary>
        /// when input finished
        /// </summary>
        /// <param name="input"></param>
        public void InputDone(string input)
        {
            WriteLine(input);
            //

            this.input.text = "";
            Focus();
        }

        /// <summary>
        /// try to auto-fill the command
        /// </summary>
        private void TryAutoFill()
        {
            throw new NotImplementedException();
        }

        #region operation codes




        #endregion

        #region Window
        public void Open()
        {
            enabled = true;
            gameObject.SetActive(true);
            BattleUI.SetUIInteractive(false);
            Focus();
        }

        public void Close()
        {
            BattleUI.SetUIInteractive(true);
            gameObject.SetActive(false);
            enabled = false;
        }

        #endregion
    }

    public static class Commands
    {

    }
}