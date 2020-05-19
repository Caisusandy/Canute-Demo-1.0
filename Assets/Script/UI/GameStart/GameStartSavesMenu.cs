using UnityEngine;

namespace Canute.UI
{
    public class GameStartSavesMenu : MonoBehaviour
    {
        public static GameStartSavesMenu instance;
        public GameObject SavesPrefab;

        public void Awake()
        {
            instance = this;
        }

        public void OnEnable()
        {
            OpenMenu();
        }

        public void OnDisable()
        {
            ClearMenu();
        }

        public void OpenMenu()
        {
            foreach (var item in PlayerFile.GetAllSaves())
            {
                GameObject sI = Instantiate(SavesPrefab, transform);
                GameStartSavesInfo savesInfo = sI.GetComponent<GameStartSavesInfo>();
                savesInfo.playerData = item;
            }
        }

        public void ClearMenu()
        {
            foreach (Transform item in transform)
            {
                Destroy(item.gameObject);
            }
        }
    }
}