using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Canute.Testing
{
    public class Console : MonoBehaviour, IWindow
    {
        public static Console instance;

        private static string totalOutput;
        private static List<string> lastCommands = new List<string>();
        private static int index { get; set; } = 0;

        public CommandSheet commandSheet;

        public Text output;
        public Text tips;
        public InputField input;

        private bool IsInputingCommand => input.text.StartsWith("/");
        private string[] CommandSection => input.text.Remove(0, 1).Split(',');
        private string Header => CommandSection[0];
        private string LastParam => CommandSection[ParamCount];
        private int ParamCount => CommandSection.Length - 1;
        private CommandInfo CommandInfo => commandSheet.commands.Get(Header);

        private void Awake()
        {
            if (instance)
            {
                Destroy(instance);
            }
            instance = this;
            output.text = totalOutput;
        }

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
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Close();
            }
            if (Input.GetKeyDown(KeyCode.UpArrow))
            {
                LastCommand();
            }
            if (Input.GetKeyDown(KeyCode.DownArrow))
            {
                NextCommand();
            }
            if (Input.GetKeyDown(KeyCode.Return))
            {
                InputDone(input.text);
            }
        }

        private void NextCommand()
        {
            //Debug.Log(index); Debug.Log(lastCommands.Count);
            if (index > lastCommands.Count - 1)
            {
                return;
            }
            else if (index == lastCommands.Count - 1)
            {
                index++;
                input.text = "";
                return;
            }

            index++;
            input.text = lastCommands[index];
            input.MoveTextEnd(false);
        }

        private void LastCommand()
        {
            //Debug.Log(index); Debug.Log(lastCommands.Count);

            if (index > lastCommands.Count)
            {
                index = lastCommands.Count;
            }
            if (index == 0)
            {
                return;
            }

            index--;
            input.text = lastCommands[index];
            input.MoveTextEnd(false);
        }


        /// <summary> 
        /// force user to start input (Start User input) 
        /// </summary>
        public void Focus()
        {
            //input.OnPointerClick(new PointerEventData(EventSystem.current));
            input.ActivateInputField();
        }

        /// <summary> console output </summary>
        /// <param name="outputstr"></param>
        public void WriteLine(string outputstr)
        {
            totalOutput += outputstr + "\n";

            output.gameObject.SetActive(true);
            output.text += outputstr + "\n";
            tips.gameObject.SetActive(false);
        }

        /// <summary>
        /// Console output
        /// </summary>
        /// <param name="obj">output</param>
        public static void WriteLine(object obj)
        {
            instance.WriteLine(obj.ToString());
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
            Debug.Log("Delta");
            this.input.text = this.input.text.Replace("\t", "");
            ShowTip();
        }

        /// <summary>
        /// when input finished
        /// </summary>
        /// <param name="input"></param>
        public void InputDone(string input)
        {
            Debug.Log("Done!");
            var s = Command.Execute(input);
            //Debug.Log(s);
            this.input.text = "";
            lastCommands.Add(input);
            index = lastCommands.Count;
            Focus();
        }

        private void ShowTip()
        {
            if (!IsInputingCommand)
            {
                return;
            }

            if (CommandInfo is null)
            {
                var list = GetAllHeaderMatches();

                if (list.Count == 0)
                {
                    SetTips("");
                }
                else
                {
                    string tip = "";
                    foreach (var item in list)
                    {
                        tip += item.Name + "\n";
                    }
                    SetTips(tip);
                }
            }
            else
            {
                if (CommandInfo.Params.Length <= ParamCount)
                {
                    return;
                }
                CommandParameter parameter = CommandInfo.Params[ParamCount];
                string tip = "";
                foreach (var item in parameter.PossibleValue)
                {
                    if (item.StartsWith(LastParam))
                    {
                        tip += item + "\n";
                    }
                }
                SetTips(tip);

            }
        }

        /// <summary>
        /// try to auto-fill the command
        /// </summary>
        private void TryAutoFill()
        {
            if (!IsInputingCommand)
            {
                return;
            }

            if (CommandInfo is null)
            {
                var list = GetAllHeaderMatches();

                if (list.Count == 0)
                {
                    return;
                }
                else
                {
                    input.text = "/" + list[0].Name;
                }
            }
            else
            {
                CommandParameter parameter = CommandInfo.Params[ParamCount];
                foreach (var item in parameter.PossibleValue)
                {
                    if (item.StartsWith(LastParam))
                    {
                        input.text = "/";
                        for (int i = 0; i < CommandSection.Length - 1; i++)
                        {
                            string section = CommandSection[i];
                            input.text += section + ",";
                        }
                        input.text += item;
                    }
                }

            }
            input.MoveTextEnd(false);
        }

        #region operation codes


        private List<CommandInfo> GetAllHeaderMatches()
        {
            List<CommandInfo> commands = new List<CommandInfo>();

            foreach (var item in commandSheet.commands)
            {
                if (item.Name.StartsWith(Header))
                {
                    commands.Add(item);
                }
            }

            return commands;
        }


        #endregion

        #region Window
        /// <summary>
        /// Open console window
        /// </summary>
        public void Open()
        {
            enabled = true;
            gameObject.SetActive(true);
            Focus();
        }
        /// <summary>
        /// Close console window
        /// </summary>
        public void Close()
        {
            gameObject.SetActive(false);
            enabled = false;
        }

        #endregion
    }

}