using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

// Class to manage Player movement: horizontal and vertical movement, dash
public class PlayerMovement : BasicMovement
{
    private Vector2 movementVector = Vector2.zero; // Movement input
    private MainControls input = null; // Input system
    private MovementDash dash_script;

    private void Awake()
    {
        input = new MainControls();
    }

    protected override void Start()
    {
        base.Start();
        dash_script = GetComponent<MovementDash>();
        if (dash_script != null)
        {
            dash_script.OnDashingChanged += setCanMove;
            dash_script.OnDashingChanged += setUseForceToApply;
        }
    }

    private void OnEnable()
    {
        input.Enable();
        input.gameplay.movement.performed += OnMovementPerformed;
        input.gameplay.movement.canceled += OnMovementCancelled;
        input.gameplay.dash.performed += OnDashPerformed;
    }

    private void OnDisable()
    {
        input.Disable();
        input.gameplay.movement.performed -= OnMovementPerformed;
        input.gameplay.movement.canceled -= OnMovementCancelled;
        input.gameplay.dash.performed -= OnDashPerformed;
    }

    private void FixedUpdate()
    {
        Move(movementVector);
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

    private void OnDashPerformed(InputAction.CallbackContext value)
    {
        if (value.performed)
        {
            MovementDash dash_script = GetComponent<MovementDash>();
            if (dash_script != null)
            {
                StartCoroutine(dash_script.Dash(movementVector));
            }
        }
    }
}
