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
        public int speakDuration;

        public void Start()
        {
            if (!character)
            {
                Destroy(gameObject);
                return;
            }

            gameObject.name = character.Name;
            characterImage.sprite = character.Portrait;
            speakDuration = Random.Range(4, 15);
        }

        public void Update()
        {
            time += Time.deltaTime;
            if (time > speakDuration)
            {
                if (isTalking) EndTalk();
                else Talk();

                speakDuration = Random.Range(4, 15);
                time = 0;
            }
        }


        public void OnMouseDown()
        {
            if (isTalking) EndTalk();
            else Talk();
        }

        public virtual void Talk()
        {
            EndTalk();
            isTalking = true;
            talk = Instantiate(talkPrefab, transform);
            time = 0;
            talk.SetActive(true);
            string wordLine = character.GetRandomWordLine();
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
            isTalking = false;
            talk = null;
        }
    }
}
