using UnityEngine;
using UnityEngine.UI;

namespace Canute.Module
{
   
    public class ProgressBar : MonoBehaviour
    {
        public float progress;
        public Vector3 initialPosition;
        public GameObject background;
        public GameObject processing;

       
        public Image bg => background.GetComponent<Image>();
       
        public Image progressImage => processing.GetComponent<Image>();


        private float Distance => -initialPosition.x;

        
        public float Progress { get => progress; set => progress = value > 0 ? value : 0; }

       
        public void Awake()
        {
            initialPosition = processing.transform.localPosition;
        }

      
        public virtual void Start()
        {

        }

     
        public void SetFull()
        {
            SetProgress(1);
        }

        /// <summary> Set the progress of bar </summary>
        /// <param name="f"> progress </param>
        public void SetProgress(float f)
        {
            var a = initialPosition;
            a.x += (f > 1 ? Distance : (Distance * f));
            processing.transform.localPosition = a;
        }

    }

}