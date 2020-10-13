using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.StorySystem
{
    public class StoryDisplayer : MonoBehaviour
    {
        [Serializable]
        class Selection
        {
            public Button button;
            public Text info;
        }

        public enum SpeakerStandPosition
        {
            none,
            left,
            middle,
            right
        }

        internal static void LoadSceneIntro(object p)
        {
            throw new NotImplementedException();
        }

        public static StoryDisplayer instance;
        public static Story currentStory;
        public static List<Story> nextStories = new List<Story>();
        public static bool transparentBG;
        #region Component
        public Image bg;

        public Image leftPerson;
        public Image rightPerson;
        public Image middlePerson;
        public List<Image> People => new List<Image>() { leftPerson, middlePerson, rightPerson };

        public GameObject selectionPanel;
        [SerializeField] private List<Selection> selections = new List<Selection>();

        public Text speakerName;
        public Text word;
        #endregion

        public float timer;

        public string loadingLines;
        public const float charPerSecond = 0.02f;

        private void Awake()
        {
            instance = this;

            timer = 0;
            if (BattleSystem.UI.BattleUI.instance)
            {
                BattleSystem.UI.BattleUI.SetUICanvasActive(false);
            }
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
            if (BattleSystem.UI.BattleUI.instance)
            {
                BattleSystem.UI.BattleUI.SetUICanvasActive(false);
            }

            timer += Time.deltaTime;
            LoadWord();
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

        public void OnDestroy()
        {
            if (BattleSystem.UI.BattleUI.instance)
            {
                BattleSystem.UI.BattleUI.SetUICanvasActive(true);
            }

            instance = null;
        }


        /// <summary> 加载台词 </summary>
        public void LoadWord()
        {
            if (word.text.Length >= loadingLines.Length)
            {
                timer = 0;
                if (currentStory.CurrentLine.HasSelection && !selectionPanel.activeSelf) LoadSelection(currentStory.CurrentLine);
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
            bg.color = transparentBG ? new Color(0, 0, 0, 0) : new Color(0, 0, 0, 1);
            bg.sprite = wordLine.ConversationBG;
            speakerName.text = wordLine.SpeakerName;
            CloseSelection();
            NormalWordline(wordLine);
            SetSpeaker(wordLine);
        }

        private void NormalWordline(WordLine wordLine)
        {
            word.text = string.Empty;
            loadingLines = wordLine.Line;
        }

        private void LoadSelection(WordLine wordLine)
        {
            selectionPanel.SetActive(true);

            for (int i = 0; i < wordLine.Selections.Count; i++)
            {
                selections[i].button.gameObject.SetActive(true);
                selections[i].info.text = wordLine.Selections[i].selectionInfo;
            }
            for (int i = wordLine.Selections.Count; i < 4; i++)
            {
                selections[i].button.gameObject.SetActive(false);
            }
        }

        private void CloseSelection()
        {
            selectionPanel.SetActive(false);
        }

        private void SetSpeaker(WordLine wordLine)
        {
            if (leftPerson.sprite) leftPerson.color = new Color(0.8f, 0.8f, 0.8f);
            else leftPerson.enabled = false;


            if (middlePerson.sprite) middlePerson.color = new Color(0.8f, 0.8f, 0.8f);
            else middlePerson.enabled = false;


            if (rightPerson.sprite) rightPerson.color = new Color(0.8f, 0.8f, 0.8f);
            else rightPerson.enabled = false;


            switch (wordLine.Position)
            {
                case SpeakerStandPosition.left:
                    leftPerson.enabled = true;
                    leftPerson.color = Color.white;
                    leftPerson.sprite = wordLine.CharacterPortrait;
                    break;
                case SpeakerStandPosition.middle:
                    middlePerson.enabled = true;
                    middlePerson.color = Color.white;
                    middlePerson.sprite = wordLine.CharacterPortrait;
                    break;
                case SpeakerStandPosition.right:
                    rightPerson.enabled = true;
                    rightPerson.color = Color.white;
                    rightPerson.sprite = wordLine.CharacterPortrait;
                    break;
                default:
                    break;
            }
        }

        public void SelectOption(int index)
        {
            var id = currentStory.CurrentLine.Selections[index].toWordLineId;
            var wordLine = currentStory.ContinueFrom(id);
            LoadLine(wordLine);
        }

        /// <summary> 进行下一个对话 </summary>
        public void Next()
        {
            Debug.Log(nextStories.Count);
            if (!currentStory)
            {
                TryQuit();
                return;
            }

            if (currentStory.CurrentLine.HasSelection)
            {
                return;
            }

            WordLine line = currentStory.Next();

            if (line == WordLine.Empty)
            {
                TryQuit();
                return;
            }

            LoadLine(line);
        }

        /// <summary> 关闭剧情窗口 </summary>
        public void TryQuit()
        {
            if (nextStories.Count > 0)
            {
                NextStory();
            }
            else Quit();
        }

        public static void NextStory()
        {
            currentStory = nextStories[0];
            transparentBG = currentStory.Type == StoryType.dailyConversation;
            nextStories.RemoveAt(0);
        }

        public void Quit()
        {
            currentStory = Story.Empty;
            SceneControl.RemoveScene(MainScene.StoryDisplayer);
        }

        public static void Load(Story story)
        {
            Debug.Log(story);
            Debug.Log(currentStory);
            Debug.Log((bool)currentStory);

            if (currentStory)
            {
                nextStories.Add(story);
                return;
            }

            SceneControl.AddScene(MainScene.StoryDisplayer);
            currentStory = story;
            transparentBG = story.Type == StoryType.dailyConversation;
        }

        public static void Load(string storyName)
        {
            Load(Story.Get(storyName));
        }

        public static void LoadSceneIntro(Story story)
        {
            if (currentStory)
            {
                nextStories.Add(story);
                return;
            }

            SceneControl.AddScene(MainScene.StoryDisplayer);
            currentStory = story;
            transparentBG = true;
        }

        public static void LoadSceneIntro(string storyName) { LoadSceneIntro(Story.Get(storyName)); }
    }
}