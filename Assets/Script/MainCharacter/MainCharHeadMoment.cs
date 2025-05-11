using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 8f;
    [SerializeField] private float acceleration = 15f;
    [SerializeField] private float deceleration = 20f;
    [SerializeField] private float groundDrag = 5f;

    [Header("Input Settings")]
    [SerializeField] private InputAction moveAction;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private bool isFacingRight = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        moveAction.performed += OnMove;
        moveAction.canceled += OnStop;
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnStop;
        moveAction.Disable();
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        
        if (moveInput.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput.x < 0 && isFacingRight)
        {
            Flip();
        }
    }

    private void OnStop(InputAction.CallbackContext context)
    {
        moveInput = Vector2.zero;
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        ApplyDrag();
    }

    private void ApplyMovement()
    {
        float targetSpeed = moveInput.x * moveSpeed;
        float speedDifference = targetSpeed - rb.linearVelocity.x;
        float accelerationRate;
        
        if (Mathf.Abs(targetSpeed) > 0.1f)
        {
            accelerationRate = acceleration;
        }
        else
        {
            accelerationRate = deceleration;
        }
        
        float movementForce = speedDifference * accelerationRate;
        rb.AddForce(movementForce * Vector2.right, ForceMode2D.Force);
    }

    private void ApplyDrag()
    {
        if (Mathf.Abs(moveInput.x) < 0.1f)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = 0f;
        }
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;
    }
}