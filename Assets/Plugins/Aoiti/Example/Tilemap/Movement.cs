using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    public Transform movePoint;
    public LayerMask stopMovementMask;
    public float moveSpeed = 5f;
    public Rigidbody2D rb;
    public Animator animator;
    Vector2 movement;
    //·Ö¸î



    Vector3 lastDirection = Vector3.zero;
    bool moveDone = false;
    List<WorldTile> reachedPathTiles = new List<WorldTile>();

    void Start()
    {
        movePoint.parent = null;
    }

    void Update()
    {
        SetMovementVector();

        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, moveSpeed * Time.fixedDeltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) <= .001f)
        {
            if (Mathf.Abs(movement.x) == 1f)
            {
                // we add 0.5f to 'y' component of the 'position'
                // to account the bottom pivot point of the sprite
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(movement.x, 0.5f, 0f), .2f, stopMovementMask))
                {
                    movePoint.position += new Vector3(movement.x, 0f, 0f);
                }
            }
            else if (Mathf.Abs(movement.y) == 1f)
            {
                // we add 0.5f to 'y' component of the 'position'
                // to account the bottom pivot point of the sprite
                if (!Physics2D.OverlapCircle(movePoint.position + new Vector3(0, movement.y + 0.5f, 0f), .2f, stopMovementMask))
                {
                    movePoint.position += new Vector3(0f, movement.y, 0f);
                }
            }
        }
    }

    void MovementPerformed()
    {

    }

    void SetMovementVector()
    {

    }

}
