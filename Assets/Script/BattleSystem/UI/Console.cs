using Canute.BattleSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Canute.Testing
{
    /// <summary>
    /// Game Console
    /// <para>Not to be confuse by System.Console</para>
    /// </summary>
    public class Console : MonoBehaviour, IWindow
    {
        const string splitter = ",";
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

            ////header
            //if (CommandInfo is null)
            //{
            //    //  Debug.Log("not a expect command header(yet)");
            //    var list = GetAllHeaderMatches();

            //    if (list.Count == 0)
            //    {
            //        // Debug.Log("not a expect command header(totally)");
            //        SetTips("");
            //    }
            //    else
            //    {
            //        string tip = "";
            //        foreach (var item in list)
            //        {
            //            tip += item.Name + "\n";
            //        }
            //        SetTips(tip);
            //    }
            //}            
            //header
            if (CommandInfo is null)
            {
                var nexts = PossibleNext();
                if (nexts is null)
                {
                    SetTips("");
                }
                else if (nexts.Length > 0)
                {
                    var tip = "";
                    foreach (var item in nexts)
                    {
                        tip += "/" + item + "\n";
                    }
                    SetTips(tip.Remove(tip.Length - 1));
                }
                else
                {
                    SetTips("");
                }
            }
            //param
            else
            {
                string tip = CommandInfo.ToString();
                var nexts = PossibleNext();
                if (!(nexts is null))
                    foreach (var item in nexts)
                    {
                        tip += "\n" + item;
                    }
                SetTips(tip);
            }
        }

        /// <summary>
        /// try to auto-fill the command
        /// </summary>
        private void TryAutoFill()
        {
            var nexts = PossibleNext();
            if (nexts is null)
            {
                return;
            }
            else
            {
                if (CommandInfo is null)
                {
                    input.text = "/" + nexts[0];
                }
                else
                {
                    var sections = CommandSection;
                    //Debug.Log(sections.Length);
                    //foreach (var item in sections)
                    //{
                    //    Debug.Log(item);
                    //}
                    input.text = "/" + CommandSection[0];
                    for (int i = 1; i < sections.Length - 1; i++)
                    {
                        string section = sections[i];
                        Debug.Log(section);
                        input.text += splitter + section;
                    }
                    if (nexts.Length > 0)
                    {
                        input.text += splitter + nexts[0];
                    }
                }
            }
            {
                //if (!IsInputingCommand)
                //{
                //    return;
                //}
                //if (CommandInfo is null)
                //{
                //    var list = GetAllHeaderMatches();
                //    if (list.Count == 0)
                //    {
                //        return;
                //    }
                //    else
                //    {
                //        input.text = "/" + list[0].Name;
                //        if (CommandInfo.Params.Length != 0)
                //            if (CommandInfo.Params[0].Parameter != CommandParameter.ParameterType.onMapEntity)
                //                input.text += splitter;
                //    }
                //}
                //else
                //{
                //    if (ParamCount == 0 && CommandInfo.Params.Length != 0)
                //    {
                //        input.text += splitter;
                //    }
                //    else
                //    {
                //        int position = ParamCount + CommandInfo.Params[0].Parameter == CommandParameter.ParameterType.onMapEntity ? 0 : -1;
                //        if (position > CommandInfo.Params.Length - 1 || position < 0)
                //        {
                //            return;
                //        }
                //        CommandParameter parameter = CommandInfo.Params[position];
                //        Debug.Log(LastParam);
                //        foreach (var item in parameter.PossibleValue)
                //        {
                //            if (item.StartsWith(LastParam))
                //            {
                //                var sections = CommandSection;
                //                input.text = "/";
                //                for (int i = 0; i < sections.Length - 1; i++)
                //                {
                //                    string section = sections[i];
                //                    input.text += section + splitter;
                //                }
                //                input.text += item;
                //            }
                //        }
                //    }
                //}
            }
            input.MoveTextEnd(false);
        }

        public string[] PossibleNext()
        {
            if (!IsInputingCommand)
            {
                return null;
            }

            List<string> ret = new List<string>();
            //header
            if (CommandInfo is null)
            {
                var list = GetAllHeaderMatches();
                if (list.Count == 0) { ret.Add(""); goto end; }
                else
                {
                    foreach (var item in list)
                    {
                        ret.Add(item.Name);
                    }
                    goto end;
                }
            }
            //param
            else
            {
                if (CommandInfo.Params.Length == 0)
                {
                    ret.Add("");
                    goto end;
                }
                bool SelectOnMapEntity = CommandInfo.Params[0].Parameter == CommandParameter.ParameterType.onMapEntity;
                if (CommandInfo.Params.Length == 1 && SelectOnMapEntity)
                {
                    ret.Add("");
                    goto end;
                }

                int position = ParamCount + (SelectOnMapEntity ? 1 : 0) - 1;
                Debug.Log(position);
                if (position > CommandInfo.Params.Length - 1 || position < 0)
                {
                    return null;
                }
                CommandParameter parameter = CommandInfo.Params[position];
                if (!string.IsNullOrEmpty(parameter.Default))
                {
                    ret.Add(parameter.Default);
                    goto end;
                }
                if (parameter.PossibleValue != null)
                {
                    if (parameter.PossibleValue.Length != 0)
                    {
                        foreach (var item in parameter.PossibleValue)
                        {
                            Debug.Log(LastParam);
                            if (item.StartsWith(LastParam))
                                ret.Add(item);
                        }
                    }
                }
            }
            end:
            ret.Sort();
            return ret.ToArray();
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