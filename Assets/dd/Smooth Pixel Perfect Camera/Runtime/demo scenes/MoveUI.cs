using UnityEngine;

namespace SmoothCam
{

    public class MoveUI : MonoBehaviour
    {
        public float moveDistance = 4f;
        public float moveSpeed = 0.25f;

        private Vector3 startPos;
        private Vector3 endPos;

        private RectTransform rectTransform;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
            startPos = rectTransform.localPosition;
            endPos = startPos + Vector3.right * moveDistance;
        }



        private void Update()
        {
            float pingPong = Mathf.PingPong(Time.time * moveSpeed, 1);
            rectTransform.localPosition = Vector3.Lerp(startPos, endPos, pingPong);
        }
    }

}