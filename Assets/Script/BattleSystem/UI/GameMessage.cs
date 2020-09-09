using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem.UI
{
    [RequireComponent(typeof(Text))]
    public class GameMessage : MonoBehaviour
    {
        public Text text;
        public string line;
        public float time;

        private void Awake()
        {
            text = GetComponent<Text>();
        }

        // Start is called before the first frame update
        void Start()
        {
            text.text = line;
        }

        // Update is called once per frame
        void Update()
        {
            transform.position += new Vector3(0, Time.deltaTime / 2, 0);
            time += Time.deltaTime;
            text.color = new Color(text.color.r, text.color.g, text.color.b, Mathf.Pow(1 - time / 6, 2));

            if (time > 2)
            {
                Destroy(gameObject);
            }
        }

        public static GameMessage GetNewMessage()
        {
            GameMessage gameMessage = new GameObject("message", typeof(Text), typeof(GameMessage)).GetComponent<GameMessage>();
            gameMessage.GetComponent<Text>().font = Font.CreateDynamicFontFromOSFont("Arial", 0);
            gameMessage.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
            gameMessage.GetComponent<Text>().horizontalOverflow = HorizontalWrapMode.Overflow;
            gameMessage.GetComponent<Text>().color = Color.black;
            gameMessage.GetComponent<Text>().fontSize = 24;
            return gameMessage;
        }
    }
}