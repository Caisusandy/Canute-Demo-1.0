using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.StorySystem
{
    public class StoryDisplayer : MonoBehaviour
    {
        public enum SpeakerStandPosition
        {
            none,
            left,
            middle,
            right
        }

        public static StoryDisplayer instance;
        public static Story currentStory;

        #region Component
        public Image leftPerson;
        public Image rightPerson;
        public Image middlePerson;
        public List<Image> People => new List<Image>() { leftPerson, middlePerson, rightPerson };

        public Text speakerName;
        public Text word;
        #endregion

        public float timer;

        public string loadingLines;
        public const float charPerSecond = 0.02f;

        public static bool IsWorking { get => instance?.enabled == true; set => instance.enabled = value; }

        private void Awake()
        {
            instance = this;
            timer = 0;
        }
        // Use this for initialization
        private void Start()
        {
            instance = this;

            GetComponent<Canvas>().worldCamera = Camera.main;
            GetComponent<Canvas>().sortingLayerName = "StoryDisplayer";
            GetComponent<Canvas>().sortingOrder = 0;

            LoadFirstLine();
        }

        // Update is called once per frame
        private void Update()
        {
            if (!(BattleSystem.UI.BattleUI.instance is null))
            {
                BattleSystem.UI.BattleUI.SetUIInteractive(false);
            }

            timer += Time.deltaTime;
            LoadWord();
        }


        /// <summary> 加载台词 </summary>
        public void LoadWord()
        {
            if (word.text.Length >= loadingLines.Length)
            {
                timer = 0;
                return;
            }
            if (timer >= charPerSecond)
            {
                timer = 0;
                word.text += loadingLines[word.text.Length];
            }
        }

        public void LoadFirstLine()
        {
            if (!currentStory)
            {
                return;
            }
            else
            {
                LoadLine(currentStory.First);
            }
        }

        /// <summary> 加载台词包 </summary>
        /// <param name="wordLine"></param>
        public void LoadLine(WordLine wordLine)
        {
            speakerName.text = wordLine.SpeakerName;
            word.text = string.Empty;
            loadingLines = wordLine.Line;

            switch (wordLine.Position)
            {
                case SpeakerStandPosition.left:
                    break;
                case SpeakerStandPosition.middle:
                    break;
                case SpeakerStandPosition.right:
                    break;
                default:
                    break;
            }
        }

        /// <summary> 进行下一个对话 </summary>
        public void Next()
        {
            if (!currentStory)
            {
                Quit();
                return;
            }

            if (currentStory.CurrentLine.HasSelection)
            {
                return;
            }

            WordLine line = currentStory.Next();

            if (line == WordLine.Empty)
            {
                Quit();
                return;
            }

            LoadLine(line);
        }

        /// <summary> 关闭剧情窗口 </summary>
        private void Quit()
        {
            SceneControl.RemoveScene(MainScene.StoryDisplayer);
        }

        public void OnMouseUp()
        {
            if (word.text == loadingLines)
            {
                Next();
            }
            else
            {
                word.text = loadingLines;
            }
        }

        public static void Load(Story wordLines)
        {
            SceneControl.AddScene(MainScene.StoryDisplayer);
            currentStory = wordLines;
        }

        public void OnDestroy()
        {
            if (!(BattleSystem.UI.BattleUI.instance is null))
            {
                BattleSystem.UI.BattleUI.SetUIInteractive(true);
            }
            instance = null;
        }
    }

}