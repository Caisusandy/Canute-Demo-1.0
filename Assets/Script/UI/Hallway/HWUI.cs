using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.UI.Hallway
{
    public class HWUI : MonoBehaviour
    {
        public GameObject hallwayLight;
        public GameObject HWCharacterPrefab;
        public GameObject Hallway;
        public int HallwayLength;

        public void Start()
        {
            var characters = Canute.Hallway.CameOut();
            Debug.Log(characters.Count);
            foreach (var character in characters)
            {
                var characterUI = Instantiate(HWCharacterPrefab, Hallway.transform).GetComponent<HWCharacter>();
                characterUI.character = character;
                characterUI.transform.position = Hallway.transform.position + (0.5f - UnityEngine.Random.value) * HallwayLength * Hallway.transform.position;
                var temp = characterUI.transform.localPosition;
                temp.y = -10 + (0.5f - UnityEngine.Random.value) * 40;
                temp.z = 0;
                characterUI.transform.localPosition = temp;
            }

            StartCoroutine("BackgroundMotion");

        }

        private IEnumerator BackgroundMotion()
        {
            var time = 0f;
            while (true)
            {
                time += Time.deltaTime;

                if (time < 1)
                    yield return new WaitForFixedUpdate();
                else break;
            }
            time = 0f;
            Vector3 basePos = hallwayLight.transform.localPosition;
            while (true)
            {
                time += Time.deltaTime / 64f;
                var displacement = new Vector3(Mathf.Cos(7 * time), Mathf.Sin(6 * time), 0);
                hallwayLight.transform.localPosition = basePos + displacement / 100;

                yield return new WaitForSeconds(1 / 60);
            }
        }
    }
}
