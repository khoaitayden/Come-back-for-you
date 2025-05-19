using UnityEngine;

public class HeadBodyAndTwoArms : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Rigidbody2D headRb;
    [SerializeField] private Rigidbody2D bodyRb;
    [SerializeField] private Rigidbody2D leftArmRb;
    [SerializeField] private Rigidbody2D rightArmRb;
    [SerializeField] private LayerMask groundLayer;

    [Header("Movement Settings")]
    [SerializeField] private float spinTorque;
    [SerializeField] private float jumpForce;
    [SerializeField] private float headInfluence;
    [SerializeField] private float bodyInfluence;
    [SerializeField] private float midAirControlForce;

    [Header("Ground Check Settings")]
    [SerializeField] private Vector2 groundCheckSize;
    [SerializeField] private float groundCheckDistance;

    private Vector2 moveInput;
    private bool wantsToJump;
    private bool isGrounded;
    private int groundedArmsCount;

    void Update()
    {
        GetPlayerInput();
        CheckGroundedState();
        CheckJumpInput();
    }

    void FixedUpdate()
    {
        if (headRb == null || leftArmRb == null || rightArmRb == null || bodyRb == null) return;

        HandleArmSpinning();
        HandleMidAirControl();
        HandleJump();
    }

    private void GetPlayerInput()
    {
        moveInput = PlayerInputHandler.Instance != null ? PlayerInputHandler.Instance.MoveInput : Vector2.zero;
    }

    private void CheckGroundedState()
    {
        bool leftArmGrounded = IsArmGrounded(leftArmRb);
        bool rightArmGrounded = IsArmGrounded(rightArmRb);

        groundedArmsCount = (leftArmGrounded ? 1 : 0) + (rightArmGrounded ? 1 : 0);
        isGrounded = groundedArmsCount > 0;
    }

    private bool IsArmGrounded(Rigidbody2D armRb)
    {
        return Physics2D.BoxCast(
            armRb.position,
            groundCheckSize,
            armRb.rotation,
            Vector2.down,
            groundCheckDistance,
            groundLayer
        );
    }

    private void CheckJumpInput()
    {
        wantsToJump = isGrounded && moveInput.y > 0.1f && Mathf.Abs(moveInput.x) < 0.05f;
    }

    private void HandleArmSpinning()
    {
        if (Mathf.Abs(moveInput.x) > 0.05f)
        {
            float spinDirection = moveInput.x > 0 ? -1f : 1f;
            leftArmRb.AddTorque(spinDirection * spinTorque, ForceMode2D.Force);
            rightArmRb.AddTorque(spinDirection * spinTorque, ForceMode2D.Force);
        }
    }

    private void HandleMidAirControl()
    {
        if (!isGrounded && Mathf.Abs(moveInput.x) > 0.05f)
        {
            Vector2 controlForce = Vector2.right * moveInput.x * midAirControlForce;
            bodyRb.AddForce(controlForce, ForceMode2D.Force);
        }
    }

    private void HandleJump()
    {
        if (wantsToJump)
        {
            float adjustedJumpForce = jumpForce * (groundedArmsCount == 2 ? 1f : 2f / 3f);

            Vector2 upwardForce = Vector2.up * adjustedJumpForce;
            leftArmRb.AddForce(upwardForce, ForceMode2D.Impulse);
            rightArmRb.AddForce(upwardForce, ForceMode2D.Impulse);
            headRb.AddForce(upwardForce * headInfluence, ForceMode2D.Impulse);
            bodyRb.AddForce(upwardForce * bodyInfluence, ForceMode2D.Impulse);

            wantsToJump = false;
        }
    }

    void OnDrawGizmos()
    {
        DrawGroundCheckGizmo(leftArmRb);
        DrawGroundCheckGizmo(rightArmRb);
    }

    private void DrawGroundCheckGizmo(Rigidbody2D armRb)
    {
        if (armRb == null) return;

        Gizmos.color = IsArmGrounded(armRb) ? Color.green : Color.red;
        Gizmos.matrix = Matrix4x4.TRS(
            armRb.position,
            Quaternion.Euler(0, 0, armRb.rotation),
            Vector3.one
        );
        Gizmos.DrawWireCube(
            Vector2.down * groundCheckDistance,
            new Vector3(groundCheckSize.x, groundCheckSize.y, 1)
        );
        Gizmos.matrix = Matrix4x4.identity;
    }

    public void SetArmRbs(Rigidbody2D newLeftArmRb, Rigidbody2D newRightArmRb)
    {
        leftArmRb = newLeftArmRb;
        rightArmRb = newRightArmRb;
    }
}