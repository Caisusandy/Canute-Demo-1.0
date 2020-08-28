using UnityEngine;
using UnityEngine.UI;

namespace Canute.BattleSystem
{
    public class ArmyDamageDisplayer : MonoBehaviour
    {
        public Text displayer;
        public Canvas canvas;
        public int damage;

        public readonly float disapearTime = 1.5f;
        public float time;
        public float dt;
        public float dx;
        public float dy;

        private void Awake()
        {
            dx = UnityEngine.Random.value;
            dy = UnityEngine.Random.value;
        }

        private void Start()
        {
            canvas.sortingLayerName = "UI";
            transform.localPosition = new Vector3(7.5f, 7.5f, 0);
            displayer.text = damage.ToString();
        }

        private void Update()
        {
            dt = Time.deltaTime;
            time += dt;

            transform.localScale += new Vector3(dt / 3, dt / 3, 0);
            transform.position += new Vector3(dx, dy, 0) * dt;
            if (time > disapearTime)
            {
                transform.localScale -= new Vector3(dt / 1.5f, dt / 1.5f, 0);
            }
            if (time > disapearTime + 0.2)
            {
                Destroy(gameObject);
            }
        }
    }
}
