using System.Collections.Generic;
using UnityEngine;

namespace Canute.Module
{
    public delegate void EndMotion();
    public class Motion : MonoBehaviour
    {
        public event EndMotion MotionEndEvent;
        public static List<Motion> ongoingMotions = new List<Motion>();

        public Vector3 startingPos;
        public Vector3 finalPos;
        public Vector3 curPos;
        public bool isUIMotion;
        public float speed = 6;
        public float minimumDistance = 0.05f;
        public Space moveSpace;

        // Start is called before the first frame update
        private void Start()
        {
            if (isUIMotion) ongoingMotions.Add(this);
            else ongoingMotions.Remove(this);
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
            MotionEndEvent?.Invoke();
            ongoingMotions.Remove(this);
            Destroy(this);
        }

        public void OnDestroy()
        {
            ongoingMotions.Remove(this);
        }

        public static void SetMotion(GameObject obj, Vector3 finalPos, Space space = Space.World, EndMotion endMotionevent = null, bool isUIMotion = false)
        {
            Motion motion = obj.GetComponent<Motion>();
            if (!obj.GetComponent<Motion>()) { motion = obj.AddComponent<Motion>(); }

            motion.MotionEndEvent += endMotionevent;
            motion.isUIMotion = isUIMotion;

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
        public static void SetMotion(GameObject obj, Vector3 finalPos, EndMotion endMotionevent, bool isUIMotion)
        {
            SetMotion(obj, finalPos, Space.World, endMotionevent, isUIMotion);
        }
        public static void SetMotion(GameObject obj, Vector3 finalPos, bool isUIMotion)
        {
            SetMotion(obj, finalPos, Space.World, null, isUIMotion);
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