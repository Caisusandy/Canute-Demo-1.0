using UnityEngine;

namespace Canute.Module
{
    public class FollowMouseMove : MonoBehaviour
    {
        public Vector3 lastPos;

        public static Vector3 UserInputPosition => Camera.main.ScreenToWorldPoint(Input.mousePosition);

        public void Start()
        {
            lastPos = UserInputPosition;
        }

        public void Update()
        {
            transform.position += UserInputPosition - lastPos;
            lastPos = UserInputPosition;
        }
    }
}