using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Canute.UI
{
    [RequireComponent(typeof(Collider2D))]
    public class IconExplain : MonoBehaviour
    {
        public string key;
        public Label label;

        public void OnMouseDown()
        {
            label = Label.GetLabel(transform);
            label.text.text = key.Lang();
        }

        private void OnMouseOver()
        {
            label = Label.GetLabel(transform);
            label.text.text = key.Lang();
        }

        private void OnMouseExit()
        {
            Destroy(label.Exist()?.gameObject);
        }

        private void OnMouseUp()
        {
            Destroy(label.Exist()?.gameObject);
        }
    }
}
