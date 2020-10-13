using UnityEngine;
using UnityEngine.UI;

namespace Canute.StorySystem
{
    public class LetterDisplayer : MonoBehaviour
    {

        public static LetterDisplayer instance;
        public static Letter letter;
        public static bool transparentBG;
        #region Component 
        public Image bg;
        public ScrollRect ScrollRect;
        public Text title;
        public Text text;
        public Text writer;
        #endregion

        public float timer;

        public string loadingLines;
        public const float charPerSecond = 0.05f;

        public static bool IsWorking { get => instance?.enabled == true; set => instance.enabled = value; }

        private void Awake()
        {
            instance = this;
            timer = 0;
            if (BattleSystem.UI.BattleUI.instance)
            {
                BattleSystem.UI.BattleUI.SetUIInteractable(false);
            }
        }
        // Use this for initialization
        private void Start()
        {
            instance = this;
            text.text = "";
            title.text = "";
            writer.text = "";
            loadingLines = "";
            LetterUp();
            GetComponent<Canvas>().worldCamera = Camera.main;
            GetComponent<Canvas>().sortingLayerName = "StoryDisplayer";
            GetComponent<Canvas>().sortingOrder = 0;

            LoadLine(letter);
        }

        private void LetterUp()
        {
            var pos = Camera.main.ScreenToWorldPoint(Vector3.zero);
            pos.y = -5000;
            ScrollRect.content.transform.position = pos;
        }

        // Update is called once per frame
        private void Update()
        {
            if (BattleSystem.UI.BattleUI.instance)
            {
                BattleSystem.UI.BattleUI.SetUIInteractable(false);
            }

            timer += Time.deltaTime;
            LoadWord();
        }

        public void OnMouseUp()
        {
            if (text.text != loadingLines)
            {
                text.text = loadingLines;
                LetterUp();
            }
        }

        public void OnDestroy()
        {
            if (BattleSystem.UI.BattleUI.instance)
            {
                BattleSystem.UI.BattleUI.SetUIInteractable(true);
            }
            instance = null;
        }


        /// <summary> 加载台词 </summary>
        public void LoadWord()
        {
            if (text.text.Length >= loadingLines.Length)
            {
                timer = 0;
                return;
            }
            if (timer >= charPerSecond)
            {
                timer = 0;
                text.text += loadingLines[text.text.Length];
            }
        }

        /// <summary> 加载台词包 </summary>
        /// <param name="wordLine"></param>
        public void LoadLine(Letter wordLine)
        {
            bg.color = transparentBG ? new Color(0, 0, 0, 0) : new Color(0, 0, 0, 1);
            if (wordLine.Background) bg.sprite = wordLine.Background;

            title.text = wordLine.Title + "\n";
            writer.text = wordLine.AuthorDisplayingName;
            text.text = string.Empty;

            loadingLines = wordLine.Text + "\n" + "\n" + "\n";

        }

        /// <summary> 进行下一个对话 </summary>
        public void Next()
        {
        }

        /// <summary> 关闭剧情窗口 </summary>
        public void Quit()
        {
            SceneControl.RemoveScene(MainScene.letterDisplayer);
        }

        public static void Load(Letter letter)
        {
            SceneControl.AddScene(MainScene.letterDisplayer);
            LetterDisplayer.letter = letter;
            transparentBG = !letter.Background;
        }
        public static void Load(string letterName)
        {
            Load(Letter.Get(letterName));
        }
    }
}