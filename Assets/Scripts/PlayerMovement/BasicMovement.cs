using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    protected Rigidbody2D rb = null; // Rigidbody2D component through which we apply force
    
    [SerializeField]
    private float moveSpeed = 7f;

    public Vector2 forceToApply = Vector2.zero; // used for knockback if a projectile hits the player
    [SerializeField]
    private float forceDamping = 1.2f;

    private bool canMove = true;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected void Move(Vector2 movementVector)
    {
        if (!canMove || rb == null)
        {
            return;
        }

        Vector2 moveForce = movementVector.normalized * moveSpeed;
        moveForce += forceToApply;
        forceToApply /= forceDamping;
        if (Mathf.Abs(forceToApply.x) <= 0.01f && Mathf.Abs(forceToApply.y) <= 0.01f)
        {
            forceToApply = Vector2.zero;
        }
        rb.velocity = moveForce;
    }
    public void setCanMove(bool value)
    {
        this.canMove = !canMove;
    }
}
