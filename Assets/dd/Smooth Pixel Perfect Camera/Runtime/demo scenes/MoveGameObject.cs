using UnityEngine;

namespace SmoothCam
{

    public class MoveGameObject : MonoBehaviour
    {
        public float moveDistance = 4f;
        public float moveSpeed = 0.25f;

        private Vector2 startPos;
        private Vector2 endPos;

        private void Awake()
        {
            startPos = transform.position;
            endPos = startPos + Vector2.right * moveDistance;
        }

        private void Update()
        {
            float pingPong = Mathf.PingPong(Time.time * moveSpeed, 1);
            transform.position = Vector2.Lerp(startPos, endPos, pingPong);
        }
    }

}