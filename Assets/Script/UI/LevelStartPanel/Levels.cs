using Canute.BattleSystem;
using Canute.LevelTree;
using Canute.UI.LevelStart;
using UnityEngine;

namespace Canute
{
    [CreateAssetMenu(fileName = "Level Start Panel", menuName = "Other/Level Start Panel", order = 1)]
    public class Levels : ScriptableObject
    {
        public static Levels instance;

        public GameObject levelStartPanel;

        public static void OpenLevelPanel(Level level, Transform parent)
        {
            var obj = Instantiate(instance.levelStartPanel, parent);
            var panel = obj.GetComponent<LevelStartPanel>();
            panel.levelName = level.Name;
        }

        public void OpenLevelPanel(Transform parent)
        {
            var obj = Instantiate(levelStartPanel, parent);
            var panel = obj.GetComponent<LevelStartPanel>();
            panel.levelName = GameData.Levels.GetLevel("CalayInfinite").Name;
        }

        public void OpenLevelPanel(string levelName)
        {
            if (GameData.Levels.GetLevel(levelName) != null)
            {
                var obj = Instantiate(levelStartPanel, Camera.main.transform.GetChild(0).transform);
                var panel = obj.GetComponent<LevelStartPanel>();
                panel.levelName = levelName;
            }
        }

        public void OpenLevelPanel(Level level)
        {
            var obj = Instantiate(levelStartPanel, Camera.main.transform.GetChild(0).transform);
            var panel = obj.GetComponent<LevelStartPanel>();
            panel.levelName = level.Name;
        }

        public void OpenTutorial()
        {
            Game.LoadBattle(GameData.Levels.GetLevel("Tutorial"), new LegionSet());
        }

        public void OnEnable()
        {
            instance = this;
        }

    }
}