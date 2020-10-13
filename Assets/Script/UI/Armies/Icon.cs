using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{

    [RequireComponent(typeof(Image))]
    public abstract class Icon : MonoBehaviour
    {
        [HideInInspector] public GameObject label;

        protected Image IconImage => GetComponent<Image>();
        protected Text Label => label.GetComponent<Label>().text;

        public virtual void OnMouseOver() { DisplayInfo(); }

        public virtual void OnMouseDown() { DisplayInfo(); }

        public virtual void OnMouseExit() { HideInfo(); }

        public virtual void OnMouseUp() { HideInfo(); }

        public virtual void DisplayInfo()
        {
            if (!label)
            {
                label = Instantiate(GameData.Prefabs.Get("label"), transform);
                label.transform.localPosition = new Vector3(30, 30, 0);
            }
            label.SetActive(true);
        }

        public virtual void HideInfo()
        {
            label.SetActive(false);
        }
    }
}