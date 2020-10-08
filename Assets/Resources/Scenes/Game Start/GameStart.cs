using Canute.Module;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public class GameStart : MonoBehaviour
    {
        public static bool initialized = false;
        public GameObject bg;
        public GameObject menu;
        public AudioSource audioSource;
        public AudioSource pre;

        public Text title;
        public Text line;
        public Text[] MenuText;

        // Start is called before the first frame update
        void Awake()
        {
            if (initialized)
            {
                pre.enabled = false;
                StartCoroutine("MenuShowImmediate");
                StartCoroutine("TitleShowImmediate");
                menu.transform.position = Vector3.zero;
                audioSource.enabled = true;
            }
            else
            {
                StartCoroutine("StartShowMenu");
                StartCoroutine("MenuShow");
                StartCoroutine("TitleShow");
                StartCoroutine("LineShowAndHide");
                initialized = true;
            }
        }

        private EndMotion EndMotion()
        {
            return () =>
            {
                audioSource.enabled = true;
                StartCoroutine("BackgroundMotion");
            };
        }

        // Update is called once per frame
        void Update()
        {
        }


        private IEnumerator StartShowMenu()
        {
            var time = 0f;
            while (true)
            {
                time += Time.deltaTime;

                if (time < 3)
                    yield return new WaitForFixedUpdate();
                else break;
            }

            var motion = Module.LinearMotion.SetMotion(menu, Vector3.zero, EndMotion());
            motion.second = 7.3f;
        }

        private IEnumerator BackgroundMotion()
        {
            Vector3 basePos = bg.transform.position;
            var time = 0f;
            while (true)
            {
                time += Time.deltaTime / 12f;
                var displacement = new Vector3(Mathf.Cos(7 * time), Mathf.Sin(6 * time), 0);
                bg.transform.position = basePos + displacement / 6 / 3;

                yield return new WaitForFixedUpdate();
            }
        }

        private IEnumerator MenuShow()
        {
            var time = 0f;
            while (true)
            {
                time += Time.deltaTime;
                if (time < 10.3)
                    yield return new WaitForFixedUpdate();
                else break;
            }

            time = 0;
            while (true)
            {
                time += Time.deltaTime;
                foreach (var item in MenuText)
                {
                    if (!item)
                    {
                        continue;
                    }
                    var c = item.color;
                    c.a = time / 2;
                    item.color = c;
                }
                if (time < 2)
                    yield return new WaitForFixedUpdate();
                else break;
            }
        }

        private IEnumerator MenuShowImmediate()
        {
            var time = 0f;
            while (true)
            {
                time += Time.deltaTime;
                foreach (var item in MenuText)
                {
                    if (!item) continue;
                    var c = item.color;
                    c.a = time / 2;
                    item.color = c;
                }
                if (time < 2)
                    yield return new WaitForFixedUpdate();
                else break;
            }
        }

        private IEnumerator TitleShow()
        {
            var time = 0f;
            while (true)
            {
                time += Time.deltaTime;
                if (time < 10.3)
                    yield return new WaitForFixedUpdate();
                else break;
            }

            time = 0;
            while (true)
            {
                time += Time.deltaTime;
                var c = title.color;
                c.a = time;
                title.color = c;
                if (time < 1)
                    yield return new WaitForFixedUpdate();
                else break;
            }
        }
        private IEnumerator TitleShowImmediate()
        {
            var time = 0f;
            while (true)
            {
                time += Time.deltaTime;
                var c = title.color;
                c.a = time;
                title.color = c;
                if (time < 1)
                    yield return new WaitForFixedUpdate();
                else break;
            }
        }

        private IEnumerator LineShowAndHide()
        {
            var time = 0f;
            while (true)
            {
                time += Time.deltaTime;
                var c = line.color;
                c.a = time / 2;
                line.color = c;
                if (time < 2)
                    yield return new WaitForFixedUpdate();
                else break;
            }
            time = 0;
            while (true)
            {
                time += Time.deltaTime;
                var c = line.color;
                c.a = 1 - time / 2;
                line.color = c;
                if (time < 2)
                    yield return new WaitForFixedUpdate();
                else break;
            }
        }

    }
}