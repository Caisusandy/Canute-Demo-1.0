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

        // Start is called before the first frame update
        private void Start()
        {
            SetFull();
        }

        // Update is called once per frame
        private void Update()
        {
            SetProgress(Progress);
        }

        private void SetFull()
        {
            SetProgress(1);
        }

        /// <summary> Set the progress of bar </summary>
        /// <param name="f"> progress </param>
        public void SetProgress(float f)
        {
            processing.transform.localPosition = new Vector3(initialPosition.x + Distance * f, processing.transform.localPosition.y);
        }

    }

}