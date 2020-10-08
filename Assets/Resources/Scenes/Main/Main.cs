using Canute.UI.LevelStart;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI
{
    public class Main : MonoBehaviour
    {
        public GameObject bg;
        public Button nextBattle;
        public Text nextBattleName;

        private void Start()
        {
            StartCoroutine("BackgroundMotion");
            LevelTree.Level level = GameData.Levels.GetNext();
            if (level == null) nextBattle.gameObject.SetActive(false);
            else { nextBattleName.text += "\n<color=#bfc85a><size=20>Level</size></color>".Replace("Level", level.Title); }
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


        public void GotoNextBattle()
        {
            var level = GameData.Levels.GetNext();
            if (level != null)
            {
                GameData.LevelStart.OpenLevelPanel(level.Name);
            }
        }
    }
}