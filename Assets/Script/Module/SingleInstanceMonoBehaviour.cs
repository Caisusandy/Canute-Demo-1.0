using UnityEngine;

namespace Canute.Module
{
    public abstract class SingleInstanceMonoBehaviour : MonoBehaviour
    {
        protected SingleInstanceMonoBehaviour instance;

        private void Awake()
        {
            instance = this;
        }

        // Use this for initialization
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }
    }

}
