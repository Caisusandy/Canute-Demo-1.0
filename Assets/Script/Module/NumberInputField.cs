using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Canute.Module
{
    [RequireComponent(typeof(InputField))]
    public class NumberInputField : MonoBehaviour
    {
        public int step;
        public int max;
        public bool hasMax;
        public int min;
        public bool hasMin;
        public InputField InputField => GetComponent<InputField>();

        public void Add()
        {
            int newValue = GetValue() + step;
            InputField.text = (newValue > max && hasMax ? max : newValue) + "";
            InputField.onEndEdit?.Invoke(InputField.text);
        }

        public void Minus()
        {
            int newValue = GetValue() - step;
            InputField.text = (newValue < min && hasMin ? min : newValue) + "";
            InputField.onEndEdit?.Invoke(InputField.text);
        }

        public int GetValue()
        {
            int a;
            if (int.TryParse(InputField.text, out a))
            {
                return a;
            }
            return 0;
        }
    }
}