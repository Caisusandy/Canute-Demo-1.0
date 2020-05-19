using UnityEngine;

namespace Canute.Module
{
    public delegate void EndMotion();
    public class Motion : MonoBehaviour
    {
        public event EndMotion MotionEndEvent;

        public Vector3 startingPos;
        public Vector3 finalPos;
        public Vector3 curPos;
        public float speed = 6;
        public float minimumDistance = 0.01f;
        public Space moveSpace;

        // Start is called before the first frame update
        private void Start()
        {
            startingPos = curPos = transform.position;
        }

        // Update is called once per frame
        private void Update()
        {
            WorldMotion();
        }

        public virtual void Move()
        {
            if (moveSpace == Space.Self)
            {
                LocalMotion();
            }
            else
            {
                WorldMotion();
            }
        }

        public virtual void WorldMotion()
        {
            curPos = transform.position;
            transform.position = Vector3.Lerp(curPos, finalPos, speed * Time.deltaTime);
            transform.position = new Vector3(transform.position.x, transform.position.y, finalPos.z);

            if (Vector3.Magnitude(transform.position - finalPos) < minimumDistance)
            {
                transform.position = finalPos;
                Arrive();
            }
        }

        public virtual void LocalMotion()
        {
            curPos = transform.localPosition;
            transform.localPosition = Vector3.Lerp(curPos, finalPos, speed * Time.deltaTime);

            if (Vector3.Magnitude(transform.localPosition - finalPos) < minimumDistance)
            {
                transform.localPosition = finalPos;
                Arrive();
            }
        }

        public virtual void Arrive()
        {
            Destroy(gameObject.GetComponent<Motion>());
            MotionEndEvent?.Invoke();
        }

        public static void SetMotion(GameObject obj, Vector3 finalPos, Space space = Space.World, EndMotion endMotionevent = null)
        {
            Motion motion = obj.GetComponent<Motion>();
            if (!obj.GetComponent<Motion>()) { motion = obj.AddComponent<Motion>(); }
            motion.MotionEndEvent += endMotionevent;


            if (space == Space.Self)
            {
                // Debug.Log(FinalPos + Obj.transform.parent.position);
                motion.finalPos = finalPos + obj.transform.parent.position;
            }
            else
            {
                // Debug.Log(FinalPos); 
                motion.finalPos = finalPos;
            }
        }

        public static void SetMotion(GameObject obj, Vector3 finalPos, EndMotion endMotionevent)
        {
            SetMotion(obj, finalPos, Space.World, endMotionevent);
        }

        public static void StopMotion(GameObject gameObject)
        {
            if (gameObject.GetComponent<Motion>())
            {
                Destroy(gameObject.GetComponent<Motion>());
            }
        }
    }
}