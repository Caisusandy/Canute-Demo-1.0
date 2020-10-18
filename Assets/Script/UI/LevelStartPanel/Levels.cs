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

        [Obsolete]
        public static void OpenLevelPanel(Level level, Transform parent)
        {
            var obj = Instantiate(instance.levelStartPanel, parent);
            var panel = obj.GetComponent<LevelStartPanel>();
            panel.levelName = level.Name;
        }

        [Obsolete]
        public void OpenLevelPanel(Transform parent)
        {
            var obj = Instantiate(levelStartPanel, parent);
            var panel = obj.GetComponent<LevelStartPanel>();
            panel.levelName = GameData.Levels.GetLevel("CalayInfinite").Name;
        }

        [Obsolete]
        public void OpenLevelPanel(string levelName)
        {
            if (GameData.Levels.GetLevel(levelName) != null)
            {
                var obj = Instantiate(levelStartPanel, Camera.main.transform.GetChild(0).transform);
                var panel = obj.GetComponent<LevelStartPanel>();
                panel.levelName = levelName;
            }
        }

        [Obsolete]
        public void OpenLevelPanel(Level level)
        {
            var obj = Instantiate(levelStartPanel, Camera.main.transform.GetChild(0).transform);
            var panel = obj.GetComponent<LevelStartPanel>();
            panel.levelName = level.Name;
        }

        [Obsolete]
        public void OpenTutorial()
        {
            Game.LoadBattle(GameData.Levels.GetLevel("Tutorial"), new LegionSet());
        }

        [Obsolete]
        public void OnEnable()
        {
            instance = this;
        }

    }
}