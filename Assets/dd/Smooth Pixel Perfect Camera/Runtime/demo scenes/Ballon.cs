using UnityEngine;

namespace SmoothCam
{

    [RequireComponent(typeof(SpriteRenderer))]
    public class Ballon : MonoBehaviour
    {

        [SerializeField] Vector2Int despawnBounds;
        [SerializeField] Vector2 speed;

        void Update()
        {
            transform.position += new Vector3(speed.x, speed.y, 0) * Time.deltaTime;
            if (Mathf.Abs(transform.position.x - 0) > despawnBounds.x) Destroy(gameObject);
            if (Mathf.Abs(transform.position.y - 0) > despawnBounds.y) Destroy(gameObject);
        }
    }

}