using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class Head : MonoBehaviour
{
    [Header("Squishy Movement")]
    public float squishTime = 1.2f;
    public float moveForce = 8f;
    public AnimationCurve squishCurve;

    [Header("Jump Settings")]
    public float jumpForce = 5f;

    [Header("Launch Settings")]
    public float launchForce;
    [SerializeField] private InputAction jumpAction;

    [Header("Input")]
    [SerializeField] private InputAction moveAction;

    private Rigidbody2D rb;
    private float timer;
    private int direction = 0;
    private bool isFacingRight = true;
    public bool OnGround = false;

    private void Awake() => rb = GetComponent<Rigidbody2D>();

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();

        moveAction.performed += OnMove;
        moveAction.canceled += ctx => direction = 0;
        jumpAction.performed += OnJump;
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();

        moveAction.performed -= OnMove;
        moveAction.canceled -= ctx => direction = 0;
        jumpAction.performed -= OnJump;
    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void OnMove(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        direction = Mathf.RoundToInt(input.x);

        if (direction > 0 && !isFacingRight) Flip();
        else if (direction < 0 && isFacingRight) Flip();
    }

    private void OnJump(InputAction.CallbackContext ctx)
    {
        if (!MainBody.headConnected || !OnGround) return;
        Jump();
    }

    private void Jump()
    {
        rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void LaunchHead(Vector2 launchDir)
    {
        MainBody.headConnected = false;
        transform.parent = null;
        rb.AddForce(launchDir.normalized * launchForce, ForceMode2D.Impulse);
    }

    private void HandleMovement()
    {
        if (direction == 0) return;

        timer += Time.fixedDeltaTime;
        float t = Mathf.PingPong(timer / squishTime, 1f);
        float forceMultiplier = squishCurve.Evaluate(t);

        Vector2 move = Vector2.right * direction * forceMultiplier * moveForce;
        rb.AddForce(move, ForceMode2D.Force);
    }

    private void Flip()
    {
        isFacingRight = !isFacingRight;

        Vector3 scale = transform.localScale;
        scale.x *= -1;
        transform.localScale = scale;

        if (transform.parent != null)
        {
            Transform body = transform.parent.Find("Body");
            if (body != null)
            {
                Vector3 bodyScale = body.localScale;
                bodyScale.x *= -1;
                body.localScale = bodyScale;
            }
        }
    }
}