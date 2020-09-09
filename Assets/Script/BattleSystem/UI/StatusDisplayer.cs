using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    public class StatusDisplayer : MonoBehaviour
    {
        [Header("Related Status")]
        public Status status;
        public GameObject StatusDetailDisplayerPrefab;

        [Header("Components")]
        public Text statusName;
        public StatusDetailDisplayer statusDetailDisplayer;
        public Image statusIcon;
        public Image statusBG;

        public void ShowStatus(Status status)
        {
            this.status = status;
        }

        private void Update()
        {
            statusName.text = status.GetDisplayingName();
            statusIcon.sprite = status.Effect.Icon;
        }

        public void OnMouseDown()
        {
            GenerateDetailDisplayer();
        }

        public void OnMouseUp()
        {
            DestroyDetailDisplayer();
        }

        public void OnMouseOver()
        {
            GenerateDetailDisplayer();
        }
        public void OnMouseExit()
        {
            DestroyDetailDisplayer();
        }

        public void GenerateDetailDisplayer()
        {
            if (statusDetailDisplayer)
            {
                return;
            }

            GameObject gameObject = Instantiate(StatusDetailDisplayerPrefab, transform);
            statusDetailDisplayer = gameObject.GetComponent<StatusDetailDisplayer>();
            statusDetailDisplayer.status = status;
            statusDetailDisplayer.GetComponent<Canvas>().sortingLayerName = "UI";
            statusDetailDisplayer.GetComponent<Canvas>().sortingOrder = 1000;
        }

        public void DestroyDetailDisplayer()
        {
            Destroy(statusDetailDisplayer.Exist()?.gameObject);
            statusDetailDisplayer = null;
        }
    }
}
