using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    [Obsolete]
    public class EndPanel : MonoBehaviour
    {
        public Image fadeOutImage;

        public void OnMouseUp()
        {
            StartCoroutine(Fade());
        }

        public void Close()
        {
            StartCoroutine(Fade());
        }

        public IEnumerator Fade()
        {
            while (true)
            {
                fadeOutImage.enabled = true;
                Color color = fadeOutImage.color;
                color.a += Time.deltaTime;
                fadeOutImage.color = color;
                if (fadeOutImage.color.a > 1)
                {
                    yield return new EntityEventPack(Clear).Execute();
                    yield break;
                }
                else
                {
                    yield return new WaitForEndOfFrame();
                }
            }
        }

        public static void Clear(params object[] vs)
        {
            Debug.Log("Clear scene");

            Game.ClearBattle();
            SceneControl.GotoScene(MainScene.mainHall);
        }
    }
}
