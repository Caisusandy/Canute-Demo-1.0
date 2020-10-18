using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Canute.UI.GlobalSetting
{
    public class SettingUI : MonoBehaviour
    {
        public static SettingUI instance;
        public List<SettingUISection> sections;
        public new Camera camera;
        public GameObject BG;

        public void Start()
        {
            foreach (var item in sections)
            {
#if UNITY_EDITOR 
                if (item is SettingUIDebug) item.gameObject.SetActive(true);
#endif
            }

            if (SceneManager.sceneCount > 1)
            {
                DestroyImmediate(camera.gameObject);
                DestroyImmediate(BG);
            }
        }

        public void Back()
        {
            if (SceneManager.sceneCount > 1)
            {
                SceneControl.RemoveScene(MainScene.settings);
            }
            else SceneControl.GotoLastImmediate();
        }
    }

    public abstract class SettingUISection : MonoBehaviour, INameable
    {
        public abstract string Name { get; }
    }
}
