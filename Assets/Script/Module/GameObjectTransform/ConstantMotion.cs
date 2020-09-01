using UnityEngine;

namespace Canute.Module
{
    public class ConstantMotion : Motion
    {
        public Vector3 displacePerSecond;
        public float second;
        public float currentSecond;


        // Start is called before the first frame update
        private void Start()
        {
            startingPos = curPos = transform.position;
            if (isUIMotion) ongoingMotions.Add(this);
            else ongoingMotions.Remove(this);
        }

        // Update is called once per frame
        private void Update()
        {
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
            transform.position += displacePerSecond / 60;

            if (currentSecond > second)
            {
                transform.position = finalPos;
                Arrive();
            }
        }

        public override void LocalMotion()
        {
            curPos = transform.localPosition;
            transform.localPosition += displacePerSecond / 60;

            if (currentSecond > second)
            {
                transform.localPosition = finalPos;
                Arrive();
            }
        }


        public static void SetMotion(GameObject obj, Vector3 displacePerSecond, float sec, Space space = Space.World, EndMotion endMotionevent = null, bool isUIMotion = false)
        {
            ConstantMotion motion = obj.GetComponent<ConstantMotion>();
            motion.isUIMotion = isUIMotion;
            if (!obj.GetComponent<ConstantMotion>()) { motion = obj.AddComponent<ConstantMotion>(); }
            motion.MotionEndEvent += endMotionevent;
            motion.second = sec;
            motion.displacePerSecond = displacePerSecond;
        }

        public static void SetMotion(GameObject obj, Vector3 displacePerSecond, float sec, EndMotion endMotionevent)
        {
            SetMotion(obj, displacePerSecond, sec, Space.World, endMotionevent);
        }
    }
}