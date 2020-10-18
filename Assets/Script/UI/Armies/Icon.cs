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
        
        public void OnMouseOver() { DisplayInfo(); }
        
        public void OnMouseDown() { DisplayInfo(); }
        
        public void OnMouseExit() { HideInfo(); }
        
        public void OnMouseUp() { HideInfo(); }

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