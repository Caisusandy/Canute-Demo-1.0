using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.Assets.Script.Global
{
    public class TipDisplayer : MonoBehaviour
    {
        public Text tip;
        public Image bg;
        public bool off;
        Coroutine coroutine;
        private void Start()
        {
            LoadTip();
            coroutine = StartCoroutine(LoadAndChange());
        }

        IEnumerator LoadAndChange()
        {
            float time = 0;
            while (true)
            {
                yield return new WaitForFixedUpdate();
                time += Time.deltaTime;
                if (time > 8)
                {
                    tip.text = "";
                    bg.enabled = false;
                }
                if (time > 10)
                {
                    time = 0;
                    LoadTip();
                    bg.enabled = true;
                }

            }
        }


        public void LoadTip()
        {
            int tipCount = 16;
            tip.text = ("Canute.Tips." + UnityEngine.Random.Range(0, tipCount)).Lang();
        }

        public void OnMouseUp()
        {
            off = !off;
            if (off)
            {
                StopCoroutine(coroutine);
                tip.enabled = !off;
                bg.enabled = !off;
            }
            else
            {
                StartCoroutine(LoadAndChange());
                tip.enabled = !off;
                bg.enabled = !off;
                LoadTip();
            }
        }
    }
}
