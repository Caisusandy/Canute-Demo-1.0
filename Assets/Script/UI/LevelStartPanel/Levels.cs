using Canute.LevelTree;
using UnityEngine;

namespace Canute.UI.LevelStart
{
    [CreateAssetMenu(fileName = "Level Start Panel", menuName = "UI/Level Start Panel", order = 1)]
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
            panel.levelName = GameData.Chapters.ChapterTree.GetLevel("CalayInfinite").Name;
        }

        public void OpenLevelPanel(string levelName)
        {
            var obj = Instantiate(levelStartPanel, Camera.main.transform.GetChild(0).transform);
            var panel = obj.GetComponent<LevelStartPanel>();
            panel.levelName = GameData.Chapters.ChapterTree.GetLevel(levelName).Name;
        }

        public void OnEnable()
        {
            instance = this;
        }

    }
}