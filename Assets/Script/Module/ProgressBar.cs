using UnityEngine;

namespace Canute.Module
{

    public class ProgressBar : MonoBehaviour
    {
        public GameObject background;
        public GameObject processing;

        protected float progress;
        protected Vector3 initialPosition;
        private float Distance => -initialPosition.x;

        public float Progress { get => progress; set => progress = value > 0 ? value : 0; }

        private void Awake()
        {
            initialPosition = processing.transform.localPosition;
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