using UnityEngine;

namespace Canute.BattleSystem.UI
{
    public class PausePanel : MonoBehaviour, IWindow
    {
        public static PausePanel instance;

        public bool IsPausing => enabled;

        private void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        private void Start()
        {
            Close();
        }

        // Update is called once per frame
        private void Update()
        {

        }

        public void Open()
        {
            enabled = true;
            gameObject.SetActive(true);
            BattleUI.SetUIInteractive(false);
        }

        public void Close()
        {
            enabled = false;
            BattleUI.SetUIInteractive(true);
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
            SceneControl.GotoScene(MainScene.mainHall);
            Game.ClearBattle();
        }
    }
}
