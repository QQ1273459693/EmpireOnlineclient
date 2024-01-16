using UnityEngine;

namespace SmoothCam
{

    public class CameraMover : MonoBehaviour
    {

        public Vector2 speed = Vector2.zero;

        void Update()
        {
            transform.position += new Vector3(speed.x, speed.y, 0) * Time.deltaTime;
        }
    }

}