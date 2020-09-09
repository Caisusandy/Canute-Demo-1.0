using UnityEngine;
using UnityEngine.UI;

namespace Canute.UI.Hallway
{
    /// <summary>  </summary>
    public class HWCharacter : MonoBehaviour
    {
        public Character character;
        public Image characterImage;
        public GameObject talk;
        public GameObject talkPrefab;
        public bool isTalking;
        public float time;

        public void Start()
        {
            if (!character)
            {
                Destroy(gameObject);
                return;
            }

            characterImage.sprite = character.Portrait;
        }

        public void Update()
        {
            if (isTalking)
            {
                time += Time.deltaTime;
                if (time > 10)
                {
                    time = 0;
                    EndTalk();
                }
            }
        }


        public void OnMouseDown()
        {
            Talk();
        }

        public virtual void Talk()
        {
            EndTalk();
            isTalking = true;
            talk = Instantiate(talkPrefab, transform);
            time = 0;
            talk.SetActive(true);
            string wordLine = character.RandomWordLine;
            Debug.Log(wordLine.Length);
            for (int i = 0, j = 0; i < wordLine.Length; i++, j++)
            {
                if (j < 50)
                    continue;
                if (wordLine[i].ToString() == " ")
                {
                    wordLine = wordLine.Insert(i, "\n");
                    j = 0;
                }
            }

            talk.GetComponent<Label>().text.text = wordLine;
            var c = talk.AddComponent<Canvas>();
            c.overrideSorting = true;
            c.sortingLayerName = "UI";
            c.sortingOrder = -transform.GetSiblingIndex();
        }

        public virtual void EndTalk()
        {
            Destroy(talk);
            talk = null;
        }
    }
}
