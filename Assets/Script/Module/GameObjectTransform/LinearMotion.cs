using UnityEngine;

namespace Canute.Module
{
    public class LinearMotion : Motion
    {
        public float second = 1;
        public float currentSecond;

        public Vector3 Delta => finalPos - startingPos;
        public float Percentage => currentSecond / second;

        // Start is called before the first frame update
        private void Start()
        {
            startingPos = curPos = transform.position;
            if (isUIMotion) ongoingMotions.Add(this);
            else ongoingMotions.Remove(this);
            currentSecond = 0;
        }

        // Update is called once per frame
        private void Update()
        {
            Debug.Log(Delta * Time.deltaTime);
            currentSecond += Time.deltaTime;
            WorldMotion();
        }

        public override void Move()
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

        public override void WorldMotion()
        {
            curPos = transform.position;
            transform.position = startingPos + Delta * Percentage;

            if (currentSecond > second)
            {
                transform.position = finalPos;
                Arrive();
            }
        }

        public override void LocalMotion()
        {
            curPos = transform.localPosition;
            transform.localPosition += startingPos + Delta * Percentage;

            if (currentSecond > second)
            {
                transform.localPosition = finalPos;
                Arrive();
            }
        }


        public static new void SetMotion(GameObject obj, Vector3 finalPos, Space space = Space.World, EndMotion endMotionevent = null, bool isUIMotion = false)
        {
            Motion motion = obj.GetComponent<Motion>();
            motion.isUIMotion = isUIMotion;
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

        public static new void SetMotion(GameObject obj, Vector3 finalPos, EndMotion endMotionevent)
        {
            SetMotion(obj, finalPos, Space.World, endMotionevent);
        }

        public static new void StopMotion(GameObject gameObject)
        {
            if (gameObject.GetComponent<Motion>())
            {
                Destroy(gameObject.GetComponent<Motion>());
            }
        }
    }
}