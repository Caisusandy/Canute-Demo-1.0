using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Canute.UI
{
    public class Main : MonoBehaviour
    {
        public GameObject bg;
        private void Start()
        {
            StartCoroutine("BackgroundMotion");
        }
        // Update is called once per frame
        void Update()
        {

        }
        private IEnumerator BackgroundMotion()
        {
            Vector3 basePos = bg.transform.position;
            var time = 0f;
            while (true)
            {
                time += Time.deltaTime / 4f;
                var displacement = new Vector3(Mathf.Cos(7 * time), Mathf.Sin(6 * time), 0);
                bg.transform.position = basePos + displacement / 6 / 3;

                yield return new WaitForFixedUpdate();
            }
        }
    }
}