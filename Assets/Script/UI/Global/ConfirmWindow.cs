using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

namespace Canute.UI
{
    public class ConfirmWindow : MonoBehaviour
    {
        public static ConfirmWindow instance;
        public static Action action;

        public Text info;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        public void Confirm()
        {
            action?.Invoke();
            Close();
        }

        public void Close()
        {
            Destroy(gameObject);
            instance = null;
        }

        public static ConfirmWindow CreateConfirmWindow(Action action, string info)
        {
            ConfirmWindow.action = action;
            if (instance)
            {
                return instance;
            }

            instance = Instantiate(GameData.Prefabs.Get("confirmWindow")).GetComponent<ConfirmWindow>();


            instance.info.text = info;
            instance.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10));

            return instance;
        }
    }
}