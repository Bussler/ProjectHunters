using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{

    private MainControls input = null; // Input system
    private Vector2 movementVector = Vector2.zero; // Movement input
    Rigidbody2D rb = null; // Rigidbody2D component through which we apply force
    [SerializeField]
    private float moveSpeed = 7f;
    
    public Vector2 forceToApply = Vector2.zero; // used for knockback if a projectile hits the player
    [SerializeField]
    private float forceDamping = 1.2f;

    private void Awake()
    {
        input = new MainControls();
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        input.Enable();
        input.gameplay.movement.performed += OnMovementPerformed;
        input.gameplay.movement.canceled += OnMovementCancelled;
    }

    private void OnDisable()
    {
        input.Disable();
        input.gameplay.movement.performed -= OnMovementPerformed;
        input.gameplay.movement.canceled -= OnMovementCancelled;
    }

    private void FixedUpdate()
    {
        //rb.velocity = movementVector * moveSpeed;
        Vector2 moveForce = movementVector * moveSpeed;
        moveForce += forceToApply;
        forceToApply /= forceDamping;
        if (Mathf.Abs(forceToApply.x) <= 0.01f && Mathf.Abs(forceToApply.y) <= 0.01f)
        {
            forceToApply = Vector2.zero;
        }
        rb.velocity = moveForce;
    }

    // Subscribe to events of input system
    private void OnMovementPerformed(InputAction.CallbackContext value)
    {
        movementVector = value.ReadValue<Vector2>();
    }

    // Stop the player; reset movement vector
    private void OnMovementCancelled(InputAction.CallbackContext value)
    {
        movementVector = Vector2.zero;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            forceToApply += new Vector2(-20, 0); // add initial vector from bullet
            Destroy(collision.gameObject);
        }
    }
}
