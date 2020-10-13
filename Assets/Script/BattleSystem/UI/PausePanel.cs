using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class PausePanel : MonoBehaviour, IWindow
    {
        public static PausePanel instance;

        public Text title;
        public Text description;
        public bool IsPausing => enabled;

        private void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        private void Start()
        {
            title.text = Game.CurrentLevel.Title;
            description.text = Game.CurrentLevel.Description;
            Close();
        }

        // Update is called once per frame
        private void Update()
        {
            BattleUI.SetUIInteractable(false);
        }

        public void Open()
        {
            enabled = true;
            gameObject.SetActive(true);
            BattleUI.SetUIInteractable(false);
        }

        public void Close()
        {
            enabled = false;
            BattleUI.SetUIInteractable(true);
            gameObject.SetActive(false);
        }

        public void ToggleOpenStatus()
        {
            if (IsPausing)
            {
                Close();
            }
            else Open();
        }

        public void Quit()
        {
            if (Game.CurrentBattle is null) { QuitBattle(); }
            else if (Game.CurrentBattle.BattleType == Battle.Type.endless) { Game.CurrentBattle.EndlessEnd(); }
            else { QuitBattle(); }
        }

        public void OpenSetting()
        {
            SceneControl.AddScene(MainScene.settings);
        }

        public void OnDisable()
        {
            BattleUI.SetUIInteractable(true);
        }
        public void QuitBattle()
        {
            StartCoroutine(Fade());
        }
        public IEnumerator Fade()
        {
            yield return BattleUI.FadeOutBattle();
            yield return new EntityEventPack(Clear).Execute();

            void Clear(params object[] vs)
            {
                Debug.Log("Clear scene");

                Game.ClearBattle();
                SceneControl.GotoScene(MainScene.mainHall);
            }
        }
    }
}
