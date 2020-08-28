using UnityEngine;

namespace Canute.BattleSystem.UI
{
    public abstract class InfoPanel : MonoBehaviour
    {
        [Header("Anchor")]
        public GameObject statusInfoAnchor;
        [Header("Status Prefab")]
        public GameObject statusPrefab;
        public abstract IStatusContainer StatusContainer { get; }

        public void LoadStatus()
        {
            ClearStatus();
            foreach (Status status in StatusContainer.StatList)
            {
                StatusDisplayer displayer = Instantiate(statusPrefab, statusInfoAnchor.transform).GetComponent<StatusDisplayer>();
                displayer.status = status;
            }
        }

        public void ClearStatus()
        {
            foreach (Transform item in statusInfoAnchor.transform)
            {
                Destroy(item);
            }
        }
    }
}
