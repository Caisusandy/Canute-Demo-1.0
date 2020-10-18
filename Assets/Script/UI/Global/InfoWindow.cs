using UnityEngine;
using UnityEngine.UI;
using System;

namespace Canute.UI
{
    public class InfoWindow : MonoBehaviour
    {
        public static InfoWindow instance;
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

        public static InfoWindow Create(Action action, string info)
        {
            ConfirmWindow.action = action;
            if (instance)
            {
                return instance;
            }

            instance = Instantiate(GameData.Prefabs.Get("infoWindow")).GetComponent<InfoWindow>();


            instance.info.text = info;
            instance.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10));

            return instance;
        }

        public static InfoWindow Create(string info)
        {
            ConfirmWindow.action = () => { };
            if (instance)
            {
                return instance;
            }

            instance = Instantiate(GameData.Prefabs.Get("infoWindow")).GetComponent<InfoWindow>();


            instance.info.text = info;
            instance.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10));

            return instance;
        }
        public static InfoWindow Create(Transform transform, Action action, string info)
        {
            ConfirmWindow.action = action;
            if (instance)
            {
                return instance;
            }

            instance = Instantiate(GameData.Prefabs.Get("infoWindow"), transform).GetComponent<InfoWindow>();


            instance.info.text = info;
            instance.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10));
            instance.transform.localScale = Vector3.one;

            return instance;
        }

        public static InfoWindow Create(Transform transform, string info)
        {
            ConfirmWindow.action = () => { };
            if (instance)
            {
                return instance;
            }

            instance = Instantiate(GameData.Prefabs.Get("infoWindow"), transform).GetComponent<InfoWindow>();


            instance.info.text = info;
            instance.transform.position = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width / 2, Screen.height / 2, 10));
            instance.transform.localScale = Vector3.one;

            return instance;
        }
    }
}